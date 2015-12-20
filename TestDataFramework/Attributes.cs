using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
