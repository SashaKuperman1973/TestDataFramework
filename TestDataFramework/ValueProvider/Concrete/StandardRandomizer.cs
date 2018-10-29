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
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Helpers.Concrete;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.Logger;
using TestDataFramework.ValueProvider.Interfaces;

namespace TestDataFramework.ValueProvider.Concrete
{
    public class StandardRandomizer : IValueProvider
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(RandomSymbolStringGenerator));
        private readonly DateTimeProvider dateProvider;
        private readonly long dateTimeMaxValue;
        private readonly long dateTimeMinValue;

        private readonly Random random;
        private readonly IRandomSymbolStringGenerator stringRandomizer;

        public StandardRandomizer(Random random, IRandomSymbolStringGenerator stringRandomizer,
            DateTimeProvider dateProvider, long dateTimeMinValue, long dateTimeMaxValue)
        {
            StandardRandomizer.Logger.Debug("Entering constructor");

            this.random = random;
            this.stringRandomizer = stringRandomizer;
            this.dateProvider = dateProvider;
            this.dateTimeMinValue = dateTimeMinValue;
            this.dateTimeMaxValue = dateTimeMaxValue;

            StandardRandomizer.Logger.Debug("Exiting constructor");
        }

        public virtual int GetInteger(int? min, int? max)
        {
            StandardRandomizer.Logger.Debug("Entering GetInteger");

            max = max ?? int.MaxValue;
            min = min ?? 0;

            StandardRandomizer.Logger.Debug("max = " + max);

            int result = this.random.Next(max.Value - min.Value) + min.Value;

            StandardRandomizer.Logger.Debug($"Exiting GetInteger. result: {result}");
            return result;
        }

        public virtual long GetLongInteger(long? min, long? max)
        {
            StandardRandomizer.Logger.Debug("Entering GetLongInteger");

            long workingMax = (max ?? long.MaxValue) - (min ?? 0);

            StandardRandomizer.Logger.Debug("workingMax = " + workingMax);

            const int wordMask = 0xffff;

            int lowerMaxWord = (int) (workingMax & wordMask);
            if (lowerMaxWord == 0)
                lowerMaxWord = 0x10000;

            long result = this.random.Next(lowerMaxWord);

            for (int i = 0; i < 3; i++)
            {
                workingMax >>= 16;
                int maxRandom = (int) (workingMax & wordMask) + 1;
                long randomValue = this.random.Next(maxRandom);
                randomValue <<= 16 * (i + 1);
                result |= randomValue;
            }

            result += min ?? 0;
            StandardRandomizer.Logger.Debug($"Exiting GetLongInteger. result: {result}");
            return result;
        }

        public virtual short GetShortInteger(short? min, short? max)
        {
            StandardRandomizer.Logger.Debug("Entering GetShortInteger");

            max = (short)((max ?? short.MaxValue) - (min ?? 0));

            StandardRandomizer.Logger.Debug("max = " + max);

            int result = this.random.Next(max.Value);
            result += min ?? 0;

            StandardRandomizer.Logger.Debug($"Exiting GetShortInteger. result: {result}");
            return (short) result;
        }

        public virtual string GetString(int? length)
        {
            StandardRandomizer.Logger.Debug($"Entering GetString. length: {length}");

            string result = this.stringRandomizer.GetRandomString(length);

            StandardRandomizer.Logger.Debug($"Exiting GetString. result: {result}");
            return result;
        }

        public virtual char GetCharacter()
        {
            StandardRandomizer.Logger.Debug("Entering GetCharacter");

            int letterCode = this.random.Next(26);

            char result = (char) (letterCode + 65);

            StandardRandomizer.Logger.Debug($"Exiting GetCharacter. result: {result}");
            return result;
        }

        public virtual decimal GetDecimal(int? precision, decimal? min, decimal? max)
        {
            StandardRandomizer.Logger.Debug("Entering GetDecimal");
            StandardRandomizer.Logger.Debug("precision = " + precision);

            precision = precision ?? 2;

            decimal result = (decimal) this.GetReal(precision.Value, min, max);

            StandardRandomizer.Logger.Debug($"Exiting GetDecimal. result: {result}");
            return result;
        }

        public virtual bool GetBoolean()
        {
            StandardRandomizer.Logger.Debug("Entering GetBoolean");

            int value = this.random.Next(2);

            bool result = value == 1;

            StandardRandomizer.Logger.Debug($"Exiting GetBoolean. result: {result}");
            return result;
        }

        public virtual DateTime GetDateTime(PastOrFuture? pastOrFuture, Func<long?, long?, long> longIntegerGetter, long? min, long? max)
        {
            StandardRandomizer.Logger.Debug($"Entering GetDateTime. pastOrFuture: {pastOrFuture}");

            pastOrFuture = pastOrFuture ?? PastOrFuture.Past;

            DateTime dateTime = this.dateProvider();
            DateTime result;

            long maxLong, randomLong;
            switch (pastOrFuture.Value)
            {
                case PastOrFuture.Future:
                    maxLong = (max ?? this.dateTimeMaxValue) - dateTime.Ticks;
                    randomLong = longIntegerGetter(0, maxLong);
                    result = dateTime.AddTicks(randomLong);
                    break;

                case PastOrFuture.Past:
                    maxLong = dateTime.Ticks - (min ?? this.dateTimeMinValue);
                    randomLong = longIntegerGetter(0, maxLong);
                    result = dateTime.AddTicks(-randomLong);
                    break;

                default:
                    throw new ArgumentException(Messages.UnknownPastOrFutureEnumValue, nameof(pastOrFuture));
            }

            StandardRandomizer.Logger.Debug($"Exiting GetDateTime. result: {result}");
            return result;
        }

        public virtual byte GetByte()
        {
            StandardRandomizer.Logger.Debug("Entering GetByte");

            var array = new byte[1];

            this.random.NextBytes(array);

            byte result = array[0];

            StandardRandomizer.Logger.Debug($"Exiting GetByte. result: {result}");
            return result;
        }

        public virtual double GetDouble(int? precision, decimal? min, decimal? max)
        {
            StandardRandomizer.Logger.Debug("Entering GetDouble");
            StandardRandomizer.Logger.Debug("precision = " + precision);

            precision = precision ?? 2;

            double result = this.GetReal(precision.Value, min, max);

            StandardRandomizer.Logger.Debug($"Exiting GetDouble. result: {result}");
            return result;
        }

        public virtual float GetFloat(int? precision, decimal? min, decimal? max)
        {
            StandardRandomizer.Logger.Debug("Entering GetFloat");
            StandardRandomizer.Logger.Debug("precision = " + precision);

            precision = precision ?? 2;

            if (precision.Value > 7)
                throw new ArgumentOutOfRangeException(nameof(precision), precision.Value,
                    Messages.FloatPrecisionOutOfRange);

            float result = (float) this.GetReal(precision.Value, min, (int) Math.Pow(10, 7 - precision.Value));

            StandardRandomizer.Logger.Debug($"Exiting GetFloat. result: {result}");
            return result;
        }

        public virtual string GetEmailAddress()
        {
            StandardRandomizer.Logger.Debug("Entering GetEmailAddress");

            string namePart = this.stringRandomizer.GetRandomString(10).ToLower();
            string result = namePart + "@domain.com";

            StandardRandomizer.Logger.Debug($"Exiting GetEmailAddress. result: {result}");
            return result;
        }

        public virtual Enum GetEnum(Type enumType)
        {
            Array enumValues = enumType.GetEnumValues();
            int valueIndex = this.random.Next(enumValues.Length);
            var result = (Enum) enumValues.GetValue(valueIndex);
            return result;
        }

        private double GetReal(int precision, decimal? min, decimal? max)
        {
            if (precision < 0)
                throw new ArgumentOutOfRangeException(nameof(precision), precision,
                    Messages.PrecisionMustBeNonNegative);

            if (max.HasValue)
            {
                max -= min ?? 0;
            }

            decimal workingMax = max ?? long.MaxValue;

            long maxWhole = (long)workingMax;
            if (workingMax != maxWhole)
            {
                maxWhole++;
            }

            decimal maxFraction = workingMax - maxWhole;

            long wholePart = this.GetLongInteger(0, maxWhole);

            decimal decimalPart;
            if (wholePart == maxWhole - 1 && maxFraction > 0)
            {
                decimalPart = (decimal)this.random.NextDouble() % maxFraction;
            }
            else
            {
                decimalPart = (decimal)this.random.NextDouble();
            }

            decimal decimalResult = wholePart + decimalPart;
            decimalResult += min ?? 0;
            double result = Math.Round((double)decimalResult, precision);

            return result;
        }
    }
}