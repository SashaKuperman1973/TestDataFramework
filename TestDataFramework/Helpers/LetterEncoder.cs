using System;
using System.Text;
using log4net;
using TestDataFramework.Exceptions;

namespace TestDataFramework.Helpers
{
    public class LetterEncoder
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LetterEncoder));

        public virtual string Encode(LargeInteger number, int maxStringLength)
        {
            LetterEncoder.Logger.Debug($"Entering Encode. number: {number}, maxStringLength: {maxStringLength}");

            var sb = new StringBuilder();

            int digitCount = 0;

            LargeInteger whole = number;
            while (digitCount++ < maxStringLength)
            {
                LargeInteger remainder = whole%26;
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

            if (digitCount > maxStringLength)
            {
                throw new OverflowException(string.Format(Messages.StringGeneratorOverflow, number, maxStringLength));
            }

            string result = sb.ToString();

            LetterEncoder.Logger.Debug($"Exiting Encode. result : {result}");
            return result;
        }

        public virtual LargeInteger Decode(string value)
        {
            LetterEncoder.Logger.Debug($"Entering Encode. value: {value}");

            LargeInteger result = 0;

            for (int i = 0; i < value.Length; i++)
            {
                var ascii = (ulong)value[value.Length - 1 - i];

                result += new LargeInteger(26).Pow((ulong)i) * (ascii - 65);
            }

            LetterEncoder.Logger.Debug($"Exiting Encode. result: {result}");
            return result;
        }
    }
}
