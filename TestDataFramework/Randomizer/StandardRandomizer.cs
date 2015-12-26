using System;
using System.Threading;
using TestDataFramework.Helpers.Concrete;
using TestDataFramework.Helpers.Interfaces;

namespace TestDataFramework.Randomizer
{
    public class StandardRandomizer : IRandomizer
    {
        private readonly Random random;
        private readonly IRandomSymbolStringGenerator stringRandomizer;

        public StandardRandomizer(Random random, IRandomSymbolStringGenerator stringRandomizer)
        {
            this.random = random;
            this.stringRandomizer = stringRandomizer;
        }

        public int RandomizeInteger(int? max)
        {
            int result = max != null ? this.random.Next(max.Value) : this.random.Next();
            return result;
        }

        public long RandomizeLongInteger(long? max)
        {
            long workingMax = max ?? long.MaxValue;

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

            return result;
        }

        public short RandomizeShortInteger(short? max)
        {
            int result = max != null ? this.random.Next(max.Value) : this.random.Next(short.MaxValue);
            return (short)result;
        }

        public string RandomizeString(int? length)
        {
            string result = this.stringRandomizer.GetRandomString(length);
            return result;
        }

        public char RandomizeCharacter()
        {
            int letterCode = this.random.Next(26);

            var result = (char)(letterCode + 65);

            return result;
        }

        public decimal RandomizeDecimal(int? precision)
        {
            precision = precision ?? 2;

            var result = (decimal) this.GetReal(precision.Value);

            return result;
        }

        public bool RandomizeBoolean()
        {
            int value = this.random.Next(2);

            bool result = value == 1;

            return result;
        }

        public DateTime RandomizeDateTime()
        {
            throw new NotImplementedException();
        }

        public byte RandomizeByte()
        {
            throw new NotImplementedException();
        }

        public double RandomizeDouble(int? precision)
        {
            precision = precision ?? 2;

            double result = this.GetReal(precision.Value);

            return result;
        }

        public object RandomizeEmailAddress()
        {
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