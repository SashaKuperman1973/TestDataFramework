using System;
using System.Linq;
using System.Reflection;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator.Concrete
{
    public class KeyTypeUniqueValueGenerator : BaseUniqueValueGenerator
    {
        public KeyTypeUniqueValueGenerator(IPropertyValueAccumulator accumulator,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator, bool throwIfUnhandledType)
            : base(accumulator, deferredValueGenerator, throwIfUnhandledType)
        {
        }

        public override object GetValue(PropertyInfo propertyInfo)
        {
            var primaryKeyAttribute = propertyInfo.GetSingleAttribute<PrimaryKeyAttribute>();

            if (primaryKeyAttribute == null)
            {
                object result = base.GetValue(propertyInfo);
                return result ?? Helper.GetDefaultValue(propertyInfo.PropertyType);
            }

            if (propertyInfo.GetSingleAttribute<ForeignKeyAttribute>() == null
                && primaryKeyAttribute.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Manual &&
                new[]
                {
                    typeof (byte), typeof (int), typeof (short), typeof (long), typeof (string),
                    typeof (uint), typeof (ushort), typeof (ulong),

                }.Contains(propertyInfo.PropertyType))
            {
                this.DeferValue(propertyInfo);
            }

            return Helper.GetDefaultValue(propertyInfo.PropertyType);
        }
    }
}