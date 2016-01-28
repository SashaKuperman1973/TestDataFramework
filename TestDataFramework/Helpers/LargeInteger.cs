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
        private List<uint> data;

        public LargeInteger(ulong initialValue)
        {
            this.data = new List<uint>();
            LargeInteger.Encode(initialValue, this);
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

        private static List<uint> GetInitialData()
        {
            return new List<uint>(new uint[] {0});
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
            result.data = new List<uint>();
            LargeInteger.Encode(value, result);
            return result;
        }

        public static LargeInteger operator +(LargeInteger left, LargeInteger right)
        {
            return LargeInteger.Accumulate(left, right, (a, b) => a + b);
        }

        public static LargeInteger operator *(LargeInteger left, LargeInteger right)
        {
            return LargeInteger.Accumulate(left, right, (a, b) => a * b);
        }

        private static LargeInteger Accumulate(LargeInteger left, LargeInteger right, Func<ulong, ulong, ulong> operation)
        {
            left.Ensure(ref right);

            LargeInteger accumulator, operand;

            if (left < right)
            {
                accumulator = left;
                operand = right;
            }
            else
            {
                accumulator = right;
                operand = left;
            }

            int i = 0;
            ulong scratch = 0;

            do
            {
                scratch = operation(accumulator.data[i], operand.data[i]) + scratch;
                accumulator.data[i] = (uint) (scratch & 0xffffffff);
                scratch >>= 32;

                i++;
                if (i < accumulator.data.Count)
                {
                    continue;
                }

                if (i < operand.data.Count)
                {
                    accumulator.data.Add(0);
                    continue;
                }

                if (scratch > 0)
                {
                    accumulator.data.Add(0);
                    accumulator.data[i] = (uint) scratch;
                }

                break;

            } while (true);

            return accumulator;
        }

        public static LargeInteger operator -(LargeInteger accumulator, LargeInteger operand)
        {
            accumulator.Ensure(ref operand);

            if (accumulator < operand)
            {
                throw new OverflowException(Messages.LargeIntegerUnderFlow);
            }

            int i = 0;
            long borrow = 0;

            do
            {
                long scratch = accumulator.data[i] - operand.data[i] - borrow;

                if (scratch < 0)
                {
                    borrow = 1;
                    accumulator.data[i] = (uint) (0x100000000 + scratch);
                }
                else
                {
                    borrow = 0;
                    accumulator.data[i] = (uint) scratch;
                }

                i++;

                if (i < operand.data.Count)
                {
                    continue;
                }

                if (borrow == 1)
                {
                    accumulator.data[i]--;
                }

                break;

            } while (true);

            int accumulatorDataLength = accumulator.data.Count;

            for (int j = accumulatorDataLength - 1; j >= 0; j--)
            {
                if (accumulator.data[j] != 0)
                {
                    break;
                }

                accumulator.data.RemoveAt(j);
            }

            return accumulator;
        }

        // Returns Tuple<quotient, modulus>
        private static Tuple<LargeInteger, LargeInteger> Divide(LargeInteger numerator, LargeInteger denominator)
        {
            numerator.Ensure(ref denominator);

            if (denominator == 0)
            {
                throw new DivideByZeroException("LargeInteger: divide by zero");
            }

            var quotient = new LargeInteger(0);

            if (numerator < denominator)
            {
                return new Tuple<LargeInteger, LargeInteger>(quotient, numerator);
            }

            if (numerator.data.Count <= 2 && denominator.data.Count <= 2)
            {
                ulong primitiveNumerator = LargeInteger.GetULong(numerator.data);
                ulong primitiveDenominator = LargeInteger.GetULong(denominator.data);

                ulong primitiveQuotient = primitiveNumerator / primitiveDenominator;
                ulong primitiveRemainder = primitiveNumerator % primitiveDenominator;

                quotient = new LargeInteger(primitiveQuotient);
                var remainder = new LargeInteger(primitiveRemainder);

                return new Tuple<LargeInteger, LargeInteger>(quotient, remainder);
            }

            do
            {
                quotient++;
            } while ((numerator -= denominator) > denominator);

            return new Tuple<LargeInteger, LargeInteger>(quotient, numerator);
        }

        private static ulong GetULong(IList<uint> input)
        {
            ulong result = input[0] + input.Count == 2
                ? (ulong) input[1] << 32
                : 0;

            return result;
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

            var result = new LargeInteger();
            result.data = largeInteger.data.GetRange(0, largeInteger.data.Count);

            int position = 0;

            while (result.data[position] == uint.MaxValue)
            {
                result.data[position++] = 0;

                if (result.data.Count == position)
                {
                    result.data.Add(0);
                }
            }

            result.data[position]++;

            return result;
        }

        public static LargeInteger operator --(LargeInteger largeInteger)
        {
            largeInteger.Ensure();

            var result = new LargeInteger();
            result.data = largeInteger.data.GetRange(0, largeInteger.data.Count);

            int position = 0;

            while (result.data[position] == 0)
            {
                result.data[position++] = 0xff;

                if (result.data.Count == position)
                {
                    throw new OverflowException(Messages.Underflow);
                }
            }

            result.data[position]--;

            if (result.data[position] == 0 && position > 0)
            {
                result.data.RemoveAt(position);
            }

            return result;
        }

        public static LargeInteger Pow(LargeInteger @base, LargeInteger power)
        {
            if (@base == 0 && power == 0)
            {
                throw new DivideByZeroException("LargeInteger: divide by zero");
            }

            var result = new LargeInteger(1);

            for (var i = new LargeInteger(0); i < power; i++)
            {
                result *= @base;
            }

            return result;
        }

        public static LargeInteger Pow(LargeInteger @base, ulong power)
        {
            if (@base == 0 && power == 0)
            {
                throw new DivideByZeroException("LargeInteger: divide by zero");
            }

            var result = new LargeInteger(1);

            for (var i = 0uL; i < power; i++)
            {
                result *= @base;
            }

            return result;
        }

        private static void Encode(ulong value, LargeInteger largeInteger)
        {
            var lower = (uint)(value & 0xffffffff);
            var upper = (uint) (value >> 32);

            largeInteger.data.Add(lower);

            if (upper > 0)
            {
                largeInteger.data.Add(upper);
            }
        }

        private static ulong Decode(LargeInteger largeInteger)
        {
            if (largeInteger.data.Count > 2)
            {
                throw new OverflowException();
            }

            ulong result = largeInteger.data[0] + ((ulong)largeInteger.data[1] << 32);

            return result;
        }
    }
}
