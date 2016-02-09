using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator.Concrete
{
    public class KeyTypeUniqueValueGenerator : BaseUniqueValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (KeyTypeUniqueValueGenerator));

        public KeyTypeUniqueValueGenerator(IPropertyValueAccumulator accumulator,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator, bool throwIfUnhandledType)
            : base(accumulator, deferredValueGenerator, throwIfUnhandledType)
        {
        }

        public override object GetValue(PropertyInfo propertyInfo)
        {
            KeyTypeUniqueValueGenerator.Logger.Debug(
                $"Entering GetValue. propertyInfo: {propertyInfo.GetExtendedMemberInfoString()}");

            var primaryKeyAttribute = propertyInfo.GetSingleAttribute<PrimaryKeyAttribute>();

            if (primaryKeyAttribute == null)
            {
                KeyTypeUniqueValueGenerator.Logger.Debug("primaryKeyAttribute == null");

                object result = base.GetValue(propertyInfo);

                KeyTypeUniqueValueGenerator.Logger.Debug($"result fom base: {result}");
                return result ?? Helper.GetDefaultValue(propertyInfo.PropertyType);
            }

            KeyTypeUniqueValueGenerator.Logger.Debug($"primaryKeyAttribute.KeyType: {primaryKeyAttribute.KeyType}");

            if (propertyInfo.GetSingleAttribute<ForeignKeyAttribute>() == null
                && primaryKeyAttribute.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Manual &&
                new[]
                {
                    typeof (byte), typeof (int), typeof (short), typeof (long), typeof (string),
                    typeof (uint), typeof (ushort), typeof (ulong),

                }.Contains(propertyInfo.PropertyType))
            {
                KeyTypeUniqueValueGenerator.Logger.Debug("Deferring value");
                this.DeferValue(propertyInfo);
            }

            return Helper.GetDefaultValue(propertyInfo.PropertyType);
        }
    }
}