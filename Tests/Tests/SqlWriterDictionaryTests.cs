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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.WritePrimitives.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class SqlWriterDictionaryTests
    {
        private SqlWriterDictionary writerDictionary;
        private Mock<LetterEncoder> encoderMock;
        private Mock<IWritePrimitives> primitivesMock;
        private Mock<SqlWriterCommandTextGenerator> commandGenerator;

        [TestInitialize]
        public void Initialize()
        {
            this.encoderMock = new Mock<LetterEncoder>();
            this.primitivesMock = new Mock<IWritePrimitives>();
            this.commandGenerator = new Mock<SqlWriterCommandTextGenerator>(null);

            this.writerDictionary = new SqlWriterDictionary(this.encoderMock.Object, this.primitivesMock.Object,
                this.commandGenerator.Object);
        }

        [TestMethod]
        public void Number_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(ClassWithIntAutoPrimaryKey).GetProperty("Key");

            const string command = "XXXX";
            this.commandGenerator.Setup(m => m.WriteNumber(propertyInfo)).Returns(command);

            var values = new object[] { (byte)1, (int)2, (short)3, (long)4 };

            foreach (object value in values)
            {
                // Act

                WriterDelegate numberWriter = this.writerDictionary[value.GetType()];
                DecoderDelegate numberDecoder = numberWriter(propertyInfo);
                LargeInteger result = numberDecoder(propertyInfo, value);

                // Assert

                Assert.AreEqual(new LargeInteger((ulong)Convert.ChangeType(value, typeof(ulong))), result);
                this.primitivesMock.Verify(m => m.AddSqlCommand(command));
            }
        }

        [TestMethod]
        public void String_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(ClassWithStringAutoPrimaryKey).GetProperty("Key");
            const string value = "X";
            var expected = new LargeInteger(7);

            const string command = "XXXX";
            this.commandGenerator.Setup(m => m.WriteString(propertyInfo)).Returns(command);

            this.encoderMock.Setup(m => m.Decode(value)).Returns(expected);

            // Act

            WriterDelegate stringWriter = this.writerDictionary[value.GetType()];
            DecoderDelegate stringDecoder = stringWriter(propertyInfo);
            LargeInteger result = stringDecoder(propertyInfo, value);

            // Assert

            Assert.AreEqual(expected, result);
            this.commandGenerator.Verify(m => m.WriteString(propertyInfo), Times.Once);
            this.primitivesMock.Verify(m => m.AddSqlCommand(command));
        }

        [TestMethod]
        public void Execute_Test()
        {
            // Act

            this.writerDictionary.Execute();

            // Assert

            this.primitivesMock.Verify(m => m.Execute(), Times.Once);
        }

        [TestMethod]
        public void NotANumberException_Test()
        {
            PropertyInfo propertyInfo = typeof(ClassWithIntAutoPrimaryKey).GetProperty("Key");
            const string input = "ABC";

            WriterDelegate stringWriter = this.writerDictionary[typeof(int)];
            DecoderDelegate decoder = stringWriter(propertyInfo);

            Helpers.ExceptionTest(() => decoder(propertyInfo, input), typeof(UnexpectedTypeException),
                string.Format(Messages.UnexpectedHandlerType, propertyInfo.GetExtendedMemberInfoString(), input));
        }

        [TestMethod]
        public void NotAStringException_Test()
        {
            PropertyInfo propertyInfo = typeof(ClassWithStringAutoPrimaryKey).GetProperty("Key");
            const int input = 5;

            WriterDelegate stringWriter = this.writerDictionary[typeof(string)];
            DecoderDelegate decoder = stringWriter(propertyInfo);

            Helpers.ExceptionTest(() => decoder(propertyInfo, input), typeof (UnexpectedTypeException),
                string.Format(Messages.UnexpectedHandlerType, propertyInfo, input));
        }
    }
}
