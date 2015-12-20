using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Randomizer;

namespace TestDataFramework.ValueGenerator
{
    public class StandardValueGenerator : IValueGenerator
    {
        private readonly IRandomizer randomizer;

        private delegate object GetValueForTypeDelegate(Type type);

        private readonly Dictionary<Type, GetValueForTypeDelegate> typeValueGetterDictionary;

        public StandardValueGenerator(IRandomizer randomizer)
        {
            this.typeValueGetterDictionary = new Dictionary<Type, GetValueForTypeDelegate>
            {
                { typeof(int), this.GetInteger },
                { typeof(long), this.GetLongInteger },
                { typeof(short), this.GetShortInteger },
            };

            this.randomizer = randomizer;
        }

        public object GetValue(Type forType)
        {
            GetValueForTypeDelegate getter;

            if (!this.typeValueGetterDictionary.TryGetValue(forType, out getter))
            {
                throw new UnknownValueGeneratorTypeException(forType);
            }

            object result = getter(forType);

            return result;
        }

        #region Private Methods

        private object GetInteger(Type type)
        {
            int result = this.randomizer.RandomizeInteger();
            return result;
        }

        private object GetLongInteger(Type type)
        {
            long result = this.randomizer.RandomizeLongInteger();
            return result;
        }

        private object GetShortInteger(Type type)
        {
            short result = this.randomizer.RandomizeShortInteger();
            return result;
        }

        #endregion Private Methods
    }
}
