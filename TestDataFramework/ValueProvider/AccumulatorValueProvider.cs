using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;

namespace TestDataFramework.ValueProvider
{
    public class AccumulatorValueProvider : IValueProvider
    {
        private short count = (short)Helper.DefaultInitalCount;

        private static readonly LetterEncoder LetterEncoder = new LetterEncoder();

        public int GetInteger(int? max)
        {
            return this.count++;
        }

        public long GetLongInteger(long? max)
        {
            return this.count++;
        }

        public short GetShortInteger(short? max)
        {
            return this.count++;
        }

        public string GetString(int? length)
        {
            int lengthToUse = length ?? 10;

            string result = AccumulatorValueProvider.LetterEncoder.Encode((ulong) this.count++, lengthToUse);

            result = result.PadRight(lengthToUse, '+');

            return result;
        }

        public char GetCharacter()
        {
            const int startCode = 0xc0;
            var result = (char)(startCode + this.count++);

            return result;
        }

        public decimal GetDecimal(int? precision)
        {
            decimal result = this.count++;
            return result;
        }

        private short booleanCount = 0;

        public bool GetBoolean()
        {
            if (this.booleanCount > 1)
            {
                throw new OverflowException(Messages.RequestForMoreThan2UniqueBooleanValues);
            }

            bool result = this.booleanCount++ == 1;
            return result;
        }

        public DateTime GetDateTime(PastOrFuture? pastOrFuture, Func<long?, long> longIntegerGetter)
        {
            DateTime result = DateTime.Now.AddDays(this.count++);
            return result;
        }

        public byte GetByte()
        {
            if (++this.count > byte.MaxValue)
            {
                throw new OverflowException(Messages.ByteUniqueValueOverflow);
            }

            var result = (byte) this.count;
            return result;
        }

        public double GetDouble(int? precision)
        {
            double result = this.count++;
            return result;
        }

        public float GetFloat(int? precision)
        {
            float result = this.count++;
            return result;
        }

        public string GetEmailAddress()
        {
            throw new NotImplementedException();
        }
    }
}
