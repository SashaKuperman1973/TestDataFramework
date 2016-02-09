using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Castle.Components.DictionaryAdapter.Xml;
using log4net;
using TestDataFramework.Exceptions;

namespace TestDataFramework.Helpers
{
    public struct LargeInteger
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LargeInteger));

        private List<uint> data;

        public LargeInteger(ulong initialValue)
        {
            LargeInteger.Logger.Debug($"Entering constructor LargeInteger(ulong initialVlaue). initialValue: {initialValue}");

            this.data = new List<uint>();
            LargeInteger.Encode(initialValue, this);

            LargeInteger.Logger.Debug("Entering constructor LargeInteger(ulong initialVlaue)");
        }

        private LargeInteger Clone()
        {
            var result = new LargeInteger {data = this.data.GetRange(0, this.data.Count) };
            return result;
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
            else if (!largeInteger.data.Any())
            {
                largeInteger.data.Add(0);
            }

            return largeInteger;
        }

        private static List<uint> GetInitialData()
        {
            return new List<uint>(new uint[] {0});
        }

        #endregion Ensure

        public override string ToString()
        {
            LargeInteger.Logger.Debug("Entering ToString");

            if (this == 0)
            {
                LargeInteger.Logger.Debug("Returning 0");
                return "0";
            }

            LargeInteger workingLargeInteger = this.Clone();
            var sb = new StringBuilder();

            do
            {
                Tuple<LargeInteger, LargeInteger> quotient = workingLargeInteger.Divide(10);
                workingLargeInteger = quotient.Item1;
                sb.Insert(0, (char)((ulong) quotient.Item2 + 48));

            } while (workingLargeInteger > 0);

            return sb.ToString();
        }

        public static explicit operator ulong(LargeInteger largeInteger)
        {
            LargeInteger.Logger.Debug($"Entering ulong(LargeInteger largeInteger). largeInteger: {largeInteger}");

            largeInteger.Ensure();

            ulong result =  LargeInteger.Decode(largeInteger);

            LargeInteger.Logger.Debug($"Exiting ulong(LargeInteger largeInteger). result: {result}");
            return result;
        }

        public static implicit operator LargeInteger(ulong value)
        {
            LargeInteger.Logger.Debug($"Entering LargeInteger(ulong value). value: {value}");

            var result = new LargeInteger {data = new List<uint>() };
            LargeInteger.Encode(value, result);

            LargeInteger.Logger.Debug($"Exiting LargeInteger(ulong value). result: {result}");
            return result;
        }

        public static LargeInteger operator +(LargeInteger left, LargeInteger right)
        {
            LargeInteger.Logger.Debug($"Entering operator +. left: {left}, right: {right}");

            left.Ensure(ref right);

            LargeInteger accumulator, operand;

            if (left < right)
            {
                accumulator = left.Clone();
                operand = right;
            }
            else
            {
                accumulator = right.Clone();
                operand = left;
            }

            int i = 0;
            ulong scratch = 0;

            do
            {
                scratch = (ulong)accumulator.data[i] + operand.data[i] + scratch;
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

            LargeInteger.Logger.Debug($"Exiting operator +. result accumulator: {accumulator}");

            return accumulator;
        }

        public static LargeInteger operator -(LargeInteger left, LargeInteger operand)
        {
            LargeInteger.Logger.Debug($"Entering operator -. left: {left}, right: {operand}");

            left.Ensure(ref operand);

            if (left < operand)
            {
                throw new OverflowException(Messages.LargeIntegerUnderFlow);
            }

            LargeInteger accumulator = left.Clone();

            int i = 0;
            long borrow = 0;

            do
            {
                long scratch = (long)accumulator.data[i] - operand.data[i] - borrow;

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

            LargeInteger.Logger.Debug($"Exiting operator -. result accumulator: {accumulator}");

            return accumulator;
        }

        public static LargeInteger operator *(LargeInteger left, LargeInteger right)
        {
            LargeInteger.Logger.Debug($"Entering operator *. left: {left}, right: {right}");

            left.Ensure(ref right);

            LargeInteger accumulator = 0, scratch = 0, lowerOperand, upperOperand;

            if (left < right)
            {
                lowerOperand = left;
                upperOperand = right;
            }
            else
            {
                lowerOperand = right;
                upperOperand = left;
            }

            int i = 0;
            int j = 0;

            do
            {
                scratch += (ulong)lowerOperand.data[i]*upperOperand.data[j];

                for (int k = 0; k < i + j; k++)
                {
                    scratch.data.Insert(0, 0);
                }

                accumulator += scratch;
                scratch = 0;

                j++;

                if (j < upperOperand.data.Count)
                {
                    continue;
                }

                j = 0;
                i++;

                if (i < lowerOperand.data.Count)
                {
                    continue;
                }

                break;

            } while (true);

            LargeInteger.Logger.Debug($"Exiting operator *. result accumulator: {accumulator}");

            return accumulator;
        }

        public static LargeInteger operator /(LargeInteger numerator, LargeInteger divisor)
        {
            LargeInteger.Logger.Debug($"Entering operator /. numerator: {numerator}, divisor: {divisor}");

            LargeInteger result = numerator.Divide(divisor).Item1;

            LargeInteger.Logger.Debug($"Exiting operator /. result: {result}" );

            return result;
        }

        public static LargeInteger operator %(LargeInteger numerator, LargeInteger divisor)
        {
            LargeInteger.Logger.Debug($"Entering operator %. numerator: {numerator}. divisor: {divisor}");

            LargeInteger result = numerator.Divide(divisor).Item2;

            LargeInteger.Logger.Debug($"Exiting operator %. result: {result}");

            return result;
        }

#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
        private class QuotienScratchAdjuster
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
        {
            private LargeInteger memberValue;

            public static bool operator ==(QuotienScratchAdjuster left, ulong right)
            {
                return left.memberValue == right;
            }

            public static bool operator !=(QuotienScratchAdjuster left, ulong right)
            {
                return !(left == right);
            }

            public static QuotienScratchAdjuster operator ++(QuotienScratchAdjuster input)
            {
                return new QuotienScratchAdjuster { memberValue = ++input };
            }

            public static QuotienScratchAdjuster operator --(QuotienScratchAdjuster input)
            {
                return new QuotienScratchAdjuster { memberValue = --input };
            }

            public static implicit operator QuotienScratchAdjuster(ulong value)
            {
                return new QuotienScratchAdjuster { memberValue = new LargeInteger(value) };
            }

            public static implicit operator QuotienScratchAdjuster(LargeInteger value)
            {
                return new QuotienScratchAdjuster { memberValue = value };
            }

            public static implicit operator LargeInteger(QuotienScratchAdjuster value)
            {
                return value.memberValue;
            }

            public void Half()
            {
                if (this.memberValue.data == null || this.memberValue.data.Count == 0 || this.memberValue.data.Count == 1 && this.memberValue.data[0] <= 1)
                {
                    return;
                }

                int count = this.memberValue.data.Count;

                uint highBit = 0;
                for (int i = count - 1; i >= 0; i--)
                {
                    uint nextHighBit = this.memberValue.data[i] << 31;
                    this.memberValue.data[i] >>= 1;
                    this.memberValue.data[i] |= highBit;
                    highBit = nextHighBit;
                }

                if (this.memberValue.data[count - 1] == 0)
                {
                    this.memberValue.data.RemoveAt(count - 1);
                }
            }
        }

        /// <summary>
        /// Divides the current instance by the divisor.
        /// </summary>
        /// <param name="divisor">Divisor</param>
        /// <returns>Tuple&lt;Quotient, Remainder&gt;</returns>
        public Tuple<LargeInteger, LargeInteger> Divide(LargeInteger divisor)
        {
            LargeInteger.Logger.Debug($"Entering Divide. this (numerator): {this}, divisor: {divisor}");

            this.Ensure().Ensure(ref divisor);

            if (this < divisor)
            {
                return new Tuple<LargeInteger, LargeInteger>(0, this);
            }

            // This is long division.

            var quotient = new LargeInteger { data = new List<uint>() };

            var testNumerator = new LargeInteger { data = new List<uint>() };
            int numeratorPosition = this.data.Count - 1;

            for (int i = 0; i < divisor.data.Count; i++)
            {
                testNumerator.data.Insert(0, this.data[numeratorPosition--]);
            }
            if (testNumerator < divisor)
            {
                testNumerator.data.Insert(0, this.data[numeratorPosition--]);
            }

            int numberOfPlacesAddedToLastTestNumerator = 0;

            do
            {
                int adjusterLength = testNumerator.data.Count - divisor.data.Count + 1;
                QuotienScratchAdjuster adjuster = new LargeInteger
                {
                    data = new List<uint>(adjusterLength)
                };

                for (int i = 0; i < adjusterLength; i++)
                {
                    ((LargeInteger)adjuster).data.Add(0);
                }

                ((LargeInteger)adjuster).data.Add(1);

                LargeInteger quotientScratch = ((LargeInteger)adjuster).Clone();

                LargeInteger compareNumerator;
                do
                {
                    compareNumerator = quotientScratch * divisor;
                    if (compareNumerator > testNumerator)
                    {
                        if (adjuster == 1)
                        {
                            quotientScratch--;
                            compareNumerator -= divisor;
                            if (compareNumerator < testNumerator)
                            {
                                break;
                            }
                        }
                        else
                        {
                            adjuster.Half();
                            quotientScratch -= adjuster;
                        }
                    }
                    else if (compareNumerator < testNumerator)
                    {
                        if (adjuster == 1)
                        {
                            LargeInteger tempCompareNumerator = compareNumerator;
                            compareNumerator += divisor;
                            if (compareNumerator > testNumerator)
                            {
                                compareNumerator = tempCompareNumerator;
                                break;
                            }
                            quotientScratch++;
                        }
                        else
                        {
                            adjuster.Half();
                            quotientScratch += adjuster;
                        }
                    }
                    else
                    {
                        break;
                    }

                } while (true);

                if (numberOfPlacesAddedToLastTestNumerator > quotientScratch.data.Count)
                {
                    if (numberOfPlacesAddedToLastTestNumerator + 1 != quotientScratch.data.Count)
                    {
                        throw new ApplicationException(
                            $"Internal error: numberOfPlacesAddedToLastTestNumerator > quotientScratch.data.Count && numberOfPlacesAddedToLastTestNumerator {numberOfPlacesAddedToLastTestNumerator} + 1 != quotientScratch.data.Count {quotientScratch.data.Count}. \r\nNumerator:\r\n{LargeInteger.PrintLargeInteger(this)}\r\nDivisor:\r\n{LargeInteger.PrintLargeInteger(divisor)}");
                    }

                    quotient.data.Insert(0, 0);
                }

                for (int i = quotientScratch.data.Count - 1; i >= 0; i--)
                {
                    quotient.data.Insert(0, quotientScratch.data[i]);
                }

                testNumerator -= compareNumerator;

                numberOfPlacesAddedToLastTestNumerator = 0;
                for (int i = 0; i < divisor.data.Count; i++)
                {
                    if (numeratorPosition < 0)
                    {
                        break;
                    }
                    testNumerator.data.Insert(0, this.data[numeratorPosition--]);
                    numberOfPlacesAddedToLastTestNumerator++;
                }

                if (testNumerator < divisor)
                {
                    if (numeratorPosition < 0)
                    {
                        LargeInteger.Logger.Debug($"Exiting Divide. quotient: {quotient}, remainder: {testNumerator}");
                        return new Tuple<LargeInteger, LargeInteger>(quotient, testNumerator);
                    }
                    testNumerator.data.Insert(0, this.data[numeratorPosition--]);
                    numberOfPlacesAddedToLastTestNumerator++;
                }

            } while (true);
        }

        private static string PrintLargeInteger(LargeInteger largeInteger)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < largeInteger.data.Count; i++)
            {
                sb.AppendLine($"[{i}] {largeInteger.data[i].ToString("x")}");
            }

            return sb.ToString();
        }

        private static int CompareSameLength(LargeInteger left, LargeInteger right)
        {
            int i;
            for (i = left.data.Count - 1; i >= 0; i--)
            {
                if (left.data[i] != right.data[i])
                {
                    break;
                }
            }

            if (i == -1)
            {
                return 0;
            }

            return left.data[i] < right.data[i] ? -1 : 1;
        }

        public static bool operator >(LargeInteger left, LargeInteger right)
        {
            LargeInteger.Logger.Debug($"Entering operator >. left: {left}, right: {right}");

            left.Ensure(ref right);
            bool result = left.data.Count > right.data.Count ||
                          left.data.Count == right.data.Count && LargeInteger.CompareSameLength(left, right) > 0;

            LargeInteger.Logger.Debug($"Exiting operator >. result: {result}");

            return result;
        }

        public static bool operator <(LargeInteger left, LargeInteger right)
        {
            LargeInteger.Logger.Debug($"Entering operator <. left: {left}, right: {right}");

            left.Ensure(ref right);
            bool result = left.data.Count < right.data.Count ||
                          left.data.Count == right.data.Count && LargeInteger.CompareSameLength(left, right) < 0;

            LargeInteger.Logger.Debug($"Exiting operator <. result: {result}");

            return result;
        }

        public static bool operator ==(LargeInteger left, LargeInteger right)
        {
            LargeInteger.Logger.Debug($"Entering operator ==. left: {left}, right: {right}");

            left.Ensure(ref right);
            bool result = left.data.Count == right.data.Count && LargeInteger.CompareSameLength(left, right) == 0;

            LargeInteger.Logger.Debug($"Exiting operator ==. result: {result}");
            return result;
        }

        public static bool operator !=(LargeInteger left, LargeInteger right)
        {
            LargeInteger.Logger.Debug($"Entering operator !=. left: {left}, right: {right}");

            left.Ensure(ref right);
            bool result = !(left == right);

            LargeInteger.Logger.Debug($"Exiting operator !=. result: {result}");
            return result;
        }

        public static bool operator >=(LargeInteger left, LargeInteger right)
        {
            LargeInteger.Logger.Debug($"Entering operator >=. left: {left}, right: {right}");

            bool result = left > right || left == right;

            LargeInteger.Logger.Debug($"Exiting operator >=. result: {result}");
            return result;
        }

        public static bool operator <=(LargeInteger left, LargeInteger right)
        {
            LargeInteger.Logger.Debug($"Entering operator <=. left: {left}, right: {right}");

            bool result = left < right || left == right;

            LargeInteger.Logger.Debug($"Exiting operator <=. result: {result}");
            return result;
        }

        public override bool Equals(object obj)
        {
            LargeInteger.Logger.Debug($"Entering Equals. this: {this}, obj: {obj}");

            this.Ensure();

            if (!(obj is LargeInteger))
            {
                return false;
            }

            var largeInteger = (LargeInteger)obj;
            largeInteger.Ensure();

            bool result = this == largeInteger;

            LargeInteger.Logger.Debug($"Entering Equals. result: {result}");
            return result;
        }

        public override int GetHashCode()
        {
            LargeInteger.Logger.Debug("Entering GetHashCode");

            this.Ensure();
            int result = this.data.GetHashCode();

            LargeInteger.Logger.Debug($"Exiting GetHashCode. result: {result}");
            return result;
        }

        public static LargeInteger operator ++(LargeInteger largeInteger)
        {
            LargeInteger.Logger.Debug($"Entering operator ++. largeInteger: {largeInteger}");

            largeInteger.Ensure();

            LargeInteger result = largeInteger.Clone();

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

            LargeInteger.Logger.Debug($"Exiting operator ++. result: {result}");
            return result;
        }

        public static LargeInteger operator --(LargeInteger largeInteger)
        {
            LargeInteger.Logger.Debug($"Entering operator --. largeInteger: {largeInteger}");

            largeInteger.Ensure();

            LargeInteger result = largeInteger.Clone();            

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

            LargeInteger.Logger.Debug($"Exiting operator --. result: {result}");
            return result;
        }

        public LargeInteger Pow(ulong power)
        {
            LargeInteger.Logger.Debug($"Entering Pow. this: {this}, power: {power}");

            this.Ensure();

            if (this == 0 && power == 0)
            {
                throw new DivideByZeroException("LargeInteger: divide by zero");
            }

            var result = new LargeInteger(1);

            for (var i = 0uL; i < power; i++)
            {
                result *= this;
            }

            LargeInteger.Logger.Debug($"Exiting Pow. result: {result}");
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

            ulong result = LargeInteger.GetULong(largeInteger.data);

            return result;
        }

        private static ulong GetULong(IList<uint> input)
        {
            LargeInteger.Logger.Debug($"Entering GetULong. input: {input}");

            ulong result = input[0] + (input.Count == 2 ? (ulong)input[1] << 32 : 0);

            return result;
        }
    }
}
