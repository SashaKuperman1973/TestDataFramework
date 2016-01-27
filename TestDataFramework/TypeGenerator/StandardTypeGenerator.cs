using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.ValueGenerator;

namespace TestDataFramework.TypeGenerator
{
    public class StandardTypeGenerator : ITypeGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardTypeGenerator));

        #region Fields

        private readonly IValueGenerator valueGenerator;
        private readonly IHandledTypeGenerator handledTypeGenerator;

        private readonly Stack<Type> complexTypeProcessingRecursionGuard = new Stack<Type>();

        #endregion Fields

        public StandardTypeGenerator(IValueGenerator valueGenerator, IHandledTypeGenerator handledTypeGenerator)
        {
            this.valueGenerator = valueGenerator;
            this.handledTypeGenerator = handledTypeGenerator;
        }

        #region Private methods

        private object ConstructObject(Type forType, Action<object> fillObject)
        {
            StandardTypeGenerator.Logger.Debug("Entering ConstructObject");

            if (this.complexTypeProcessingRecursionGuard.Contains(forType))
            {
                StandardTypeGenerator.Logger.Debug("Circular reference encountered. Type: " + forType);

                return null;
            }

            this.complexTypeProcessingRecursionGuard.Push(forType);

            ConstructorInfo defaultConstructor = forType.GetConstructor(Type.EmptyTypes);

            if (defaultConstructor == null)
            {
                this.complexTypeProcessingRecursionGuard.Pop();

                StandardTypeGenerator.Logger.Debug("Type has no public default constructor. Type: " + forType);

                return null;
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
            object targetPropertyValue = this.valueGenerator.GetValue(targetPropertyInfo);
            targetPropertyInfo.SetValue(objectToFill, targetPropertyValue);
        }

        #endregion Private methods

        #region Public methods

        public virtual object GetObject<T>(ConcurrentDictionary<PropertyInfo, Action<T>> explicitProperySetters)
        {
            object result = this.ConstructObject(typeof (T), objectToFill => this.FillObject((T)objectToFill, explicitProperySetters));

            if (result == null)
            {
                return typeof(T).IsValueType
                    ? Activator.CreateInstance(typeof(T))
                    : null;
            }            

            StandardTypeGenerator.Logger.Debug("Exiting GetObject<T>");
            return result;
        }

        public virtual object GetObject(Type forType)
        {
            StandardTypeGenerator.Logger.Debug("Entering GetObject");

            object result = this.ConstructObject(forType, this.FillObject);

            if (result == null)
            {
                return forType.IsValueType
                    ? Activator.CreateInstance(forType)
                    : null;
            }

            StandardTypeGenerator.Logger.Debug("Exiting GetObject");
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
                    setter(objectToFill);
                }
                else
                {
                    this.SetProperty(objectToFill, targetPropertyInfo);
                }
            }

            StandardTypeGenerator.Logger.Debug("Exiting FillObject<T>");
        }

        #endregion Protected methods
    }
}
