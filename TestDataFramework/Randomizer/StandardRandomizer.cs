using System;
using System.Threading;
using log4net;
using TestDataFramework.Helpers.Concrete;
using TestDataFramework.Helpers.Interfaces;

namespace TestDataFramework.Randomizer
{
    public class StandardRandomizer : IRandomizer
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RandomSymbolStringGenerator));

        private readonly Random random;
        private readonly IRandomSymbolStringGenerator stringRandomizer;
        private readonly Func<DateTime> dateProvider;

        public StandardRandomizer(Random random, IRandomSymbolStringGenerator stringRandomizer, Func<DateTime> dateProvider)
        {
            StandardRandomizer.Logger.Debug("Entering constructor");

            this.random = random;
            this.stringRandomizer = stringRandomizer;
            this.dateProvider = dateProvider;

            StandardRandomizer.Logger.Debug("Exiting constructor");
        }

        public int RandomizeInteger(int? max)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeInteger");

            max = max ?? int.MaxValue;

            StandardRandomizer.Logger.Debug("max = " + max);

            int result = this.random.Next(max.Value);

            StandardRandomizer.Logger.Debug("Exiting RandomizeInteger");
            return result;
        }

        public long RandomizeLongInteger(long? max)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeLongInteger");

            long workingMax = max ?? long.MaxValue;

            StandardRandomizer.Logger.Debug("workingMax = " + workingMax);

            const int wordMask = 0xffff;

            var lowerMaxWord = (int) (workingMax & wordMask);
            if (lowerMaxWord == 0)
            {
                lowerMaxWord = 0x10000;
            }

            long result = this.random.Next(lowerMaxWord);

            for (var i = 0; i < 3; i++)
            {
                workingMax >>= 16;
                int maxRandom = (int)(workingMax & wordMask) + 1;
                long randomValue = this.random.Next(maxRandom);
                randomValue <<= 16 * (i + 1);
                result |= randomValue;
            }

            StandardRandomizer.Logger.Debug("Exiting RandomizeLongInteger");
            return result;
        }

        public short RandomizeShortInteger(short? max)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeShortInteger");

            max = max ?? short.MaxValue;

            StandardRandomizer.Logger.Debug("max = " + max);

            int result = this.random.Next(max.Value);

            StandardRandomizer.Logger.Debug("Exiting RandomizeShortInteger");
            return (short)result;
        }

        public string RandomizeString(int? length)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeString");

            string result = this.stringRandomizer.GetRandomString(length);

            StandardRandomizer.Logger.Debug("Exiting RandomizeString");
            return result;
        }

        public char RandomizeCharacter()
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeCharacter");

            int letterCode = this.random.Next(26);

            var result = (char)(letterCode + 65);

            StandardRandomizer.Logger.Debug("Exiting RandomizeCharacter");
            return result;
        }

        public decimal RandomizeDecimal(int? precision)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeDecimal");

            precision = precision ?? 2;

            StandardRandomizer.Logger.Debug("precision = " + precision);

            var result = (decimal) this.GetReal(precision.Value);

            StandardRandomizer.Logger.Debug("Exiting RandomizeDecimal");
            return result;
        }

        public bool RandomizeBoolean()
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeBoolean");

            int value = this.random.Next(2);

            bool result = value == 1;

            StandardRandomizer.Logger.Debug("Exiting RandomizeBoolean");
            return result;
        }

        public DateTime RandomizeDateTime(PastOrFuture? pastOrFuture)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeBoolean");

            pastOrFuture = pastOrFuture ?? PastOrFuture.Past;

            StandardRandomizer.Logger.Debug("pastOrFuture = " + pastOrFuture);

            int ticks = this.random.Next();

            ticks = pastOrFuture == PastOrFuture.Future ? ticks : -ticks;

            DateTime result = this.dateProvider().AddTicks(ticks);

            StandardRandomizer.Logger.Debug("Exiting RandomizeBoolean");
            return result;
        }

        public byte RandomizeByte()
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeByte");

            StandardRandomizer.Logger.Debug("Exiting RandomizeByte");

            throw new NotImplementedException();
        }

        public double RandomizeDouble(int? precision)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeBoolean");

            precision = precision ?? 2;

            StandardRandomizer.Logger.Debug("precision = " + precision);

            double result = this.GetReal(precision.Value);

            StandardRandomizer.Logger.Debug("Exiting RandomizeBoolean");
            return result;
        }

        public object RandomizeEmailAddress()
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeEmailAddress");

            StandardRandomizer.Logger.Debug("Exiting RandomizeEmailAddress");

            throw new NotImplementedException();
        }

        private double GetReal(int precision)
        {
            int wholePart = this.random.Next();
            double decimalPart = this.random.NextDouble();
            double result = wholePart + decimalPart;
            result = Math.Round(result, precision);

            return result;
        }
    };
}