using System;
using System.Linq;
using System.Reflection;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator.Concrete
{
    public class KeyTypeUniqueValueGenerator : BaseUniqueValueGenerator
    {
        public KeyTypeUniqueValueGenerator(IPropertyValueAccumulator accumulator,
            IDeferredValueGenerator<ulong> deferredValueGenerator) : base(accumulator, deferredValueGenerator)
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

            if (primaryKeyAttribute.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Manual &&
                new[] {typeof (byte), typeof (int), typeof (short), typeof (long), typeof(string)}.Contains(propertyInfo.PropertyType))
            {
                this.DeferValue(propertyInfo);
            }

            return propertyInfo.PropertyType.IsValueType
                ? Activator.CreateInstance(propertyInfo.PropertyType)
                : null;
        }
    }
}