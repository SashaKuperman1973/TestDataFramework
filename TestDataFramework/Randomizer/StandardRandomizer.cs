using System;

namespace TestDataFramework.Randomizer
{
    public class StandardRandomizer : IRandomizer
    {
        private readonly Random random;

        public StandardRandomizer(Random random)
        {
            this.random = random;
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
            throw new NotImplementedException();
        }

        public string RandomizeString(int? length)
        {
            throw new NotImplementedException();
        }

        public char RandomizeCharacter()
        {
            throw new NotImplementedException();
        }

        public decimal RandomizeDecimal(int? precision)
        {
            throw new NotImplementedException();
        }

        public bool RandomizeBoolean()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public object RandomizeEmailAddress()
        {
            throw new NotImplementedException();
        }
    };
}