using System;
using System.Text;
using TestDataFramework.Exceptions;

namespace TestDataFramework.Helpers
{
    public class LetterEncoder
    {
        public virtual string Encode(ulong number, int stringLength)
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

        public virtual ulong Decode(string value)
        {
            ulong result = Helper.DefaultInitalCount;

            for (int i = 0; i < value.Length; i++)
            {
                var ascii = (ulong)value[value.Length - 1 - i];
                result += (ascii - 65) * (ulong)Math.Pow(26, i);
            }

            return result;
        }
    }
}
