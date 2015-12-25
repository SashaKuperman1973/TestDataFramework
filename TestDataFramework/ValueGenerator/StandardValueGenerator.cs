using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator;
using TestDataFramework.Randomizer;
using TestDataFramework.TypeGenerator;

namespace TestDataFramework.ValueGenerator
{
    public class StandardValueGenerator : IValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardValueGenerator));

        private readonly IRandomizer randomizer;

        private readonly ITypeGenerator typeGenerator;

        private delegate object GetValueForTypeDelegate(PropertyInfo propertyInfo);

        private readonly Dictionary<Type, GetValueForTypeDelegate> typeValueGetterDictionary;

        public StandardValueGenerator(IRandomizer randomizer, ITypeGenerator typeGenerator)
        {
            this.randomizer = randomizer;
            this.typeGenerator = typeGenerator;

            this.typeValueGetterDictionary = new Dictionary<Type, GetValueForTypeDelegate>
            {
                {typeof (EmailAttribute), x => this.randomizer.RandomizeEmailAddress()},
                {typeof (string), this.GetString},
                {typeof (decimal), this.GetDecimal},
                {typeof (int), this.GetInteger},
                {typeof (long), x => this.randomizer.RandomizeLongInteger()},
                {typeof (short), x => this.randomizer.RandomizeShortInteger()},
                {typeof (bool), x => this.randomizer.RandomizeBoolean()},
                {typeof (char), x => this.randomizer.RandomizeCharacter()},
                {typeof (DateTime), x => this.randomizer.RandomizeDateTime()},
                {typeof (byte), x => this.randomizer.RandomizeByte()},
                {typeof (double), this.GetDouble},
            };
        }

        public object GetValue(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetValue");

            Assert.IsNotNull(propertyInfo, "propertyInfo argument");

            GetValueForTypeDelegate getter;

            object result = this.TryGetGetter(propertyInfo, out getter)
                ? getter(propertyInfo)
                : this.typeGenerator.GetObject(propertyInfo.PropertyType);

            StandardValueGenerator.Logger.Debug("Exiting GetValue");
            return result;
        }

        private bool TryGetGetter(PropertyInfo propertyInfo, out GetValueForTypeDelegate getValueForTypeDelegate)
        {
            StandardValueGenerator.Logger.Debug("Entering TryGetGetter");

            GetValueForTypeDelegate tempGetter = null;

            if (
                propertyInfo.GetCustomAttributesData()
                    .Any(
                        attributeData =>
                            this.typeValueGetterDictionary.TryGetValue(attributeData.AttributeType, out tempGetter)))
            {
                getValueForTypeDelegate = tempGetter;
                return true;
            }

            Type forType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            bool result = this.typeValueGetterDictionary.TryGetValue(forType, out getValueForTypeDelegate);

            StandardValueGenerator.Logger.Debug("Exiting TryGetGetter");
            return result;
        }

        #region Private Methods

        private object GetString(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetString");

            var lengthAttribute = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
            int? length = lengthAttribute?.Length;

            string result = this.randomizer.RandomizeString(length);

            StandardValueGenerator.Logger.Debug("Exiting GetString");
            return result;
        }

        private object GetDecimal(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetDecimal");

            var precisionAttribute = propertyInfo.GetCustomAttribute<PrecisionAttribute>();
            int? precision = precisionAttribute?.Precision;

            decimal result = this.randomizer.RandomizeDecimal(precision);

            StandardValueGenerator.Logger.Debug("Exiting GetDecimal");
            return result;
        }

        private object GetDouble(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetDouble");

            var precisionAttribute = propertyInfo.GetCustomAttribute<PrecisionAttribute>();
            int? precision = precisionAttribute?.Precision;

            double result = this.randomizer.RandomizeDouble(precision);

            StandardValueGenerator.Logger.Debug("Exiting GetDouble");
            return result;
        }

        private object GetInteger(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetInteger");

            var maxAttribute = propertyInfo.GetCustomAttribute<MaxAttribute>();
            int? max = maxAttribute?.Max;

            int result = this.randomizer.RandomizeInteger(max);

            StandardValueGenerator.Logger.Debug("Exiting GetInteger");
            return result;
        }

        #endregion Private Methods
    }
}
