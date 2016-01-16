using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Helpers;

namespace Tests.Tests
{
    [TestClass]
    public class LargeIntegerTests
    {
        [TestMethod]
        public void Increment_Test()
        {
            var largeInteger = new LargeInteger();

            for (ulong i = 0; i < ulong.MaxValue; i++)
            {
                Assert.AreEqual(i, (ulong)largeInteger++);
            }
        }

        [TestMethod]
        public void Addition_Test()
        {
            var left = new LargeInteger(ulong.MaxValue / 2);
            var right = new LargeInteger(257);

            LargeInteger result = left + right;

            Assert.AreEqual(ulong.MaxValue / 2 + 257, (ulong)result);
        }

        [TestMethod]
        public void Subtraction_Test()
        {
            var left = new LargeInteger(ulong.MaxValue / 2);
            var right = new LargeInteger(257);

            LargeInteger result = left - right;

            Assert.AreEqual(ulong.MaxValue / 2 - 257, (ulong)result);
        }

        [TestMethod]
        public void Division_Test()
        {
            var largeInteger = new LargeInteger(ulong.MaxValue / 2);

            LargeInteger result = largeInteger/23;

            Assert.AreEqual(2uL, (ulong)result);
        }

        [TestMethod]
        public void Modulus_Test()
        {
            var largeInteger = new LargeInteger(long.MaxValue / 2);

            LargeInteger result = largeInteger % 23;

            Assert.AreEqual(15uL, (ulong)result);
        }

        [TestMethod]
        public void Cast_ToULong_Test()
        {
            var largeInteger = new LargeInteger(ulong.MaxValue);

            var result = (ulong) largeInteger;

            Assert.AreEqual(ulong.MaxValue, result);
        }
    }
}
