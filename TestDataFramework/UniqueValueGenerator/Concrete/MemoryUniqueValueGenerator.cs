using System;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator.Concrete
{
    public class MemoryUniqueValueGenerator : BaseUniqueValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MemoryUniqueValueGenerator));

        public MemoryUniqueValueGenerator(IPropertyValueAccumulator accumulator,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator, bool throwIfUnhandledType)
            : base(accumulator, deferredValueGenerator, throwIfUnhandledType)
        {
        }

        public override object GetValue(PropertyInfo propertyInfo)
        {
            MemoryUniqueValueGenerator.Logger.Debug($"Entering GetValue. propertyInfo: {propertyInfo.GetExtendedMemberInfoString()}");

            if (propertyInfo.PropertyType == typeof (Guid))
            {
                MemoryUniqueValueGenerator.Logger.Debug("Property type is Guid. Returning new Guid.");
                Guid result = Guid.NewGuid();

                MemoryUniqueValueGenerator.Logger.Debug($"result: {result}");
                return result;
            }

            var primaryKeyAttribute = propertyInfo.GetSingleAttribute<PrimaryKeyAttribute>();

            if (primaryKeyAttribute == null)
            {
                MemoryUniqueValueGenerator.Logger.Debug("primaryKeyAttribute == null");
                object result = base.GetValue(propertyInfo);

                MemoryUniqueValueGenerator.Logger.Debug($"result fom base: {result}");
                return result ?? Helper.GetDefaultValue(propertyInfo.PropertyType);
            }

            if (primaryKeyAttribute.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Manual
                || primaryKeyAttribute.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Auto)
            {
                MemoryUniqueValueGenerator.Logger.Debug("Deferring value");
                this.DeferValue(propertyInfo);
            }

            return Helper.GetDefaultValue(propertyInfo.PropertyType);
        }
    }
}