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

            int result1 = this.valueProvider.GetInteger(null, null);
            int result2 = this.valueProvider.GetInteger(null, null);

            // Assert

            Assert.AreEqual((int) this.initialCount, result1);
            Assert.AreEqual((int) this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetLongInteger_Test()
        {
            // Act

            long result1 = this.valueProvider.GetLongInteger(null, null);
            long result2 = this.valueProvider.GetLongInteger(null, null);

            // Assert

            Assert.AreEqual((long) this.initialCount, result1);
            Assert.AreEqual((long) this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetShortInteger_Test()
        {
            // Act

            short result1 = this.valueProvider.GetShortInteger(null, null);
            short result2 = this.valueProvider.GetShortInteger(null, null);

            // Assert

            Assert.AreEqual((short) this.initialCount, result1);
            Assert.AreEqual((short) this.initialCount + 1, result2);
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

            decimal result1 = this.valueProvider.GetDecimal(null, null, null);
            decimal result2 = this.valueProvider.GetDecimal(null, null, null);

            // Assert

            Assert.AreEqual(this.initialCount, result1);
            Assert.AreEqual((decimal) this.initialCount + 1, result2);
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

            DateTime result1 = this.valueProvider.GetDateTime(null, null, null, null);
            DateTime result2 = this.valueProvider.GetDateTime(null, null, null, null);

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

            Assert.AreEqual((byte) this.initialCount, result1);
            Assert.AreEqual((byte) this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetDouble_Test()
        {
            // Act

            double result1 = this.valueProvider.GetDouble(null, null, null);
            double result2 = this.valueProvider.GetDouble(null, null, null);

            // Assert

            Assert.AreEqual(this.initialCount, result1);
            Assert.AreEqual((double) this.initialCount + 1, result2);
        }

        [TestMethod]
        public void GetFloat_Test()
        {
            // Act

            float result1 = this.valueProvider.GetFloat(null, null, null);
            float result2 = this.valueProvider.GetFloat(null, null, null);

            // Assert

            Assert.AreEqual(this.initialCount, result1);
            Assert.AreEqual((float) this.initialCount + 1, result2);
        }

        [TestMethod]
        public void Count_Overflow_Test()
        {
            this.valueProvider.Count = int.MaxValue - 10;

            for (int i = int.MaxValue - 10; i <= int.MaxValue - 1; i++)
            {
                int result = this.valueProvider.GetInteger(null, null);
                Assert.AreEqual(i, result);
            }

            Assert.AreEqual(1, this.valueProvider.GetInteger(null, null));
        }

        [TestMethod]
        public void ByteCount_Overflow_Test()
        {
            this.valueProvider.ByteCount = byte.MaxValue - 10;

            for (int i = byte.MaxValue - 10; i <= byte.MaxValue; i++)
            {
                int result = this.valueProvider.GetByte();
                Assert.AreEqual(i, result);
            }

            Assert.AreEqual(1, this.valueProvider.GetByte());
        }

        [TestMethod]
        public void GetEmailAddress_Throws_Test()
        {
            Helpers.ExceptionTest(() => this.valueProvider.GetEmailAddress(), typeof(NotSupportedException));
        }

        [TestMethod]
        public void GetEnum_Throws_Test()
        {
            Helpers.ExceptionTest(() => this.valueProvider.GetEnum(null), typeof(NotImplementedException));
        }
    }
}