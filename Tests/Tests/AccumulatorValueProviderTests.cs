using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Helpers;
using TestDataFramework.ValueProvider.Concrete;

namespace Tests.Tests
{
    [TestClass]
    public class AccumulatorValueProviderTests
    {
        private AccumulatorValueProvider valueProvider;
        private readonly ulong initialCount = Helper.DefaultInitalCount;

        [TestInitialize]
        public void Initialize()
        {
            this.valueProvider = new AccumulatorValueProvider();
        }

        [TestMethod]
        public void GetInteger_Test()
        {
            // Act

            int result1 = this.valueProvider.GetInteger(null);
            int result2 = this.valueProvider.GetInteger(null);

            // Assert

            Assert.AreEqual((int)this.initialCount, result1);
            Assert.AreEqual((int)this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetLongInteger_Test()
        {
            // Act

            long result1 = this.valueProvider.GetLongInteger(null);
            long result2 = this.valueProvider.GetLongInteger(null);

            // Assert

            Assert.AreEqual((long)this.initialCount, result1);
            Assert.AreEqual((long)this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetShortInteger_Test()
        {
            // Act

            short result1 = this.valueProvider.GetShortInteger(null);
            short result2 = this.valueProvider.GetShortInteger(null);

            // Assert

            Assert.AreEqual((short)this.initialCount, result1);
            Assert.AreEqual((short)this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetString_Test()
        {
            // Act

            string result1 = this.valueProvider.GetString(5);
            string result2 = this.valueProvider.GetString(7);

            // Assert

            Assert.AreEqual("B++++", result1);
            Assert.AreEqual("C++++++", result2);
        }

        [TestMethod]
        public void GetString_DefaultLength_Test()
        {
            string result = this.valueProvider.GetString(null);

            Assert.AreEqual("B+++++++++", result);
        }

        [TestMethod]
        public void GetCharacter_Test()
        {
            // Act

            char result1 = this.valueProvider.GetCharacter();
            char result2 = this.valueProvider.GetCharacter();

            // Assert

            Assert.AreEqual('!', result1);
            Assert.AreEqual('"', result2);
        }

        [TestMethod]
        public void GetDecimal_Test()
        {
            // Act

            decimal result1 = this.valueProvider.GetDecimal(null);
            decimal result2 = this.valueProvider.GetDecimal(null);

            // Assert

            Assert.AreEqual((decimal)this.initialCount, result1);
            Assert.AreEqual((decimal)this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetBoolean_Test()
        {
            // Act

            bool result1 = this.valueProvider.GetBoolean();
            bool result2 = this.valueProvider.GetBoolean();
            bool result3 = this.valueProvider.GetBoolean();

            // Assert

            Assert.AreEqual(false, result1);
            Assert.AreEqual(true, result2);
            Assert.AreEqual(false, result3);
        }

        [TestMethod]
        public void GetDateTime_Test()
        {
            // Act

            DateTime result1 = this.valueProvider.GetDateTime(null, null);
            DateTime result2 = this.valueProvider.GetDateTime(null, null);

            // Assert

            Assert.AreEqual(DateTime.Now.Date.AddDays(1), result1.Date);
            Assert.AreEqual(DateTime.Now.Date.AddDays(2), result2.Date);
        }

        [TestMethod]
        public void GetByte_Test()
        {
            // Act

            byte result1 = this.valueProvider.GetByte();
            byte result2 = this.valueProvider.GetByte();

            // Assert

            Assert.AreEqual((byte)this.initialCount + 1, result1);
            Assert.AreEqual((byte)this.initialCount + 2, result2);
        }

        [TestMethod]
        public void GetDouble_Test()
        {
            // Act

            double result1 = this.valueProvider.GetDouble(null);
            double result2 = this.valueProvider.GetDouble(null);

            // Assert

            Assert.AreEqual((double)this.initialCount, result1);
            Assert.AreEqual((double)this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetFloat_Test()
        {
            // Act

            float result1 = this.valueProvider.GetFloat(null);
            float result2 = this.valueProvider.GetFloat(null);

            // Assert

            Assert.AreEqual((float)this.initialCount, result1);
            Assert.AreEqual((float)this.initialCount + 1, result2);
        }
    }
}
