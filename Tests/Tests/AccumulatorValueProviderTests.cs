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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Helpers;
using TestDataFramework.ValueProvider.Concrete;

namespace Tests.Tests
{
    [TestClass]
    public class AccumulatorValueProviderTests
    {
        private readonly ulong initialCount = Helper.DefaultInitalCount;
        private AccumulatorValueProvider valueProvider;

        [TestInitialize]
        public void Initialize()
        {
            this.valueProvider = new AccumulatorValueProvider();
        }

        [TestMethod]
        public void GetInteger_Test()
        {
            // Act

            var result1 = this.valueProvider.GetInteger(null);
            var result2 = this.valueProvider.GetInteger(null);

            // Assert

            Assert.AreEqual((int) this.initialCount, result1);
            Assert.AreEqual((int) this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetLongInteger_Test()
        {
            // Act

            var result1 = this.valueProvider.GetLongInteger(null);
            var result2 = this.valueProvider.GetLongInteger(null);

            // Assert

            Assert.AreEqual((long) this.initialCount, result1);
            Assert.AreEqual((long) this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetShortInteger_Test()
        {
            // Act

            var result1 = this.valueProvider.GetShortInteger(null);
            var result2 = this.valueProvider.GetShortInteger(null);

            // Assert

            Assert.AreEqual((short) this.initialCount, result1);
            Assert.AreEqual((short) this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetString_Test()
        {
            // Act

            var result1 = this.valueProvider.GetString(5);
            var result2 = this.valueProvider.GetString(7);

            // Assert

            Assert.AreEqual("B++++", result1);
            Assert.AreEqual("C++++++", result2);
        }

        [TestMethod]
        public void GetString_DefaultLength_Test()
        {
            var result = this.valueProvider.GetString(null);

            Assert.AreEqual("B+++++++++", result);
        }

        [TestMethod]
        public void GetCharacter_Test()
        {
            // Act

            var result1 = this.valueProvider.GetCharacter();
            var result2 = this.valueProvider.GetCharacter();

            // Assert

            Assert.AreEqual('!', result1);
            Assert.AreEqual('"', result2);
        }

        [TestMethod]
        public void GetDecimal_Test()
        {
            // Act

            var result1 = this.valueProvider.GetDecimal(null);
            var result2 = this.valueProvider.GetDecimal(null);

            // Assert

            Assert.AreEqual(this.initialCount, result1);
            Assert.AreEqual((decimal) this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetBoolean_Test()
        {
            // Act

            var result1 = this.valueProvider.GetBoolean();
            var result2 = this.valueProvider.GetBoolean();
            var result3 = this.valueProvider.GetBoolean();

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

            var result1 = this.valueProvider.GetByte();
            var result2 = this.valueProvider.GetByte();

            // Assert

            Assert.AreEqual((byte) this.initialCount + 1, result1);
            Assert.AreEqual((byte) this.initialCount + 2, result2);
        }

        [TestMethod]
        public void GetDouble_Test()
        {
            // Act

            var result1 = this.valueProvider.GetDouble(null);
            var result2 = this.valueProvider.GetDouble(null);

            // Assert

            Assert.AreEqual(this.initialCount, result1);
            Assert.AreEqual((double) this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetFloat_Test()
        {
            // Act

            var result1 = this.valueProvider.GetFloat(null);
            var result2 = this.valueProvider.GetFloat(null);

            // Assert

            Assert.AreEqual(this.initialCount, result1);
            Assert.AreEqual((float) this.initialCount + 1, result2);
        }
    }
}