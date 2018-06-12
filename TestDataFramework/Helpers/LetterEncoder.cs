/*
    Copyright 2016, 2017 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Text;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Logger;

namespace TestDataFramework.Helpers
{
    public class LetterEncoder
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(LetterEncoder));

        public virtual string Encode(LargeInteger number, int maxStringLength)
        {
            LetterEncoder.Logger.Debug($"Entering Encode. number: {number}, maxStringLength: {maxStringLength}");

            var sb = new StringBuilder();

            var digitCount = 0;

            LargeInteger whole = number;
            while (digitCount++ < maxStringLength)
            {
                LargeInteger remainder = whole % 26;
                whole /= 26;

                if (remainder == 0 && whole == 0)
                {
                    if (digitCount == 1)
                        sb.Insert(0, "A");

                    break;
                }

                var ascii = (char) (remainder + 65);
                sb.Insert(0, ascii);
            }

            if (digitCount > maxStringLength)
                throw new OverflowException(string.Format(Messages.StringGeneratorOverflow, number, maxStringLength));

            var result = sb.ToString();

            LetterEncoder.Logger.Debug($"Exiting Encode. result : {result}");
            return result;
        }

        public virtual LargeInteger Decode(string value)
        {
            LetterEncoder.Logger.Debug($"Entering Encode. value: {value}");

            LargeInteger result = 0;

            for (var i = 0; i < value.Length; i++)
            {
                var ascii = (ulong) value[value.Length - 1 - i];

                result += new LargeInteger(26).Pow((ulong) i) * (ascii - 65);
            }

            LetterEncoder.Logger.Debug($"Exiting Encode. result: {result}");
            return result;
        }
    }
}