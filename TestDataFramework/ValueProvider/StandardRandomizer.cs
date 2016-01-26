using System;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Helpers.Concrete;
using TestDataFramework.Helpers.Interfaces;

namespace TestDataFramework.ValueProvider
{
    public class StandardRandomizer : IValueProvider
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RandomSymbolStringGenerator));

        private readonly Random random;
        private readonly IRandomSymbolStringGenerator stringRandomizer;
        private readonly DateTimeProvider dateProvider;
        private readonly long dateTimeMinValue;
        private readonly long dateTimeMaxValue;

        public StandardRandomizer(Random random, IRandomSymbolStringGenerator stringRandomizer, DateTimeProvider dateProvider, long dateTimeMinValue, long dateTimeMaxValue)
        {
            StandardRandomizer.Logger.Debug("Entering constructor");

            this.random = random;
            this.stringRandomizer = stringRandomizer;
            this.dateProvider = dateProvider;
            this.dateTimeMinValue = dateTimeMinValue;
            this.dateTimeMaxValue = dateTimeMaxValue;

            StandardRandomizer.Logger.Debug("Exiting constructor");
        }

        public virtual int GetInteger(int? max)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeInteger");

            max = max ?? int.MaxValue;

            StandardRandomizer.Logger.Debug("max = " + max);

            int result = this.random.Next(max.Value);

            StandardRandomizer.Logger.Debug("Exiting RandomizeInteger");
            return result;
        }

        public virtual long GetLongInteger(long? max)
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

        public virtual short GetShortInteger(short? max)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeShortInteger");

            max = max ?? short.MaxValue;

            StandardRandomizer.Logger.Debug("max = " + max);

            int result = this.random.Next(max.Value);

            StandardRandomizer.Logger.Debug("Exiting RandomizeShortInteger");
            return (short)result;
        }

        public virtual string GetString(int? length)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeString");

            string result = this.stringRandomizer.GetRandomString(length);

            StandardRandomizer.Logger.Debug("Exiting RandomizeString");
            return result;
        }

        public virtual char GetCharacter()
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeCharacter");

            int letterCode = this.random.Next(26);

            var result = (char)(letterCode + 65);

            StandardRandomizer.Logger.Debug("Exiting RandomizeCharacter");
            return result;
        }

        public virtual decimal GetDecimal(int? precision)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeDecimal");

            precision = precision ?? 2;

            StandardRandomizer.Logger.Debug("precision = " + precision);

            var result = (decimal) this.GetReal(precision.Value);

            StandardRandomizer.Logger.Debug("Exiting RandomizeDecimal");
            return result;
        }

        public virtual bool GetBoolean()
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeBoolean");

            int value = this.random.Next(2);

            bool result = value == 1;

            StandardRandomizer.Logger.Debug("Exiting RandomizeBoolean");
            return result;
        }

        public virtual DateTime GetDateTime(PastOrFuture? pastOrFuture, Func<long?, long> longIntegerGetter)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeBoolean");

            pastOrFuture = pastOrFuture ?? PastOrFuture.Past;

            StandardRandomizer.Logger.Debug("pastOrFuture = " + pastOrFuture);

            DateTime dateTime = this.dateProvider();
            DateTime result;

            long maxLong, randomLong;
            switch (pastOrFuture.Value)
            {
                case PastOrFuture.Future:
                    maxLong = this.dateTimeMaxValue - dateTime.Ticks;
                    randomLong = longIntegerGetter(maxLong);
                    result = dateTime.AddTicks(randomLong);
                    break;

                case PastOrFuture.Past:
                    maxLong = dateTime.Ticks - this.dateTimeMinValue;
                    randomLong = longIntegerGetter(maxLong);
                    result = dateTime.AddTicks(-randomLong);
                    break;

                default:
                    throw new ArgumentException(Messages.UnknownPastOrFutureEnumValue, nameof(pastOrFuture));
            }

            long ticks = this.GetLongInteger(null);

            ticks = pastOrFuture == PastOrFuture.Future ? ticks : -ticks;

            //DateTime result = this.dateProvider().AddTicks(ticks);

            StandardRandomizer.Logger.Debug("Exiting RandomizeBoolean");
            return result;
        }

        public virtual byte GetByte()
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeByte");

            var array = new byte[1];

            this.random.NextBytes(array);

            byte result = array[0];

            StandardRandomizer.Logger.Debug("Exiting RandomizeByte");
            return result;
        }

        public virtual double GetDouble(int? precision)
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeBoolean");

            precision = precision ?? 2;

            StandardRandomizer.Logger.Debug("precision = " + precision);

            double result = this.GetReal(precision.Value);

            StandardRandomizer.Logger.Debug("Exiting RandomizeBoolean");
            return result;
        }

        public virtual string GetEmailAddress()
        {
            StandardRandomizer.Logger.Debug("Entering RandomizeEmailAddress");

            string namePart = this.stringRandomizer.GetRandomString(10).ToLower();
            string result = namePart + "@domain.com";

            StandardRandomizer.Logger.Debug("Exiting RandomizeEmailAddress");
            return result;
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