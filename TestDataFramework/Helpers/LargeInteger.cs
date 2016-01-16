using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using TestDataFramework.Exceptions;

namespace TestDataFramework.Helpers
{
    public struct LargeInteger
    {
        private List<byte> data;

        public LargeInteger(ulong? initialValue)
        {
            this.data = LargeInteger.GetInitialData();

            if (initialValue.HasValue)
            {
                LargeInteger.Encode(initialValue.Value, this);
            }
        }

        #region Ensure
        // My attempt at a value type fluent calling convention.

        private LargeInteger Ensure()
        {
            return LargeInteger.Ensure(ref this, null);
        }

        private LargeInteger Ensure(ref LargeInteger largeInteger)
        {
            this.Ensure();
            return LargeInteger.Ensure(ref largeInteger, null);            
        }

        private static LargeInteger Ensure(ref LargeInteger largeInteger, object o)
        {
            if (largeInteger.data == null)
            {
                largeInteger.data = LargeInteger.GetInitialData();
            }

            return largeInteger;
        }

        private static List<byte> GetInitialData()
        {
            return new List<byte>(new byte[] {0});
        }

        #endregion Ensure
        
        public static explicit operator ulong(LargeInteger largeInteger)
        {
            largeInteger.Ensure();
            return LargeInteger.Decode(largeInteger);
        }

        public static implicit operator LargeInteger(ulong value)
        {
            var result = new LargeInteger();
            result.Ensure();
            LargeInteger.Encode(value, result);
            return result;
        }

        public static LargeInteger operator +(LargeInteger left, LargeInteger right)
        {
            left.Ensure(ref right);

            while (right.data.Count > 1 || right.data[0] > 0)
            {
                right--;
                left++;
            } 

            return left;
        }

        public static LargeInteger operator -(LargeInteger left, LargeInteger right)
        {
            left.Ensure(ref right);

            while (right.data.Count > 1 || right.data[0] > 0)
            {
                right--;
                left--;
            }

            return left;
        }

        // Returns Tuple<quotient, modulus>
        public static Tuple<LargeInteger, LargeInteger> Divide(LargeInteger numerator, LargeInteger denominator)
        {
            numerator.Ensure(ref denominator);

            var quotient = new LargeInteger(0);

            if (numerator < denominator)
            {
                return new Tuple<LargeInteger, LargeInteger>(quotient, numerator);
            }

            do
            {
                quotient++;
            } while ((numerator -= denominator) > denominator);

            return new Tuple<LargeInteger, LargeInteger>(quotient, numerator);
        }

        public static LargeInteger operator /(LargeInteger numerator, LargeInteger denominator)
        {
            numerator.Ensure(ref denominator);
            return LargeInteger.Divide(numerator, denominator).Item1;
        }
        public static LargeInteger operator %(LargeInteger numerator, LargeInteger denominator)
        {
            numerator.Ensure(ref denominator);
            return LargeInteger.Divide(numerator, denominator).Item2;
        }

        private static int CompareSameLength(LargeInteger left, LargeInteger right)
        {
            int i;
            for (i = 0; i < left.data.Count; i++)
            {
                if (left.data[i] != right.data[i])
                {
                    break;
                }
            }

            if (i == left.data.Count)
            {
                return 0;
            }

            return left.data[i] < right.data[i] ? -1 : 1;
        }

        public static bool operator >(LargeInteger left, LargeInteger right)
        {
            left.Ensure(ref right);
            bool result = left.data.Count > right.data.Count ||
                          left.data.Count == right.data.Count && LargeInteger.CompareSameLength(left, right) > 0;

            return result;
        }

        public static bool operator <(LargeInteger left, LargeInteger right)
        {
            left.Ensure(ref right);
            bool result = left.data.Count < right.data.Count ||
                          left.data.Count == right.data.Count && LargeInteger.CompareSameLength(left, right) < 0;

            return result;
        }

        public static bool operator ==(LargeInteger left, LargeInteger right)
        {
            left.Ensure(ref right);
            bool result = left.data.Count == right.data.Count && LargeInteger.CompareSameLength(left, right) == 0;
            return result;
        }

        public static bool operator !=(LargeInteger left, LargeInteger right)
        {
            left.Ensure(ref right);
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            this.Ensure();

            if (!(obj is LargeInteger))
            {
                return false;
            }

            var largeInteger = (LargeInteger)obj;
            largeInteger.Ensure();

            return this == largeInteger;
        }

        public override int GetHashCode()
        {
            this.Ensure();
            return this.data.GetHashCode();
        }

        public static LargeInteger operator ++(LargeInteger largeInteger)
        {
            largeInteger.Ensure();
            int position = 0;

            while (largeInteger.data[position] == byte.MaxValue)
            {
                largeInteger.data[position++] = 0;

                if (largeInteger.data.Count == position)
                {
                    largeInteger.data.Add(0);
                }
            }

            largeInteger.data[position]++;

            return largeInteger;
        }

        public static LargeInteger operator --(LargeInteger largeInteger)
        {
            largeInteger.Ensure();
            int position = 0;

            while (largeInteger.data[position] == 0)
            {
                largeInteger.data[position++] = 0xff;

                if (largeInteger.data.Count == position)
                {
                    throw new OverflowException(Messages.Underflow);
                }
            }

            largeInteger.data[position]--;

            if (largeInteger.data[position] == 0 && position > 0)
            {
                largeInteger.data.RemoveAt(position);
            }

            return largeInteger;
        }

        private static void Encode(ulong value, LargeInteger largeInteger)
        {
            do
            {
                ulong remainder = value%byte.MaxValue;
                value /= byte.MaxValue;

                largeInteger.data.Add((byte) remainder);

            } while (value > 0);
        }

        private static ulong Decode(LargeInteger largeInteger)
        {
            ulong result = 0;

            for (int i=0; i < largeInteger.data.Count; i++)
            {
                result += (ulong) Math.Pow(byte.MaxValue, i)*largeInteger.data[i];
            }

            return result;
        }
    }
}
