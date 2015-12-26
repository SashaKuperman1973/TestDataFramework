using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Randomizer;

namespace TestDataFramework
{
    public class StringLengthAttribute : Attribute
    {
        public StringLengthAttribute(int length)
        {
            this.Length = length;
        }

        public int Length { get; }
    }

    public class PrecisionAttribute : Attribute
    {
        public PrecisionAttribute(int precision)
        {
            this.Precision = precision;
        }

        public int Precision { get; }
    }

    public class EmailAttribute : Attribute
    {        
    }

    public class MaxAttribute : Attribute
    {
        public MaxAttribute(long max)
        {
            this.Max = max;
        }

        public long Max { get; }
    }

    public class PastOrFutureAttribute : Attribute
    {
        public PastOrFutureAttribute(PastOrFuture pastOrFuture)
        {
            this.PastOrFuture = pastOrFuture;
        }

        public PastOrFuture PastOrFuture { get; }
    }
}
