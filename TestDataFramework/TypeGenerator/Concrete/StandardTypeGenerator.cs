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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using log4net;
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

            ConstructorInfo defaultConstructor = forType.GetConstructor(Type.EmptyTypes);

            if (defaultConstructor == null)
            {
                StandardTypeGenerator.Logger.Debug("Type has no public default constructor. Type: " + forType);

                object value = this.valueGenerator.GetValue(null, forType);
                this.complexTypeProcessingRecursionGuard.Pop();
                return value;
            }

            object objectToFill = defaultConstructor.Invoke(null);

            fillObject(objectToFill);
            this.complexTypeProcessingRecursionGuard.Pop();

            StandardTypeGenerator.Logger.Debug("Exiting ConstructObject");
            return objectToFill;
        }

        private static PropertyInfo[] GetProperties(object objectToFill)
        {
            PropertyInfo[] targetProperties = objectToFill.GetType().GetPropertiesHelper();
            return targetProperties;
        }

        protected virtual void SetProperty(object objectToFill, PropertyInfo targetPropertyInfo)
        {
            StandardTypeGenerator.Logger.Debug("Entering SetProperty. PropertyInfo: " + targetPropertyInfo.GetExtendedMemberInfoString());

            object targetPropertyValue = this.valueGenerator.GetValue(targetPropertyInfo);
            StandardTypeGenerator.Logger.Debug($"targetPropertyValue: {targetPropertyValue}");
            targetPropertyInfo.SetValue(objectToFill, targetPropertyValue);

            StandardTypeGenerator.Logger.Debug("Exiting SetProperty");
        }

        #endregion Private methods

        #region Public methods

        public virtual object GetObject<T>(ConcurrentDictionary<PropertyInfo, Action<T>> explicitProperySetters)
        {
            StandardTypeGenerator.Logger.Debug($"Entering GetObject. T: {typeof(T)}");

            object result = this.ConstructObject(typeof (T), objectToFill => this.FillObject((T)objectToFill, explicitProperySetters));

            StandardTypeGenerator.Logger.Debug($"Exiting GetObject. result: {result}");
            return result;
        }

        public virtual object GetObject(Type forType)
        {
            StandardTypeGenerator.Logger.Debug($"Entering GetObject. forType: {forType}");

            object result = this.ConstructObject(forType, this.FillObject);

            StandardTypeGenerator.Logger.Debug($"Exiting GetObject. result: {result}");
            return result;
        }

        #endregion Public methods

        #region Protected methods

        protected virtual void FillObject(object objectToFill)
        {
            StandardTypeGenerator.Logger.Debug("Entering FillObject");

            PropertyInfo[] targetProperties = StandardTypeGenerator.GetProperties(objectToFill);

            foreach (PropertyInfo targetPropertyInfo in targetProperties)
            {
                this.SetProperty(objectToFill, targetPropertyInfo);
            }

            StandardTypeGenerator.Logger.Debug("Exiting FillObject");
        }

        protected virtual void FillObject<T>(T objectToFill, ConcurrentDictionary<PropertyInfo, Action<T>> explicitProperySetters)
        {
            StandardTypeGenerator.Logger.Debug("Entering FillObject<T>");

            PropertyInfo[] targetProperties = StandardTypeGenerator.GetProperties(objectToFill);

            foreach (PropertyInfo targetPropertyInfo in targetProperties)
            {
                Action<T> setter;

                if (explicitProperySetters.TryGetValue(targetPropertyInfo, out setter))
                {
                    StandardTypeGenerator.Logger.Debug($"explicit property setter found: {setter}");
                    setter(objectToFill);
                }
                else
                {
                    StandardTypeGenerator.Logger.Debug("no explicit property setter found");
                    this.SetProperty(objectToFill, targetPropertyInfo);
                }
            }

            StandardTypeGenerator.Logger.Debug("Exiting FillObject<T>");
        }

        #endregion Protected methods
    }
}
