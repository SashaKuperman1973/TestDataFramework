/*
    Copyright 2016 Alexander Kuperman

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
using System.Runtime.CompilerServices;
using log4net;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.UniqueValueGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;
using TestDataFramework.ValueProvider.Interfaces;

namespace TestDataFramework.ValueGenerator
{
    public abstract class BaseValueGenerator : IValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseValueGenerator));

        private readonly IValueProvider valueProvider;
        private readonly GetTypeGeneratorDelegate getTypeGenerator;
        private readonly Func<IArrayRandomizer> getArrayRandomizer;
        private readonly IUniqueValueGenerator uniqueValueGenerator;
        private readonly IAttributeDecorator attributeDecorator;

        private delegate object GetValueForTypeDelegate(PropertyInfo propertyInfo);

        public delegate ITypeGenerator GetTypeGeneratorDelegate();

        private readonly Dictionary<Type, GetValueForTypeDelegate> typeValueGetterDictionary;

        protected BaseValueGenerator(IValueProvider valueProvider, GetTypeGeneratorDelegate getTypeGenerator,
            Func<IArrayRandomizer> getArrayRandomizer, IUniqueValueGenerator uniqueValueGenerator, IAttributeDecorator attributeDecorator)
        {
            BaseValueGenerator.Logger.Debug("Entering constructor");

            this.valueProvider = valueProvider;
            this.getTypeGenerator = getTypeGenerator;
            this.getArrayRandomizer = getArrayRandomizer;
            this.uniqueValueGenerator = uniqueValueGenerator;
            this.attributeDecorator = attributeDecorator;

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
            BaseValueGenerator.Logger.Debug($"Entering GetValue(PropertyInfo). propertyInfo: {propertyInfo}");

            propertyInfo.IsNotNull(nameof(propertyInfo));

            GetValueForTypeDelegate getter = null;

            this.attributeDecorator.GetCustomAttributes(propertyInfo)
                .Any(
                    attribute =>
                        this.typeValueGetterDictionary.TryGetValue(attribute.GetType(), out getter));

            object result = getter != null ? getter(propertyInfo) : this.GetValue(propertyInfo, propertyInfo.PropertyType);

            BaseValueGenerator.Logger.Debug($"Exiting GetValue(PropertyInfo). result: {result}");
            return result;
        }

        // This entry point is used when a different type is requested for a particular 
        // PropertyInfo or property info doesn't exist in the calling context.
        public virtual object GetValue(PropertyInfo propertyInfo, Type type)
        {
            BaseValueGenerator.Logger.Debug(
                $"Entering GetValue(PropertyInfo, Type). propertyInfo: {propertyInfo}, type: {type}");

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

            BaseValueGenerator.Logger.Debug($"Exiting GetValue(PropertyInfo, Type). result: {result}");
            return result;
        }

        #region Private Methods

        protected abstract object GetGuid(PropertyInfo propertyInfo);

        private object GetString(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetString");

            StringLengthAttribute lengthAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<StringLengthAttribute>(propertyInfo)
                : null;

            int? length = lengthAttribute?.Length;

            string result = this.valueProvider.GetString(length);

            BaseValueGenerator.Logger.Debug("Exiting GetString");
            return result;
        }

        private object GetDecimal(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetDecimal");

            PrecisionAttribute precisionAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<PrecisionAttribute>(propertyInfo)
                : null;

            int? precision = precisionAttribute?.Precision;

            decimal result = this.valueProvider.GetDecimal(precision);

            BaseValueGenerator.Logger.Debug("Exiting GetDecimal");
            return result;
        }

        private object GetDouble(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetDouble");

            PrecisionAttribute precisionAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<PrecisionAttribute>(propertyInfo)
                : null;

            int? precision = precisionAttribute?.Precision;

            double result = this.valueProvider.GetDouble(precision);

            BaseValueGenerator.Logger.Debug("Exiting GetDouble");
            return result;
        }

        private object GetFloat(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetFloat");

            PrecisionAttribute precisionAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<PrecisionAttribute>(propertyInfo)
                : null;

            int? precision = precisionAttribute?.Precision;

            float result = this.valueProvider.GetFloat(precision);

            BaseValueGenerator.Logger.Debug("Exiting GetFloat");
            return result;
        }

        private object GetInteger(PropertyInfo propertyInfo)
        {
            BaseValueGenerator.Logger.Debug("Entering GetInteger");

            MaxAttribute maxAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<MaxAttribute>(propertyInfo)
                : null;

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

            MaxAttribute maxAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<MaxAttribute>(propertyInfo)
                : null;

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

            MaxAttribute maxAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<MaxAttribute>(propertyInfo)
                : null;

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

            PastOrFutureAttribute pastOrFutureAttribute = propertyInfo != null
                ? this.attributeDecorator.GetCustomAttribute<PastOrFutureAttribute>(propertyInfo)
                : null;

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
