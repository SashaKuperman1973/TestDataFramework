using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.ValueGenerator;

namespace TestDataFramework.TypeGenerator
{
    public class StandardTypeGenerator : ITypeGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardTypeGenerator));

        #region Fields

        private readonly IValueGenerator valueGenerator;

        private readonly List<Type> complexTypeProcessingRecursionGuard = new List<Type>();

        #endregion Fields

        public StandardTypeGenerator(Func<ITypeGenerator, IValueGenerator> getValueGenerator)
        {
            this.valueGenerator = getValueGenerator(this);
        }

        #region Private methods

        private object ConstructObject(Type forType)
        {
            StandardTypeGenerator.Logger.Debug("Entering ConstructObject");

            if (this.complexTypeProcessingRecursionGuard.Contains(forType))
            {
                return null;
            }

            this.complexTypeProcessingRecursionGuard.Add(forType);

            ConstructorInfo defaultConstructor = forType.GetConstructor(Type.EmptyTypes);

            if (defaultConstructor == null)
            {
                throw new NoDefaultConstructorException(forType);
            }

            object objectToFill = defaultConstructor.Invoke(null);

            this.FillObject(objectToFill);

            StandardTypeGenerator.Logger.Debug("Exiting ConstructObject");
            return objectToFill;
        }

        private static PropertyInfo[] GetProperties(object objectToFill)
        {
            PropertyInfo[] targetProperties = objectToFill.GetType().GetPropertiesHelper();
            return targetProperties;
        }

        private void SetProperty(object objectToFill, PropertyInfo targetPropertyInfo)
        {
            object targetPropertyValue = this.valueGenerator.GetValue(targetPropertyInfo);
            targetPropertyInfo.SetValue(objectToFill, targetPropertyValue);
        }

        #endregion Private methods

        #region Public methods

        public virtual object GetObject<T>(ConcurrentDictionary<PropertyInfo, Action<T>> explicitProperySetters)
        {
            object objectToFill = this.ConstructObject(typeof (T));

            if (objectToFill == null)
            {
                return null;
            }

            this.FillObject((T)objectToFill, explicitProperySetters);

            StandardTypeGenerator.Logger.Debug("Exiting GetObject<T>");
            return objectToFill;
        }

        public virtual object GetObject(Type forType)
        {
            StandardTypeGenerator.Logger.Debug("Entering GetObject");

            object objectToFill = this.ConstructObject(forType);

            if (objectToFill == null)
            {
                return null;
            }

            this.FillObject(objectToFill);

            StandardTypeGenerator.Logger.Debug("Exiting GetObject");
            return objectToFill;
        }

        public virtual void ResetRecursionGuard()
        {
            this.complexTypeProcessingRecursionGuard.Clear();
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
