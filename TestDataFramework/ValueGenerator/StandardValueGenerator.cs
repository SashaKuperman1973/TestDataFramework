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
using TestDataFramework.ValueProvider;

namespace TestDataFramework.ValueGenerator
{
    public class StandardValueGenerator : IValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardValueGenerator));

        private readonly IValueProvider randomizer;
        private readonly ITypeGenerator typeGenerator;
        private readonly IArrayRandomizer arrayRandomizer;
        private readonly IUniqueValueGenerator uniqueValueGenerator;

        private delegate object GetValueForTypeDelegate(PropertyInfo propertyInfo);

        private readonly Dictionary<Type, GetValueForTypeDelegate> typeValueGetterDictionary;

        public StandardValueGenerator(IValueProvider randomizer, ITypeGenerator typeGenerator,
            Func<IValueGenerator, IArrayRandomizer> getArrayRandomizer, IUniqueValueGenerator uniqueValueGenerator)
            : this(randomizer, x => typeGenerator, getArrayRandomizer, uniqueValueGenerator)
        {
        }

        public StandardValueGenerator(IValueProvider randomizer, Func<IValueGenerator, ITypeGenerator> getTypeGenerator,
            Func<IValueGenerator, IArrayRandomizer> getArrayRandomizer, IUniqueValueGenerator uniqueValueGenerator)
        {
            StandardValueGenerator.Logger.Debug("Entering constructor");

            this.randomizer = randomizer;
            this.typeGenerator = getTypeGenerator(this);
            this.arrayRandomizer = getArrayRandomizer(this);
            this.uniqueValueGenerator = uniqueValueGenerator;

            this.typeValueGetterDictionary = new Dictionary<Type, GetValueForTypeDelegate>
            {
                {typeof (EmailAttribute), x => this.randomizer.GetEmailAddress()},
                {typeof (PrimaryKeyAttribute), this.GetPrimaryKey},
                {typeof (string), this.GetString},
                {typeof (decimal), this.GetDecimal},
                {typeof (int), this.GetInteger},
                {typeof (uint), this.GetInteger},
                {typeof (long), this.GetLong},
                {typeof (ulong), this.GetLong},
                {typeof (short), this.GetShort},
                {typeof (ushort), this.GetShort},
                {typeof (bool), x => this.randomizer.GetBoolean()},
                {typeof (char), x => this.randomizer.GetCharacter()},
                {typeof (DateTime), this.GetDateTime},
                {typeof (byte), x => this.randomizer.GetByte()},
                {typeof (double), this.GetDouble},
                {typeof (Guid), x => StandardValueGenerator.NoOp(typeof(Guid)) },
            };

            StandardValueGenerator.Logger.Debug("Exiting constructor");
        }

        // This is the general entry point.
        public virtual object GetValue(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetValue(PropertyInfo propertyInfo)");

            propertyInfo.IsNotNull(nameof(propertyInfo));

            GetValueForTypeDelegate getter = null;

            propertyInfo.GetCustomAttributesData()
                .Any(
                    attributeData =>
                        this.typeValueGetterDictionary.TryGetValue(attributeData.AttributeType, out getter));

            object result = getter != null ? getter(propertyInfo) : this.GetValue(propertyInfo, propertyInfo.PropertyType);

            StandardValueGenerator.Logger.Debug("Exiting GetValue(PropertyInfo propertyInfo)");
            return result;
        }

        // This entry point is used only when a different type is requested for a particular PropertyInfo
        public virtual object GetValue(PropertyInfo propertyInfo, Type type)
        {
            StandardValueGenerator.Logger.Debug("Entering GetValue(PropertyInfo propertyInfo, Type type)");

            propertyInfo.IsNotNull(nameof(type));

            if (type.IsArray)
            {                
                return this.arrayRandomizer.GetArray(propertyInfo, type);
            }

            Type forType = Nullable.GetUnderlyingType(type) ?? type;

            GetValueForTypeDelegate getter;

            object result = 
                this.typeValueGetterDictionary.TryGetValue(forType, out getter)
                ? getter(propertyInfo)
                : this.typeGenerator.GetObject(forType);

            StandardValueGenerator.Logger.Debug("Exiting GetValue(PropertyInfo propertyInfo, Type type)");
            return result;
        }

        #region Private Methods

        private static object NoOp(Type fortype)
        {
            return fortype.IsValueType
                ? Activator.CreateInstance(fortype)
                : null;
        }

        private object GetString(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetString");

            var lengthAttribute = propertyInfo?.GetCustomAttribute<StringLengthAttribute>();
            int? length = lengthAttribute?.Length;

            string result = this.randomizer.GetString(length);

            StandardValueGenerator.Logger.Debug("Exiting GetString");
            return result;
        }

        private object GetDecimal(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetDecimal");

            var precisionAttribute = propertyInfo?.GetCustomAttribute<PrecisionAttribute>();
            int? precision = precisionAttribute?.Precision;

            decimal result = this.randomizer.GetDecimal(precision);

            StandardValueGenerator.Logger.Debug("Exiting GetDecimal");
            return result;
        }

        private object GetDouble(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetDouble");

            var precisionAttribute = propertyInfo?.GetCustomAttribute<PrecisionAttribute>();
            int? precision = precisionAttribute?.Precision;

            double result = this.randomizer.GetDouble(precision);

            StandardValueGenerator.Logger.Debug("Exiting GetDouble");
            return result;
        }

        private object GetInteger(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetInteger");

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

            int result = this.randomizer.GetInteger((int?)max);

            StandardValueGenerator.Logger.Debug("Exiting GetInteger");
            return result;
        }

        private object GetLong(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetLong");

            var maxAttribute = propertyInfo?.GetCustomAttribute<MaxAttribute>();
            long? max = maxAttribute?.Max;

            if (max < 0)
            {
                throw new ArgumentOutOfRangeException(Messages.MaxAttributeLessThanZero, (Exception)null);
            }

            long result = this.randomizer.GetLongInteger(max);

            StandardValueGenerator.Logger.Debug("Exiting GetLong");
            return result;
        }

        private object GetShort(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetShort");

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

            short result = this.randomizer.GetShortInteger((short?)max);

            StandardValueGenerator.Logger.Debug("Exiting GetShort");
            return result;
        }

        private object GetDateTime(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetDateTime");

            var pastOrFutureAttribute = propertyInfo?.GetCustomAttribute<PastOrFutureAttribute>();
            PastOrFuture? pastOrFuture = pastOrFutureAttribute?.PastOrFuture;

            DateTime result = this.randomizer.GetDateTime((PastOrFuture?)pastOrFuture, this.randomizer.GetLongInteger);

            StandardValueGenerator.Logger.Debug("Exiting GetDateTime");
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
