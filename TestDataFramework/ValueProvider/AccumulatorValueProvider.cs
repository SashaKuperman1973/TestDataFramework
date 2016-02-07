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
        private short countField = (short)Helper.DefaultInitalCount;

        private short Count
        {
            get
            {
                if (this.countField != short.MaxValue)
                {
                    return this.countField;
                }

                this.countField = (short)Helper.DefaultInitalCount;
                return this.countField;
            }

            set { this.countField = value; }
        }

        private static readonly LetterEncoder LetterEncoder = new LetterEncoder();

        public int GetInteger(int? max)
        {
            return this.Count++;
        }

        public long GetLongInteger(long? max)
        {
            return this.Count++;
        }

        public short GetShortInteger(short? max)
        {
            return this.Count++;
        }

        public string GetString(int? length)
        {
            int lengthToUse = length ?? 10;

            string result = AccumulatorValueProvider.LetterEncoder.Encode((ulong) this.Count++, lengthToUse);

            result = result.PadRight(lengthToUse, '+');

            return result;
        }

        private int characterCount;

        public char GetCharacter()
        {
            const int startCode = 33;
            var result = (char)(startCode + this.characterCount++);

            return result;
        }

        public decimal GetDecimal(int? precision)
        {
            decimal result = this.Count++;
            return result;
        }

        private bool boolean = false;

        public bool GetBoolean()
        {
            bool result = this.boolean;
            this.boolean = !this.boolean;
            return result;
        }

        public DateTime GetDateTime(PastOrFuture? pastOrFuture, Func<long?, long> longIntegerGetter)
        {
            DateTime result = DateTime.Now.AddDays(this.Count++);
            return result;
        }

        public byte GetByte()
        {
            if (++this.Count > byte.MaxValue)
            {
                throw new OverflowException(Messages.ByteUniqueValueOverflow);
            }

            var result = (byte) this.Count;
            return result;
        }

        public double GetDouble(int? precision)
        {
            double result = this.Count++;
            return result;
        }

        public float GetFloat(int? precision)
        {
            float result = this.Count++;
            return result;
        }

        public string GetEmailAddress()
        {
            throw new NotImplementedException();
        }
    }
}
