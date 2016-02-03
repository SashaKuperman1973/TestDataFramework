using System;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.UniqueValueGenerator.Interface;

namespace TestDataFramework.UniqueValueGenerator
{
    public abstract class BaseUniqueValueGenerator : IUniqueValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseUniqueValueGenerator));

        private readonly IPropertyValueAccumulator accumulator;
        private readonly IDeferredValueGenerator<LargeInteger> deferredValueGenerator;
        private readonly bool throwIfUnhandledType;


        protected BaseUniqueValueGenerator(IPropertyValueAccumulator accumulator, IDeferredValueGenerator<LargeInteger> deferredValueGenerator, bool throwIfUnhandledType)
        {
            this.accumulator = accumulator;
            this.deferredValueGenerator = deferredValueGenerator;
            this.throwIfUnhandledType = throwIfUnhandledType;
        }

        public virtual object GetValue(PropertyInfo propertyInfo)
        {
            BaseUniqueValueGenerator.Logger.Debug("Entering GetValue");

            this.UnhandledTypeCheck(propertyInfo.PropertyType);

            object result = this.accumulator.GetValue(propertyInfo, Helper.DefaultInitalCount);

            BaseUniqueValueGenerator.Logger.Debug("Exiting GetValue");
            return result;
        }

        protected virtual void DeferValue(PropertyInfo propertyInfo)
        {
            BaseUniqueValueGenerator.Logger.Debug("Entering DeferValue");

            this.UnhandledTypeCheck(propertyInfo.PropertyType);

            this.deferredValueGenerator.AddDelegate(propertyInfo,

                initialCount => this.accumulator.GetValue(propertyInfo, initialCount)

                );

            BaseUniqueValueGenerator.Logger.Debug("Exiting DeferValue");
        }

        private void UnhandledTypeCheck(Type type)
        {
            if (this.throwIfUnhandledType && !this.accumulator.IsTypeHandled(type))
            {
                throw new UnHandledTypeException(Messages.UnhandledUniqueKeyType, type);
            }
        }
    }
}
