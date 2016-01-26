using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.ValueProvider
{
    public class ValueAccumulator : IValueProvider
    {
        public int GetInteger(int? max)
        {
            throw new NotImplementedException();
        }

        public long GetLongInteger(long? max)
        {
            throw new NotImplementedException();
        }

        public short GetShortInteger(short? max)
        {
            throw new NotImplementedException();
        }

        public string GetString(int? length)
        {
            throw new NotImplementedException();
        }

        public char GetCharacter()
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int? precision)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean()
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(PastOrFuture? pastOrFuture, Func<long?, long> longIntegerGetter)
        {
            throw new NotImplementedException();
        }

        public byte GetByte()
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int? precision)
        {
            throw new NotImplementedException();
        }

        public string GetEmailAddress()
        {
            throw new NotImplementedException();
        }
    }
}
