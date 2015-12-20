using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator;
using TestDataFramework.Randomizer;

namespace TestDataFramework.ValueGenerator
{
    public class StandardValueGenerator : IValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardValueGenerator));

        private readonly IRandomizer randomizer;

        private delegate object GetValueForTypeDelegate(PropertyInfo propertyInfo);

        private readonly Dictionary<Type, GetValueForTypeDelegate> typeValueGetterDictionary;

        public StandardValueGenerator(IRandomizer randomizer)
        {
            this.randomizer = randomizer;

            this.typeValueGetterDictionary = new Dictionary<Type, GetValueForTypeDelegate>
            {
                {typeof (string), this.GetString},
                {typeof (decimal), this.GetDecimal},
                {typeof (int), x => this.randomizer.RandomizeInteger()},
                {typeof (long), x => this.randomizer.RandomizeLongInteger()},
                {typeof (short), x => this.randomizer.RandomizeShortInteger()},
                {typeof (bool), x => this.randomizer.RandomizeBoolean()},
                {typeof (char), x => this.randomizer.RandomizeCharacter()},
            };
        }

        public object GetValue(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetValue");

            Type forType = propertyInfo.PropertyType;

            GetValueForTypeDelegate getter;

            if (!this.typeValueGetterDictionary.TryGetValue(forType, out getter))
            {
                throw new UnknownValueGeneratorTypeException(forType);
            }

            object result = getter(propertyInfo);

            StandardValueGenerator.Logger.Debug("Exiting GetValue");
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

        #endregion Private Methods
    }
}
