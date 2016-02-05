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
            StandardRandomizer.Logger.Debug("Entering GetInteger");

            max = max ?? int.MaxValue;

            StandardRandomizer.Logger.Debug("max = " + max);

            int result = this.random.Next(max.Value);

            StandardRandomizer.Logger.Debug("Exiting GetInteger");
            return result;
        }

        public virtual long GetLongInteger(long? max)
        {
            StandardRandomizer.Logger.Debug("Entering GetLongInteger");

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

            StandardRandomizer.Logger.Debug("Exiting GetLongInteger");
            return result;
        }

        public virtual short GetShortInteger(short? max)
        {
            StandardRandomizer.Logger.Debug("Entering GetShortInteger");

            max = max ?? short.MaxValue;

            StandardRandomizer.Logger.Debug("max = " + max);

            int result = this.random.Next(max.Value);

            StandardRandomizer.Logger.Debug("Exiting GetShortInteger");
            return (short)result;
        }

        public virtual string GetString(int? length)
        {
            StandardRandomizer.Logger.Debug("Entering GetString");

            string result = this.stringRandomizer.GetRandomString(length);

            StandardRandomizer.Logger.Debug("Exiting GetString");
            return result;
        }

        public virtual char GetCharacter()
        {
            StandardRandomizer.Logger.Debug("Entering GetCharacter");

            int letterCode = this.random.Next(26);

            var result = (char)(letterCode + 65);

            StandardRandomizer.Logger.Debug("Exiting GetCharacter");
            return result;
        }

        public virtual decimal GetDecimal(int? precision)
        {
            StandardRandomizer.Logger.Debug("Entering GetDecimal");

            precision = precision ?? 2;

            StandardRandomizer.Logger.Debug("precision = " + precision);

            var result = (decimal) this.GetReal(precision.Value);

            StandardRandomizer.Logger.Debug("Exiting GetDecimal");
            return result;
        }

        public virtual bool GetBoolean()
        {
            StandardRandomizer.Logger.Debug("Entering GetBoolean");

            int value = this.random.Next(2);

            bool result = value == 1;

            StandardRandomizer.Logger.Debug("Exiting GetBoolean");
            return result;
        }

        public virtual DateTime GetDateTime(PastOrFuture? pastOrFuture, Func<long?, long> longIntegerGetter)
        {
            StandardRandomizer.Logger.Debug("Entering GetDateTime");

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

            StandardRandomizer.Logger.Debug("Exiting GetDateTime");
            return result;
        }

        public virtual byte GetByte()
        {
            StandardRandomizer.Logger.Debug("Entering GetByte");

            var array = new byte[1];

            this.random.NextBytes(array);

            byte result = array[0];

            StandardRandomizer.Logger.Debug("Exiting GetByte");
            return result;
        }

        public virtual double GetDouble(int? precision)
        {
            StandardRandomizer.Logger.Debug("Entering GetDouble");

            precision = precision ?? 2;

            StandardRandomizer.Logger.Debug("precision = " + precision);

            double result = this.GetReal(precision.Value);

            StandardRandomizer.Logger.Debug("Exiting GetDouble");
            return result;
        }

        public virtual float GetFloat(int? precision)
        {
            StandardRandomizer.Logger.Debug("Entering GetFloat");

            precision = precision ?? 2;

            StandardRandomizer.Logger.Debug("precision = " + precision);

            if (precision.Value > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(precision), precision.Value, Messages.FloatPrecisionOutOfRange);
            }

            var result = (float)this.GetReal(precision.Value, (int) Math.Pow(10, 7 - precision.Value));

            StandardRandomizer.Logger.Debug("Exiting GetFloat");
            return result;
        }

        public virtual string GetEmailAddress()
        {
            StandardRandomizer.Logger.Debug("Entering GetEmailAddress");

            string namePart = this.stringRandomizer.GetRandomString(10).ToLower();
            string result = namePart + "@domain.com";

            StandardRandomizer.Logger.Debug("Exiting GetEmailAddress");
            return result;
        }

        private double GetReal(int precision, int maxValue = int.MaxValue)
        {
            if (precision < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(precision), precision, Messages.PrecisionMustBeNonNegative);
            }

            int wholePart = this.random.Next(maxValue);
            double decimalPart = this.random.NextDouble();
            double result = wholePart + decimalPart;
            result = Math.Round(result, precision);

            return result;
        }
    };
}