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

        public int RandomizeInteger()
        {
            int result = this.random.Next();
            return result;
        }

        public long RandomizeLongInteger()
        {
            throw new NotImplementedException();
        }

        public short RandomizeShortInteger()
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