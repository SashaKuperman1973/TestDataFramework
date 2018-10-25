/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.UniqueValueGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;
using TestDataFramework.ValueProvider.Interfaces;

namespace TestDataFramework.ValueGenerator
{
    public abstract class BaseValueGenerator : IValueGenerator
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(BaseValueGenerator));
        private readonly IAttributeDecorator attributeDecorator;
        private readonly Func<IArrayRandomizer> getArrayRandomizer;
        private readonly Func<ITypeGenerator> getTypeGenerator;

        private readonly Dictionary<Type, GetValueForTypeDelegate> typeValueGetterDictionary;
        private readonly IUniqueValueGenerator uniqueValueGenerator;

        protected readonly IValueProvider ValueProvider;

        protected BaseValueGenerator(IValueProvider valueProvider, Func<ITypeGenerator> getTypeGenerator,
            Func<IArrayRandomizer> getArrayRandomizer, IUniqueValueGenerator uniqueValueGenerator,
            IAttributeDecorator attributeDecorator)
        {
            BaseValueGenerator.Logger.Debug("Entering constructor");

            this.ValueProvider = valueProvider;
            this.getTypeGenerator = getTypeGenerator;
            this.getArrayRandomizer = getArrayRandomizer;
            this.uniqueValueGenerator = uniqueValueGenerator;
            this.attributeDecorator = attributeDecorator;

            this.typeValueGetterDictionary = new Dictionary<Type, GetValueForTypeDelegate>
            {
                {typeof(EmailAttribute), x => this.ValueProvider.GetEmailAddress()},
                {typeof(PrimaryKeyAttribute), this.GetPrimaryKey},
                {typeof(string), this.GetString},
                {typeof(decimal), this.GetDecimal},
                {typeof(int), this.GetInteger},
                {typeof(uint), this.GetUnsignedInteger},
                {typeof(long), this.GetLong},
                {typeof(ulong), this.GetUnsignedLong},
                {typeof(short), this.GetShort},
                {typeof(ushort), this.GetUnsignedShort},
                {typeof(bool), x => this.ValueProvider.GetBoolean()},
                {typeof(char), x => this.ValueProvider.GetCharacter()},
                {typeof(DateTime), this.GetDateTime},
                {typeof(byte), x => this.ValueProvider.GetByte()},
                {typeof(double), this.GetDouble},
                {typeof(float), this.GetFloat},
                {typeof(Guid), this.GetGuid}
            };

            BaseValueGenerator.Logger.Debug("Exiting constructor");
        }

        // This is the general entry point.
        public virtual object GetValue(PropertyInfo propertyInfo, ObjectGraphNode objectGraphNode, TypeGeneratorContext context)
        {
            BaseValueGenerator.Logger.Debug(
                $"Entering GetValue(PropertyInfo, ObjectGraphNode). propertyInfo: {propertyInfo}");

            propertyInfo.IsNotNull(nameof(propertyInfo));

            GetValueForTypeDelegate getter = null;

            this.attributeDecorator.GetCustomAttributes(propertyInfo)
                .Any(
                    attribute =>
                        this.typeValueGetterDictionary.TryGetValue(attribute.GetType(), out getter));

            object result = getter != null
                ? getter(propertyInfo)
                : this.GetValue(
                    propertyInfo,
                    propertyInfo.PropertyType,
                    forType => this.getTypeGenerator().GetObject(forType, objectGraphNode, context), context);

            BaseValueGenerator.Logger.Debug($"Exiting GetValue(PropertyInfo, ObjectGraphNode). result: {result?.GetType()}");
            return result;
        }

        // This entry point is used when a different type is requested for a particular 
        // PropertyInfo or property info doesn't exist in the calling context.
        public virtual object GetValue(PropertyInfo propertyInfo, Type type, TypeGeneratorContext context)
        {
            return this.GetValue(propertyInfo, type, forType => this.getTypeGenerator().GetObject(forType, null, context), context);
        }

        public virtual object GetIntrinsicValue(PropertyInfo propertyInfo, Type type, TypeGeneratorContext typeGeneratorContext)
        {
            return this.GetValue(propertyInfo, type, forType => null, typeGeneratorContext);
        }

        private delegate object GetValueForTypeDelegate(PropertyInfo propertyInfo);

        #region Private Methods

        private object GetValue(PropertyInfo propertyInfo, Type type, Func<Type, object> nonIntrinsicTypeGenerator,
            TypeGeneratorContext typeGeneratorContext)
        {
            BaseValueGenerator.Logger.Debug(
                $"Entering GetValue(PropertyInfo, Type, ObjectGraphNode). propertyInfo: {propertyInfo}, type: {type}");

            type.IsNotNull(nameof(type));

            if (type.IsArray)
                return this.getArrayRandomizer().GetArray(propertyInfo, type, typeGeneratorContext);

            Type forType = Nullable.GetUnderlyingType(type) ?? type;

            object result;

            if (typeof(Enum).IsAssignableFrom(forType))
            {
                result = this.GetEnum(forType);
                return result;
            }

            result =
                this.typeValueGetterDictionary.TryGetValue(forType, out GetValueForTypeDelegate getter)
                    ? getter(propertyInfo)
                    : nonIntrinsicTypeGenerator(forType);

            BaseValueGenerator.Logger.Debug($"Exiting GetValue(PropertyInfo, Type). result: {result?.GetType()}");
            return result;
        }

        protected abstract object GetGuid(PropertyInfo propertyInfo);

        protected virtual object GetDateTime(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetDateTime");

            PastOrFutureAttribute pastOrFutureAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<PastOrFutureAttribute>(propertyInfo)
                : null;

            PastOrFuture? pastOrFuture = pastOrFutureAttribute?.PastOrFuture;

            MaxAttribute maxAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<MaxAttribute>(propertyInfo)
                : null;

            long? max = maxAttribute?.Max;

            MinAttribute minAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<MinAttribute>(propertyInfo)
                : null;

            long? min = minAttribute?.Min;

            DateTime result = this.ValueProvider.GetDateTime(pastOrFuture, this.ValueProvider.GetLongInteger, min, max);

            BaseValueGenerator.Logger.Debug("Exiting GetDateTime");
            return result;
        }

        private object GetString(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetString");

            StringLengthAttribute lengthAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<StringLengthAttribute>(propertyInfo)
                : null;

            int? length = lengthAttribute?.Length;

            string result = this.ValueProvider.GetString(length);

            BaseValueGenerator.Logger.Debug("Exiting GetString");
            return result;
        }

        private object GetDecimal(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetDecimal");

            this.GetRealPropertyValues(propertyInfo, out int? precision, out decimal? min, out decimal? max);

            decimal result = this.ValueProvider.GetDecimal(precision, min, max);

            BaseValueGenerator.Logger.Debug("Exiting GetDecimal");
            return result;
        }

        private object GetDouble(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetDouble");

            this.GetRealPropertyValues(propertyInfo, out int? precision, out decimal? min, out decimal? max);

            double result = this.ValueProvider.GetDouble(precision, min, max);

            BaseValueGenerator.Logger.Debug("Exiting GetDouble");
            return result;
        }

        private object GetFloat(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetFloat");

            this.GetRealPropertyValues(propertyInfo, out int? precision, out decimal? min, out decimal? max);

            float result = this.ValueProvider.GetFloat(precision, min, max);

            BaseValueGenerator.Logger.Debug("Exiting GetFloat");
            return result;
        }

        private void GetRealPropertyValues(PropertyInfo propertyInfo, out int? precision, out decimal? min, out decimal? max)
        {
            precision = null;
            min = null;
            max = null;

            if (propertyInfo == null) return;

            var precisionAttribute =
                this.attributeDecorator.GetCustomAttribute<PrecisionAttribute>(propertyInfo);

            precision = precisionAttribute?.Precision;

            var maxAttribute =
                this.attributeDecorator.GetCustomAttribute<MaxAttribute>(propertyInfo);

            max = maxAttribute?.MaxReal;

            var minAttribute =
                this.attributeDecorator.GetCustomAttribute<MinAttribute>(propertyInfo);

            min = minAttribute?.RealMin;
        }

        private object GetInteger(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetInteger");

            this.GetIntegerPropertyValues(propertyInfo, out long? min, out long? max);

            int result = this.ValueProvider.GetInteger((int?) min, (int?)max);

            BaseValueGenerator.Logger.Debug("Exiting GetInteger");
            return result;
        }

        private object GetUnsignedInteger(PropertyInfo propertyInfo)
        {
            object integer = this.GetInteger(propertyInfo);
            uint result = Convert.ToUInt32(integer);
            return result;
        }

        private object GetLong(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetLong");

            this.GetIntegerPropertyValues(propertyInfo, out long? min, out long? max);

            long result = this.ValueProvider.GetLongInteger(min, max);

            BaseValueGenerator.Logger.Debug("Exiting GetLong");
            return result;
        }

        private object GetUnsignedLong(PropertyInfo propertyInfo)
        {
            object integer = this.GetLong(propertyInfo);
            ulong result = Convert.ToUInt64(integer);
            return result;
        }

        private object GetShort(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetShort");

            this.GetIntegerPropertyValues(propertyInfo, out long? min, out long? max);

            short result = this.ValueProvider.GetShortInteger((short?) min, (short?)max);

            BaseValueGenerator.Logger.Debug("Exiting GetShort");
            return result;
        }

        private object GetUnsignedShort(PropertyInfo propertyInfo)
        {
            object integer = this.GetShort(propertyInfo);
            ushort result = Convert.ToUInt16(integer);
            return result;
        }

        private void GetIntegerPropertyValues(PropertyInfo propertyInfo, out long? min, out long? max)
        {
            min = null;
            max = null;

            if (propertyInfo == null) return;

            var maxAttribute =
                this.attributeDecorator.GetCustomAttribute<MaxAttribute>(propertyInfo);

            max = maxAttribute?.Max;

            var minAttribute =
                this.attributeDecorator.GetCustomAttribute<MinAttribute>(propertyInfo);

            min = minAttribute?.Min;

            if (max < 0)
                throw new ArgumentOutOfRangeException(Messages.MaxAttributeLessThanZero, (Exception)null);

            if (max > int.MaxValue)
                throw new ArgumentOutOfRangeException(string.Format(Messages.MaxAttributeOutOfRange, "int"),
                    (Exception)null);

            if (min < 0)
                throw new ArgumentOutOfRangeException(Messages.MinAttributeLessThanZero, (Exception)null);

            if (min > int.MaxValue)
                throw new ArgumentOutOfRangeException(string.Format(Messages.MinAttributeOutOfRange, "int"),
                    (Exception)null);
        }

        private object GetPrimaryKey(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetPrimaryKey");

            object result = this.uniqueValueGenerator.GetValue(propertyInfo);

            BaseValueGenerator.Logger.Debug("Exiting GetPrimaryKey");
            return result;
        }

        private object GetEnum(Type forType)
        {
            object result = this.ValueProvider.GetEnum(forType);
            return result;
        }

        #endregion Private Methods
    }
}