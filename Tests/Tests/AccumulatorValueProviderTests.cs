using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Helpers;
using TestDataFramework.ValueProvider;

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
    }
}
