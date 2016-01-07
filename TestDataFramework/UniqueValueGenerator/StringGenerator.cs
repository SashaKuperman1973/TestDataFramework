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
        public virtual string GetValue(ulong number, int stringLength)
        {
            var sb = new StringBuilder();

            int digitCount = 0;

            ulong whole = number;
            while (digitCount++ < stringLength)
            {
                ulong remainder = whole%26;
                whole /= 26;

                if (remainder == 0 && whole == 0)
                {
                    if (digitCount == 1)
                    {
                        sb.Insert(0, "A");
                    }

                    break;
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
