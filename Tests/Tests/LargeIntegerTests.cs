﻿/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;

namespace Tests.Tests
{
    [TestClass]
    public class LargeIntegerTests
    {
        private const ulong MaxValue = 600;

        [TestMethod]
        public void DefaultInitialValue_Test()
        {
            var largeInteger = new LargeInteger();

            Assert.AreEqual(0uL, (ulong) largeInteger);
        }

        [TestMethod]
        public void Increment_Test()
        {
            var largeInteger = new LargeInteger();

            for (ulong i = 1; i <= LargeIntegerTests.MaxValue; i++)
            {
                largeInteger++;
                Assert.AreEqual(i, (ulong) largeInteger);
            }

            largeInteger = 0xffffffff;
            largeInteger++;

            Assert.AreEqual("4294967296", largeInteger.ToString());
        }

        [TestMethod]
        public void Decrement_Test()
        {
            var largeInteger = new LargeInteger(LargeIntegerTests.MaxValue);

            for (int i = (int) LargeIntegerTests.MaxValue - 1; i >= 0; i--)
            {
                largeInteger--;
                Assert.AreEqual((ulong) i, (ulong) largeInteger);
            }
        }

        [TestMethod]
        public void Borrow_When_Decrement_Test()
        {
            var largeInteger = new LargeInteger(0x100000000);
            LargeInteger result = --largeInteger;

            Assert.AreEqual(new LargeInteger(0xffffffff), result);
        }

        [TestMethod]
        public void Decrement_Underflow_Test()
        {
            var largeInteger = new LargeInteger(0);

            Helpers.ExceptionTest(() => { largeInteger--; },
                typeof(OverflowException),
                Messages.Underflow);
        }

        [TestMethod]
        public void Subtract_Underflow_Test()
        {
            var largeInteger = new LargeInteger(5);

            Helpers.ExceptionTest(() =>
                {
                    LargeInteger placeHolder = largeInteger - 6;
                },
                typeof(OverflowException),
                Messages.LargeIntegerUnderFlow
            );
        }

        [TestMethod]
        public void Addition_Test()
        {
            var left = new LargeInteger(LargeIntegerTests.MaxValue);
            var right = new LargeInteger(257);

            LargeInteger result = left + right;

            Assert.AreEqual(LargeIntegerTests.MaxValue + 257, (ulong) result);
        }

        [TestMethod]
        public void Subtraction_Test()
        {
            var left = new LargeInteger(LargeIntegerTests.MaxValue);
            var right = new LargeInteger(LargeIntegerTests.MaxValue / 2);

            LargeInteger result = left - right;

            Assert.AreEqual(LargeIntegerTests.MaxValue - LargeIntegerTests.MaxValue / 2, (ulong) result);
        }

        [TestMethod]
        public void Multiplication_Test()
        {
            var a = new LargeInteger(ulong.MaxValue);
            LargeInteger b = a * new LargeInteger(ulong.MaxValue);

            LargeInteger c = b / a;

            Assert.AreEqual(ulong.MaxValue, (ulong) c);
        }

        [TestMethod]
        public void Exponent_Test()
        {
            LargeInteger exponent = new LargeInteger(ulong.MaxValue).Pow(3);
            LargeInteger actual = exponent / new LargeInteger(ulong.MaxValue);
            LargeInteger a = new LargeInteger(ulong.MaxValue) * new LargeInteger(ulong.MaxValue);
            actual /= new LargeInteger(ulong.MaxValue);

            var expected = new LargeInteger(ulong.MaxValue);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Division_LargeDenominator_Test()
        {
            LargeInteger a = new LargeInteger(ulong.MaxValue).Pow(7);
            LargeInteger b = new LargeInteger(ulong.MaxValue).Pow(5);

            LargeInteger result = a / b;
            Assert.AreEqual(new LargeInteger(ulong.MaxValue).Pow(2), result);
        }

        [TestMethod]
        public void Division_Test()
        {
            LargeInteger numerator = new LargeInteger(ulong.MaxValue) * 35 * 14;
            LargeInteger denominator = new LargeInteger(ulong.MaxValue) * 35;

            LargeInteger quotient = numerator / denominator;

            Assert.AreEqual((ulong) 14, (ulong) quotient);
        }

        [TestMethod]
        public void Division_Test2()
        {
            var a = new LargeInteger(ulong.MaxValue);
            LargeInteger b = a * (new LargeInteger(ulong.MaxValue) * 9000);

            LargeInteger c = b / (a * 9000);

            Assert.AreEqual(ulong.MaxValue, (ulong) c);
        }

        [TestMethod]
        public void Division_OneElementDenominator_Test()
        {
            const ulong expected = (ulong) (ulong.MaxValue * (3m / 4));

            LargeInteger largeInteger = new LargeInteger(ulong.MaxValue) * 3;
            LargeInteger result = largeInteger / 4;

            Assert.AreEqual(expected, (ulong) result);
        }

        [TestMethod]
        public void Division_OneElementNumerator_Test()
        {
            const ulong expected = (ulong) 48648;

            var largeInteger = new LargeInteger(48648 * 75);
            LargeInteger result = largeInteger / 75;

            Assert.AreEqual(expected, (ulong) result);
        }

        [TestMethod]
        public void Modulus_Test()
        {
            var largeInteger = new LargeInteger(LargeIntegerTests.MaxValue);

            LargeInteger result = largeInteger % 23;

            Assert.AreEqual(LargeIntegerTests.MaxValue % 23, (ulong) result);
        }

        [TestMethod]
        public void Cast_ToULong_Test()
        {
            var largeInteger = new LargeInteger(LargeIntegerTests.MaxValue);

            ulong result = (ulong) largeInteger;

            Assert.AreEqual(LargeIntegerTests.MaxValue, result);
        }

        [TestMethod]
        public void Cast_FromULong_Test()
        {
            ulong value = LargeIntegerTests.MaxValue;

            var result = (LargeInteger) value;

            result--;

            Assert.AreEqual(value - 1, (ulong) result);
        }

        [TestMethod]
        public void Initializer_Test()
        {
            ulong value = LargeIntegerTests.MaxValue;

            var result = new LargeInteger(value);

            result--;

            Assert.AreEqual(value - 1, (ulong) result);
        }

        [TestMethod]
        public void LessThan_Test()
        {
            var lessThan = new LargeInteger(LargeIntegerTests.MaxValue - LargeIntegerTests.MaxValue / 2);
            var greaterThan = new LargeInteger(LargeIntegerTests.MaxValue);

            Assert.IsTrue(lessThan < greaterThan);
        }

        [TestMethod]
        public void GreaterThan_Test()
        {
            var lessThan = new LargeInteger(LargeIntegerTests.MaxValue - LargeIntegerTests.MaxValue / 2);
            var greaterThan = new LargeInteger(LargeIntegerTests.MaxValue);

            Assert.IsTrue(greaterThan > lessThan);
        }

        [TestMethod]
        public void ToString_Test()
        {
            LargeInteger largeInteger = new LargeInteger(uint.MaxValue) * uint.MaxValue * uint.MaxValue;

            string result = largeInteger.ToString();
            const string expected = "79228162458924105385300197375";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ToString_Zero_Test()
        {
            LargeInteger largeInteger = 0;

            string result = largeInteger.ToString();

            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public void Less_Than_Or_Equal_To_Test()
        {
            var left = new LargeInteger(5);
            var equalRight = new LargeInteger(5);

            bool result = left <= equalRight;
            Assert.IsTrue(result);

            var greaterRight = new LargeInteger(6);
            result = left <= greaterRight;
            Assert.IsTrue(result);

            var lesserRight = new LargeInteger(4);
            result = left <= lesserRight;
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Greater_Than_Or_Equal_To_Test()
        {
            var left = new LargeInteger(5);
            var equalRight = new LargeInteger(5);

            bool result = left >= equalRight;
            Assert.IsTrue(result);

            var lesserRight = new LargeInteger(4);
            result = left >= lesserRight;
            Assert.IsTrue(result);

            var greaterRight = new LargeInteger(6);
            result = left >= greaterRight;
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UnEqual_Test()
        {
            var left = new LargeInteger(5);
            var unequalRight = new LargeInteger(6);

            bool result = left != unequalRight;
            Assert.IsTrue(result);

            var equalRight = new LargeInteger(5);
            result = left != equalRight;
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_Test()
        {
            var a = new LargeInteger(1234);
            var b = new LargeInteger(1234);

            Assert.AreEqual(a, b);

            var c = new LargeInteger(12345);
            Assert.AreNotEqual(a, c);

            var objectRight = new object();
            Assert.AreNotEqual(a, objectRight);
        }

        [TestMethod]
        public void Equality_Symbol_Test()
        {
            var left = new LargeInteger(1234);
            var right = new LargeInteger(1234);

            bool result = left == right;
            Assert.IsTrue(result);

            var unequalRight = new LargeInteger(4321);
            result = left == unequalRight;
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetHashCode_Test()
        {
            var a = new LargeInteger(1234);

            int firstHashCode = a.GetHashCode();
            int secondHashCode = a.GetHashCode();

            Assert.AreEqual(firstHashCode, secondHashCode);

            a++;
            int newHashCode = a.GetHashCode();
            Assert.AreNotEqual(firstHashCode, newHashCode);
        }

        [TestMethod]
        public void Pow_DivideByZero_Test()
        {
            var a = new LargeInteger(0);

            Helpers.ExceptionTest(() => a.Pow(0), typeof(DivideByZeroException));
        }

        [TestMethod]
        public void Pow_Test()
        {
            ulong expectedLong = (ulong) Math.Pow(4, 12);
            var expected = new LargeInteger(expectedLong);

            var @base = new LargeInteger(4);
            LargeInteger exponent = @base.Pow(12);

            Assert.AreEqual(expected, exponent);
        }

        [TestMethod]
        public void Divide_DivisionByZero()
        {
            var a = new LargeInteger(5);
            var b = new LargeInteger(0);
         
            Helpers.ExceptionTest(() => a.Divide(b), typeof(DivideByZeroException));
        }

        [TestMethod]
        public void Divide_Operator_DivisionByZero()
        {
            var a = new LargeInteger(5);
            var b = new LargeInteger(0);

            Helpers.ExceptionTest(() =>
            {
                var x = a / b;
            }, typeof(DivideByZeroException));
        }

        [TestMethod]
        public void Modulus_Operator_DivisionByZero()
        {
            var a = new LargeInteger(5);
            var b = new LargeInteger(0);

            Helpers.ExceptionTest(() =>
            {
                var x = a % b;
            }, typeof(DivideByZeroException));
        }

        [TestMethod]
        public void Test_When_Adding_Placeholder_In_Quotient_During_Long_Division()
        {
            LargeInteger numberBase = new LargeInteger(uint.MaxValue) + 1;

            LargeInteger a = 5 * numberBase.Pow(2) + 3 * numberBase + 7;

            LargeInteger quotient = a / 5;

            LargeInteger originalNumber = quotient * 5;

            Assert.AreEqual(a, originalNumber);
        }
    }
}