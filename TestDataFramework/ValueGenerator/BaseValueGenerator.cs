using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using log4net;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.TypeGenerator;
using TestDataFramework.UniqueValueGenerator;
using TestDataFramework.UniqueValueGenerator.Interface;
using TestDataFramework.ValueGenerator.Interface;
using TestDataFramework.ValueProvider;

namespace TestDataFramework.ValueGenerator
{
    public abstract class BaseValueGenerator : IValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseValueGenerator));

        private readonly IValueProvider valueProvider;
        private readonly GetTypeGeneratorDelegate getTypeGenerator;
        private readonly Func<IArrayRandomizer> getArrayRandomizer;
        private readonly IUniqueValueGenerator uniqueValueGenerator;

        private delegate object GetValueForTypeDelegate(PropertyInfo propertyInfo);

        public delegate ITypeGenerator GetTypeGeneratorDelegate();

        private readonly Dictionary<Type, GetValueForTypeDelegate> typeValueGetterDictionary;

        protected BaseValueGenerator(IValueProvider valueProvider, GetTypeGeneratorDelegate getTypeGenerator,
            Func<IArrayRandomizer> getArrayRandomizer, IUniqueValueGenerator uniqueValueGenerator)
        {
            BaseValueGenerator.Logger.Debug("Entering constructor");

            this.valueProvider = valueProvider;
            this.getTypeGenerator = getTypeGenerator;
            this.getArrayRandomizer = getArrayRandomizer;
            this.uniqueValueGenerator = uniqueValueGenerator;

            this.typeValueGetterDictionary = new Dictionary<Type, GetValueForTypeDelegate>
            {
                {typeof (EmailAttribute), x => this.valueProvider.GetEmailAddress()},
                {typeof (PrimaryKeyAttribute), this.GetPrimaryKey},
                {typeof (string), this.GetString},
                {typeof (decimal), this.GetDecimal},
                {typeof (int), this.GetInteger},
                {typeof (uint), this.GetInteger},
                {typeof (long), this.GetLong},
                {typeof (ulong), this.GetLong},
                {typeof (short), this.GetShort},
                {typeof (ushort), this.GetShort},
                {typeof (bool), x => this.valueProvider.GetBoolean()},
                {typeof (char), x => this.valueProvider.GetCharacter()},
                {typeof (DateTime), this.GetDateTime},
                {typeof (byte), x => this.valueProvider.GetByte()},
                {typeof (double), this.GetDouble},
                {typeof (float), this.GetFloat},
                {typeof (Guid), this.GetGuid },
            };

            BaseValueGenerator.Logger.Debug("Exiting constructor");
        }

        // This is the general entry point.
        public virtual object GetValue(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetValue(PropertyInfo propertyInfo)");

            propertyInfo.IsNotNull(nameof(propertyInfo));

            GetValueForTypeDelegate getter = null;

            propertyInfo.GetCustomAttributesData()
                .Any(
                    attributeData =>
                        this.typeValueGetterDictionary.TryGetValue(attributeData.AttributeType, out getter));

            object result = getter != null ? getter(propertyInfo) : this.GetValue(propertyInfo, propertyInfo.PropertyType);

            BaseValueGenerator.Logger.Debug("Exiting GetValue(PropertyInfo propertyInfo)");
            return result;
        }

        // This entry point is used when a different type is requested for a particular 
        // PropertyInfo or property info doesn't exist in the calling context.
        public virtual object GetValue(PropertyInfo propertyInfo, Type type)
        {
            BaseValueGenerator.Logger.Debug("Entering GetValue(PropertyInfo propertyInfo, Type type)");

            type.IsNotNull(nameof(type));

            if (type.IsArray)
            {                
                return this.getArrayRandomizer().GetArray(propertyInfo, type);
            }

            Type forType = Nullable.GetUnderlyingType(type) ?? type;

            GetValueForTypeDelegate getter;

            object result = 
                this.typeValueGetterDictionary.TryGetValue(forType, out getter)
                ? getter(propertyInfo)
                : this.getTypeGenerator().GetObject(forType);

            BaseValueGenerator.Logger.Debug("Exiting GetValue(PropertyInfo propertyInfo, Type type)");
            return result;
        }

        #region Private Methods

        protected abstract object GetGuid(PropertyInfo propertyInfo);

        private object GetString(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetString");

            var lengthAttribute = propertyInfo?.GetCustomAttribute<StringLengthAttribute>();
            int? length = lengthAttribute?.Length;

            string result = this.valueProvider.GetString(length);

            BaseValueGenerator.Logger.Debug("Exiting GetString");
            return result;
        }

        private object GetDecimal(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetDecimal");

            var precisionAttribute = propertyInfo?.GetCustomAttribute<PrecisionAttribute>();
            int? precision = precisionAttribute?.Precision;

            decimal result = this.valueProvider.GetDecimal(precision);

            BaseValueGenerator.Logger.Debug("Exiting GetDecimal");
            return result;
        }

        private object GetDouble(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetDouble");

            var precisionAttribute = propertyInfo?.GetCustomAttribute<PrecisionAttribute>();
            int? precision = precisionAttribute?.Precision;

            double result = this.valueProvider.GetDouble(precision);

            BaseValueGenerator.Logger.Debug("Exiting GetDouble");
            return result;
        }

        private object GetFloat(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetFloat");

            var precisionAttribute = propertyInfo?.GetCustomAttribute<PrecisionAttribute>();
            int? precision = precisionAttribute?.Precision;

            float result = this.valueProvider.GetFloat(precision);

            BaseValueGenerator.Logger.Debug("Exiting GetFloat");
            return result;
        }

        private object GetInteger(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetInteger");

            var maxAttribute = propertyInfo?.GetCustomAttribute<MaxAttribute>();
            long? max = maxAttribute?.Max;

            if (max < 0)
            {
                throw new ArgumentOutOfRangeException(Messages.MaxAttributeLessThanZero, (Exception) null);
            }

            if (max > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(string.Format(Messages.MaxAttributeOutOfRange, "int"), (Exception) null);
            }

            int result = this.valueProvider.GetInteger((int?)max);

            BaseValueGenerator.Logger.Debug("Exiting GetInteger");
            return result;
        }

        private object GetLong(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetLong");

            var maxAttribute = propertyInfo?.GetCustomAttribute<MaxAttribute>();
            long? max = maxAttribute?.Max;

            if (max < 0)
            {
                throw new ArgumentOutOfRangeException(Messages.MaxAttributeLessThanZero, (Exception)null);
            }

            long result = this.valueProvider.GetLongInteger(max);

            BaseValueGenerator.Logger.Debug("Exiting GetLong");
            return result;
        }

        private object GetShort(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetShort");

            var maxAttribute = propertyInfo?.GetCustomAttribute<MaxAttribute>();
            long? max = maxAttribute?.Max;

            if (max < 0)
            {
                throw new ArgumentOutOfRangeException(Messages.MaxAttributeLessThanZero, (Exception)null);
            }

            if (max > short.MaxValue)
            {
                throw new ArgumentOutOfRangeException(string.Format(Messages.MaxAttributeOutOfRange, "short"), (Exception)null);
            }

            short result = this.valueProvider.GetShortInteger((short?)max);

            BaseValueGenerator.Logger.Debug("Exiting GetShort");
            return result;
        }

        private object GetDateTime(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetDateTime");

            var pastOrFutureAttribute = propertyInfo?.GetCustomAttribute<PastOrFutureAttribute>();
            PastOrFuture? pastOrFuture = pastOrFutureAttribute?.PastOrFuture;

            DateTime result = this.valueProvider.GetDateTime((PastOrFuture?)pastOrFuture, this.valueProvider.GetLongInteger);

            BaseValueGenerator.Logger.Debug("Exiting GetDateTime");
            return result;
        }

        private object GetPrimaryKey(PropertyInfo propertyInfo)
        {
            object result = this.uniqueValueGenerator.GetValue(propertyInfo);
            return result;
        }

        #endregion Private Methods
    }
}
