/*
    Copyright 2016 Alexander Kuperman

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
using System.Data.SqlTypes;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.ValueProvider.Concrete;
using TestDataFramework.ValueProvider.Interfaces;

namespace Tests.Tests
{
    [TestClass]
    public class StandardRandomizerTests
    {
        private StandardRandomizer randomizer;
        private Mock<Random> randomMock;
        private Mock<IRandomSymbolStringGenerator> stringGeneratorMock;
        private readonly DateTime now = DateTime.Now;
        private Func<long> randomizeLongInteger;


        private const int Integer = 5;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.randomMock = new Mock<Random>();
            this.stringGeneratorMock = new Mock<IRandomSymbolStringGenerator>();
            this.randomizer = new StandardRandomizer(this.randomMock.Object, this.stringGeneratorMock.Object,
                () => this.now, SqlDateTime.MinValue.Value.Ticks, SqlDateTime.MaxValue.Value.Ticks);

            this.randomizeLongInteger =
                () => this.randomizer.GetLongInteger(SqlDateTime.MaxValue.Value.Ticks - this.now.Ticks);
        }

        [TestMethod]
        public void GetInteger_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.Is<int>(i => i == int.MaxValue))).Returns(StandardRandomizerTests.Integer).Verifiable();

            // Act

            int result = this.randomizer.GetInteger(null);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(StandardRandomizerTests.Integer, result);
        }

        [TestMethod]
        public void GetInteger_WithMax_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(StandardRandomizerTests.Integer);
            const int maximum = 7;

            // Act

            int result = this.randomizer.GetInteger(maximum);

            // Assert

            Assert.AreEqual(StandardRandomizerTests.Integer, result);
            this.randomMock.Verify(m => m.Next(), Times.Never());
            this.randomMock.Verify(m => m.Next(It.Is<int>(max => max == maximum)), Times.Once());
        }

        [TestMethod]
        public void GetLongInteger_Word0Max_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(0x10000)).Returns(StandardRandomizerTests.Integer).Verifiable();

            // Act

            long result = this.randomizer.GetLongInteger(0x10000);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual((long)StandardRandomizerTests.Integer, result);
        }


        [TestMethod]
        public void GetLongInteger_Test()
        {
            // Arrange

            long maxValue = long.MaxValue - new Random().Next() - 1;
            long expected = maxValue - new Random().Next();

            if ((maxValue & 0xffff) == 0)
            {
                maxValue--;
            }

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 0)) & 0xffff)))
                .Returns((int)((expected >> (16 * 0)) & 0xffff)).Verifiable();

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 1)) & 0xffff) + 1))
                .Returns((int)((expected >> (16 * 1)) & 0xffff)).Verifiable();

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 2)) & 0xffff) + 1))
                .Returns((int)((expected >> (16 * 2)) & 0xffff)).Verifiable();

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 3)) & 0xffff) + 1))
                .Returns((int)((expected >> (16 * 3)) & 0xffff)).Verifiable();

            // Act

            long result = this.randomizer.GetLongInteger(maxValue);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetLongInteger_DefaultMax_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(1);

            // Act

            long result = this.randomizer.GetLongInteger(null);

            // Assert

            this.randomMock.Verify(m => m.Next(0xffff), Times.Once);
            this.randomMock.Verify(m => m.Next(0x10000), Times.Exactly(2));
            this.randomMock.Verify(m => m.Next(0x8000), Times.Once);

            const long expected = 0x0001000100010001;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetLongInteger_MaxReturnBoundaryCondition_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(0xffff)).Returns(0xffff);
            this.randomMock.Setup(m => m.Next(0x10000)).Returns(0xffff);
            this.randomMock.Setup(m => m.Next(0x8000)).Returns(0x7fff);

            // Act

            long result = this.randomizer.GetLongInteger(null);

            // Assert

            Assert.AreEqual(long.MaxValue, result);
        }

        [TestMethod]
        public void GetShortInteger_Test()
        {
            // Arrange

            const int max = 7;
            this.randomMock.Setup(m => m.Next(It.Is<int>(i => i == max))).Returns(StandardRandomizerTests.Integer).Verifiable();

            // Act

            short result = this.randomizer.GetShortInteger(max);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual((short)StandardRandomizerTests.Integer, result);
        }

        [TestMethod]
        public void GetShortInteger_DefaultMax_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.Is<int>(i => i == short.MaxValue))).Returns(StandardRandomizerTests.Integer).Verifiable();

            // Act

            short result = this.randomizer.GetShortInteger(null);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual((short)StandardRandomizerTests.Integer, result);
        }

        [TestMethod]
        public void GetString_Test()
        {
            // Arrange

            const string returnValue = "ABCD";
            this.stringGeneratorMock.Setup(m => m.GetRandomString(It.Is<int?>(i => i == 5))).Returns(returnValue);

            // Act

            string result = this.randomizer.GetString(5);

            // Assert

            Assert.AreEqual(returnValue, result);
        }

        [TestMethod]
        public void GetCharacter_Test()
        {
            // 26 letters in the alphabet
            for (int code = 0; code < 26; code++)
            {
                this.randomMock.Setup(m => m.Next(It.Is<int>(i => i == 26))).Returns(code);

                char result = this.randomizer.GetCharacter();

                // 65 is ASCII code of "A".
                Assert.AreEqual((char)(code + 65), result);
            }
        }

        [TestMethod]
        public void GetDecimal_DefaultPrecision_Test()
        {
            // Arrange

            const decimal expected = 12345.12m;

            this.randomMock.Setup(m => m.Next(int.MaxValue)).Returns(12345);
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.12345d);

            // Act

            decimal result = this.randomizer.GetDecimal(null);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetDecimal_Test()
        {
            // Arrange

            const decimal expected = 12345.1234m;

            this.randomMock.Setup(m => m.Next(int.MaxValue)).Returns(12345);
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.12345d);

            // Act

            decimal result = this.randomizer.GetDecimal(4);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetDouble_DefaultPrecision_Test()
        {
            // Arrange

            const double expected = 12345.12d;

            this.randomMock.Setup(m => m.Next(int.MaxValue)).Returns(12345);
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.12345d);

            // Act

            double result = this.randomizer.GetDouble(null);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetDouble_Test()
        {
            // Arrange

            const double expected = 12345.1234d;

            this.randomMock.Setup(m => m.Next(int.MaxValue)).Returns(12345);
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.12345d);

            // Act

            double result = this.randomizer.GetDouble(4);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetBoolean_ReturnsTrue_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(2)).Returns(1).Verifiable();

            // Act

            bool result = this.randomizer.GetBoolean();

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void GetBoolean_ReturnsFalse_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(2)).Returns(0).Verifiable();

            // Act

            bool result = this.randomizer.GetBoolean();

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void GetDateTime_Past_Test()
        {
            // Arrange

            long ticks = this.randomizeLongInteger();

            DateTime expected = this.now.AddTicks(-ticks);

            // Act

            DateTime explicitPastResult = this.randomizer.GetDateTime(PastOrFuture.Past, x => ticks);
            DateTime implicitPastResult = this.randomizer.GetDateTime(null, x => ticks);

            // Assert

            Assert.AreEqual(expected, explicitPastResult);
            Assert.AreEqual(expected, implicitPastResult);
        }

        [TestMethod]
        public void GetDateTime_Future_Test()
        {
            // Arrange

            long ticks = this.randomizeLongInteger();

            DateTime expected = this.now.AddTicks(ticks);

            // Act

            DateTime result = this.randomizer.GetDateTime(PastOrFuture.Future, x => ticks);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetByte_Test()
        {
            // Arrange

            const byte expected = 5;
            this.randomMock.Setup(m => m.NextBytes(It.IsAny<byte[]>())).Callback<byte[]>(a => a[0] = expected);

            // Act

            byte result = this.randomizer.GetByte();

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetEmailAddress_Test()
        {
            // Arrange

            this.stringGeneratorMock.Setup(m => m.GetRandomString(It.IsAny<int?>())).Returns("abcde");

            // Act

            string result = this.randomizer.GetEmailAddress();

            // Assert

            Assert.AreEqual("abcde@domain.com", result);
        }

        [TestMethod]
        public void GetFloat_DefaultPrecision_Test()
        {
            // Arrange

            const float expected = 12345.12f;

            this.randomMock.Setup(m => m.Next(100000)).Returns(12345).Verifiable();
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.12345d);

            // Act

            float result = this.randomizer.GetFloat(null);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetFloat_Test()
        {
            // Arrange

            const float expected = 123.1234f;

            this.randomMock.Setup(m => m.Next(1000)).Returns(123).Verifiable();
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.12345d);

            // Act

            float result = this.randomizer.GetFloat(4);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetFloat_MaximumPrecision_Test()
        {
            // Arrange

            const float expected = 0.1234567f;

            this.randomMock.Setup(m => m.Next(1)).Returns(0).Verifiable();
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.1234567d);

            // Act

            float result = this.randomizer.GetFloat(7);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetFloat_MinimumPrecision_Test()
        {
            // Arrange

            const float expected = 1234567f;

            this.randomMock.Setup(m => m.Next(10000000)).Returns(1234567);
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.12345d);

            // Act

            float result = this.randomizer.GetFloat(0);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetFloat_PrecisionOverflow_Test()
        {
            // Act. Assert.

            Helpers.ExceptionTest(() => this.randomizer.GetFloat(8), typeof (ArgumentOutOfRangeException),
                new ArgumentOutOfRangeException("precision", 8, Messages.FloatPrecisionOutOfRange).Message);
        }
    }
}
