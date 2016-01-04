using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;

namespace TestDataFramework.UniqueValueGenerator
{
    public class StringGenerator
    {
        public virtual string GetValue(long number, int stringLength)
        {
            var sb = new StringBuilder();

            int digitCount = 0;

            long whole = number;
            while (digitCount++ < stringLength)
            {
                long remainder;

                if (whole < 27)
                {
                    if (whole == 0)
                    {
                        break;
                    }

                    remainder = whole - 1;
                    whole = 0;
                }
                else
                {
                    remainder = whole%26;
                    whole /= 26;
                }

                var ascii = (char)(remainder + 65);
                sb.Insert(0, ascii);
            }

            if (digitCount > stringLength)
            {
                throw new OverflowException(string.Format(Messages.StringGeneratorOverflow, number, stringLength));
            }

            string result = sb.ToString();

            return result;
        }
    }
}
