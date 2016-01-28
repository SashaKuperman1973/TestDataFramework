using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            Assert.AreEqual(0uL, (ulong)largeInteger);
        }

        [TestMethod]
        public void Increment_Test()
        {
            var largeInteger = new LargeInteger();

            for (ulong i = 1; i <= LargeIntegerTests.MaxValue; i++)
            {
                largeInteger++;
                Assert.AreEqual(i, (ulong)largeInteger);
            }
        }

        [TestMethod]
        public void Decrement_Test()
        {
            var largeInteger = new LargeInteger(LargeIntegerTests.MaxValue);

            for (int i = (int)LargeIntegerTests.MaxValue - 1; i >= 0; i--)
            {
                largeInteger--;
                Assert.AreEqual((ulong)i, (ulong)largeInteger);
            }
        }

        [TestMethod]
        public void Addition_Test()
        {
            var left = new LargeInteger(LargeIntegerTests.MaxValue);
            var right = new LargeInteger(257);

            LargeInteger result = left + right;

            Assert.AreEqual(LargeIntegerTests.MaxValue + 257, (ulong)result);
        }

        [TestMethod]
        public void Subtraction_Test()
        {
            var left = new LargeInteger(LargeIntegerTests.MaxValue);
            var right = new LargeInteger(LargeIntegerTests.MaxValue / 2);

            LargeInteger result = left - right;

            Assert.AreEqual(LargeIntegerTests.MaxValue - LargeIntegerTests.MaxValue / 2, (ulong)result);
        }

        [TestMethod]
        public void Multiplication_Test()
        {
            throw new NotImplementedException();            
        }

        [TestMethod]
        public void Exponent_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Division_Test()
        {
            var largeInteger = new LargeInteger(LargeIntegerTests.MaxValue);

            LargeInteger result = largeInteger/23;

            Assert.AreEqual(LargeIntegerTests.MaxValue/23, (ulong)result);
        }

        [TestMethod]
        public void Modulus_Test()
        {
            var largeInteger = new LargeInteger(LargeIntegerTests.MaxValue);

            LargeInteger result = largeInteger%23;

            Assert.AreEqual(LargeIntegerTests.MaxValue % 23, (ulong)result);
        }

        [TestMethod]
        public void Cast_ToULong_Test()
        {
            var largeInteger = new LargeInteger(LargeIntegerTests.MaxValue);

            var result = (ulong)largeInteger;

            Assert.AreEqual(LargeIntegerTests.MaxValue, result);
        }

        [TestMethod]
        public void Cast_FromULong_Test()
        {
            ulong value = LargeIntegerTests.MaxValue;

            var result = (LargeInteger)value;

            result--;

            Assert.AreEqual(value - 1, (ulong)result);
        }

        [TestMethod]
        public void Initializer_Test()
        {
            ulong value = LargeIntegerTests.MaxValue;

            var result = new LargeInteger(value);

            result--;

            Assert.AreEqual(value - 1, (ulong)result);
        }

        [TestMethod]
        public void LessThan_Test()
        {
            var lessThan = new LargeInteger(LargeIntegerTests.MaxValue- LargeIntegerTests.MaxValue/2);
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
    }
}
