using System;
using System.Reflection;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator.Concrete
{
    public class MemoryUniqueValueGenerator : BaseUniqueValueGenerator
    {
        public MemoryUniqueValueGenerator(IPropertyValueAccumulator accumulator, IDeferredValueGenerator<ulong> deferredValueGenerator) : base(accumulator, deferredValueGenerator)
        {
        }

        public override object GetValue(PropertyInfo propertyInfo)
        {
            var primaryKeyAttribute = propertyInfo.GetSingleAttribute<PrimaryKeyAttribute>();

            if (primaryKeyAttribute == null)
            {
                object result = base.GetValue(propertyInfo);
                return result;
            }

            if (primaryKeyAttribute.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Manual
                || primaryKeyAttribute.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Auto)
            {
                this.DeferValue(propertyInfo);
            }

            return propertyInfo.PropertyType.IsValueType
                ? Activator.CreateInstance(propertyInfo.PropertyType)
                : null;
        }
    }
}