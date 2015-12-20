using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using TestDataFramework.Populator;
using TestDataFramework.Randomizer;

namespace TestDataFramework.ValueGenerator
{
    public class StandardValueGenerator : IValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardValueGenerator));

        private readonly IRandomizer randomizer;

        private delegate object GetValueForTypeDelegate(PropertyInfo type);

        private readonly Dictionary<Type, GetValueForTypeDelegate> typeValueGetterDictionary;

        public StandardValueGenerator(IRandomizer randomizer)
        {
            this.typeValueGetterDictionary = new Dictionary<Type, GetValueForTypeDelegate>
            {
                { typeof(int), this.GetInteger },
                { typeof(long), this.GetLongInteger },
                { typeof(short), this.GetShortInteger },
                { typeof(string), this.GetString },
                { typeof(char), this.GetChar },
                { typeof(decimal), this.GetDecimal },
            };

            this.randomizer = randomizer;
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

        private object GetInteger(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetInteger");

            int result = this.randomizer.RandomizeInteger();

            StandardValueGenerator.Logger.Debug("Exiting GetInteger");
            return result;
        }

        private object GetLongInteger(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetLongInteger");

            long result = this.randomizer.RandomizeLongInteger();

            StandardValueGenerator.Logger.Debug("Exiting GetLongInteger");
            return result;
        }

        private object GetShortInteger(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetShortInteger");

            short result = this.randomizer.RandomizeShortInteger();

            StandardValueGenerator.Logger.Debug("Exiting GetShortInteger");
            return result;
        }

        private object GetString(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetString");

            var lengthAttribute = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
            int? length = lengthAttribute?.Length;

            string result = this.randomizer.RandomizeString(length);

            StandardValueGenerator.Logger.Debug("Exiting GetString");
            return result;
        }

        private object GetChar(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetChar");

            char result = this.randomizer.RandomizeCharacter();

            StandardValueGenerator.Logger.Debug("Exiting GetChar");
            return result;
        }

        private object GetDecimal(PropertyInfo propertyInfo)
        {
            StandardValueGenerator.Logger.Debug("Entering GetChar");

            var precisionAttribute = propertyInfo.GetCustomAttribute<PrecisionAttribute>();
            int? precision = precisionAttribute?.Precision;

            decimal result = this.randomizer.RandomizeDecimal(precision);

            StandardValueGenerator.Logger.Debug("Exiting GetChar");
            return result;
        }

        #endregion Private Methods
    }
}
