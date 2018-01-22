/*
    Copyright 2016, 2017 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using log4net;
using TestDataFramework.DeepSetting;
using TestDataFramework.Logger;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.TypeGenerator.Concrete
{
    public class StandardTypeGenerator : ITypeGenerator
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardTypeGenerator));

        #region Fields

        private readonly IValueGenerator valueGenerator;
        private readonly IHandledTypeGenerator handledTypeGenerator;

        private readonly Stack<Type> complexTypeProcessingRecursionGuard = new Stack<Type>();

        private List<ExplicitPropertySetters> explicitPropertySetters;

        #endregion Fields

        public StandardTypeGenerator(IValueGenerator valueGenerator, IHandledTypeGenerator handledTypeGenerator)
        {
            StandardTypeGenerator.Logger.Debug("Entering constructor");

            this.valueGenerator = valueGenerator;
            this.handledTypeGenerator = handledTypeGenerator;

            StandardTypeGenerator.Logger.Debug("Exiting constructor");
        }

        #region Private methods

        private object ConstructObject(Type forType, Action<object> fillObject)
        {
            StandardTypeGenerator.Logger.Debug("Entering ConstructObject");

            object handledTypeObject = this.handledTypeGenerator.GetObject(forType);

            if (handledTypeObject != null)
            {
                return handledTypeObject;
            }

            if (this.complexTypeProcessingRecursionGuard.Contains(forType))
            {
                StandardTypeGenerator.Logger.Debug("Circular reference encountered. Type: " + forType);

                return Helper.GetDefaultValue(forType);
            }

            this.complexTypeProcessingRecursionGuard.Push(forType);

            bool canBeConstructed = this.InvokeConstructor(forType, out object objectToFillResult);

            if (!canBeConstructed)
            {
                this.complexTypeProcessingRecursionGuard.Pop();
                return objectToFillResult;
            }

            fillObject(objectToFillResult);
            this.complexTypeProcessingRecursionGuard.Pop();

            StandardTypeGenerator.Logger.Debug("Exiting ConstructObject");
            return objectToFillResult;
        }

        private bool InvokeConstructor(Type forType, out object result)
        {
            StandardTypeGenerator.Logger.Debug("Entering StandardTypeGenerator.GetObjectToFill()");

            IOrderedEnumerable<ConstructorInfo> constructors = forType.GetConstructors()
                .OrderBy(constructorInfo => constructorInfo.GetParameters().Length);

            List<object> constructorArguments = null;
            ConstructorInfo resultConstructorInfo = null;
            foreach (ConstructorInfo constructorInfo in constructors)
            {
                ParameterInfo[] parameterInfos = constructorInfo.GetParameters();

                constructorArguments = new List<object>();

                var parametersFound = true;
                foreach (ParameterInfo parameterInfo in parameterInfos)
                {
                    object argument = this.valueGenerator.GetValue(null, parameterInfo.ParameterType);

                    if (argument == null)
                    {
                        parametersFound = false;
                        break;
                    }

                    constructorArguments.Add(argument);
                }

                if (!parametersFound) continue;

                resultConstructorInfo = constructorInfo;
                break;
            }

            if (resultConstructorInfo != null)
            {
                result = resultConstructorInfo.Invoke(constructorArguments.ToArray());
                return true;
            }

            StandardTypeGenerator.Logger.Debug("Type has no public constructor. Type: " + forType);

            if (forType.IsValueType)
            {
                result = Activator.CreateInstance(forType);
                return true;
            }

            result = null;
            return false;
        }

        private void FillObject(object objectToFill, ObjectGraphNode objectGraphNode)
        {
            StandardTypeGenerator.Logger.Debug("Entering FillObject<T>");

            PropertyInfo[] targetProperties = StandardTypeGenerator.GetProperties(objectToFill);

            foreach (PropertyInfo targetPropertyInfo in targetProperties)
            {
                ObjectGraphNode propertyObjectGraphNode = objectGraphNode != null
                    ? new ObjectGraphNode(targetPropertyInfo, objectGraphNode)
                    : null;

                IEnumerable<ExplicitPropertySetters> setters =
                    StandardTypeGenerator.IsPropertyExplicitlySet(this.explicitPropertySetters, propertyObjectGraphNode)
                    .ToList();

                if (setters.Any())
                {
                    StandardTypeGenerator.Logger.Debug($"explicit property setter found");
                    setters.ToList().ForEach(setter => setter.Action(objectToFill));
                }
                else
                {
                    StandardTypeGenerator.Logger.Debug("no explicit property setter found");
                    this.SetProperty(objectToFill, targetPropertyInfo, propertyObjectGraphNode);
                }
            }

            StandardTypeGenerator.Logger.Debug("Exiting FillObject<T>");
        }

        private static IEnumerable<ExplicitPropertySetters> IsPropertyExplicitlySet(IEnumerable<ExplicitPropertySetters> explicitPropertySetters,
            ObjectGraphNode objectGraphNode)
        {
            if (objectGraphNode == null)
            {
                return Enumerable.Empty<ExplicitPropertySetters>();
            }

            IEnumerable<ExplicitPropertySetters> result =
                explicitPropertySetters.Where(
                    setters => StandardTypeGenerator.IsPropertyExplicitlySet(setters, objectGraphNode));
            return result;
        }

        private static bool IsPropertyExplicitlySet(ExplicitPropertySetters explicitPropertySetters, ObjectGraphNode objectGraphNode)
        {
            var stack = new Stack<PropertyInfo>(explicitPropertySetters.PropertyChain);

            while (objectGraphNode.PropertyInfo != null)
            {
                if (!stack.Any())
                {
                    return false;
                }

                PropertyInfo setters = stack.Pop();

                if (!objectGraphNode.PropertyInfo.Name.Equals(setters.Name, StringComparison.Ordinal))
                {
                    return false;
                }

                objectGraphNode = objectGraphNode.Parent;
            }

            return !stack.Any();
        }

        private static PropertyInfo[] GetProperties(object objectToFill)
        {
            PropertyInfo[] targetProperties = objectToFill.GetType().GetPropertiesHelper();
            return targetProperties;
        }

        #endregion Private methods

        #region Protected Methods

        protected virtual void SetProperty(object objectToFill, PropertyInfo targetPropertyInfo, ObjectGraphNode objectGraphNode)
        {
            StandardTypeGenerator.Logger.Debug("Entering SetProperty. PropertyInfo: " + targetPropertyInfo.GetExtendedMemberInfoString());

            object targetPropertyValue = this.valueGenerator.GetValue(targetPropertyInfo, objectGraphNode);
            StandardTypeGenerator.Logger.Debug($"targetPropertyValue: {targetPropertyValue}");
            targetPropertyInfo.SetValue(objectToFill, targetPropertyValue);

            StandardTypeGenerator.Logger.Debug("Exiting SetProperty");
        }

        #endregion Protected Methods

        #region Public methods

        public virtual object GetObject<T>(IEnumerable<ExplicitPropertySetters> explicitProperySetters)
        {
            StandardTypeGenerator.Logger.Debug($"Entering GetObject. T: {typeof(T)}");

            object intrinsicValue = this.valueGenerator.GetIntrinsicValue(null, typeof(T));

            if (intrinsicValue != null)
            {
                return intrinsicValue;
            }

            this.explicitPropertySetters = explicitProperySetters.ToList();

            var parentObjectGraphNode = new ObjectGraphNode(null, null);

            object result = this.ConstructObject(typeof (T), objectToFill => this.FillObject(objectToFill, parentObjectGraphNode));

            StandardTypeGenerator.Logger.Debug($"Exiting GetObject. result: {result}");
            return result;
        }

        public virtual object GetObject(Type forType, ObjectGraphNode objectGraphNode)
        {
            StandardTypeGenerator.Logger.Debug($"Entering GetObject. forType: {forType}");

            object result = this.ConstructObject(forType, objectToFill => this.FillObject(objectToFill, objectGraphNode));

            StandardTypeGenerator.Logger.Debug($"Exiting GetObject. result: {result}");
            return result;
        }

        #endregion Public methods
    }
}
