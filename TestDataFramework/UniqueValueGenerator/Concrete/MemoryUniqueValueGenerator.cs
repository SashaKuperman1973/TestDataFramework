using System;
using System.CodeDom;
using System.Reflection;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator.Concrete
{
    public class MemoryUniqueValueGenerator : BaseUniqueValueGenerator
    {
        public MemoryUniqueValueGenerator(IPropertyValueAccumulator accumulator,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator, bool throwIfUnhandledType)
            : base(accumulator, deferredValueGenerator, throwIfUnhandledType)
        {
        }

        public override object GetValue(PropertyInfo propertyInfo)
        {
            var primaryKeyAttribute = propertyInfo.GetSingleAttribute<PrimaryKeyAttribute>();

            if (propertyInfo.PropertyType == typeof (Guid))
            {
                return Guid.NewGuid();
            }

            if (primaryKeyAttribute == null)
            {
                object result = base.GetValue(propertyInfo);
                return result ?? Helper.GetDefaultValue(propertyInfo.PropertyType);
            }

            if (primaryKeyAttribute.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Manual
                || primaryKeyAttribute.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Auto)
            {
                this.DeferValue(propertyInfo);
            }

            return Helper.GetDefaultValue(propertyInfo.PropertyType);
        }
    }
}