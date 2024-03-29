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
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator.Interfaces;
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
        private Mock<IAttributeDecorator> attributeDecoratorMock;
        private Mock<SqlWriterCommandTextGenerator> commandGeneratorMock;
        private Mock<SqlWriterCommandText> commandTextProviderMock;
        private Mock<LetterEncoder> encoderMock;
        private Mock<IWritePrimitives> primitivesMock;
        private SqlWriterDictionary writerDictionary;

        [TestInitialize]
        public void Initialize()
        {
            this.encoderMock = new Mock<LetterEncoder>();
            this.primitivesMock = new Mock<IWritePrimitives>();
            this.commandTextProviderMock = new Mock<SqlWriterCommandText>();
            this.commandGeneratorMock =
                new Mock<SqlWriterCommandTextGenerator>(this.attributeDecoratorMock,
                    this.commandTextProviderMock.Object);
            this.attributeDecoratorMock = new Mock<IAttributeDecorator>();

            this.writerDictionary = new SqlWriterDictionary(this.encoderMock.Object, this.primitivesMock.Object,
                this.commandGeneratorMock.Object);
        }

        [TestMethod]
        public void Number_Test()
        {
            // Arrange

            PropertyInfoProxy propertyInfo = typeof(ClassWithIntAutoPrimaryKey).GetPropertyInfoProxy("Key");

            const string command = "XXXX";
            this.commandGeneratorMock.Setup(m => m.WriteNumber(propertyInfo)).Returns(command);

            var values = new object[] {(byte) 1, 2, (short) 3, (long) 4};

            foreach (object value in values)
            {
                // Act

                WriterDelegate numberWriter = this.writerDictionary[value.GetType()];
                DecoderDelegate numberDecoder = numberWriter(propertyInfo);
                LargeInteger result = numberDecoder(propertyInfo, value);

                // Assert

                Assert.AreEqual(new LargeInteger((ulong) Convert.ChangeType(value, typeof(ulong))), result);
                this.primitivesMock.Verify(m => m.AddSqlCommand(command));
            }
        }

        [TestMethod]
        public void Number_Input_Is_DbNull_Test()
        {
            // Arrange

            PropertyInfoProxy propertyInfo = typeof(ClassWithIntAutoPrimaryKey).GetPropertyInfoProxy("Key");

            WriterDelegate numberWriter = this.writerDictionary[typeof(int)];
            DecoderDelegate numberDecoder = numberWriter(propertyInfo);

            // Act

            LargeInteger result = numberDecoder(propertyInfo, DBNull.Value);

            // Assert

            Assert.AreEqual((LargeInteger) 0, result);
        }

        [TestMethod]
        public void String_Input_Is_DbNull_Test()
        {
            // Arrange

            PropertyInfoProxy propertyInfo = typeof(SubjectClass).GetPropertyInfoProxy(nameof(SubjectClass.Text));

            WriterDelegate stringWriter = this.writerDictionary[typeof(string)];
            DecoderDelegate stringDecoder = stringWriter(propertyInfo);

            // Act

            LargeInteger result = stringDecoder(propertyInfo, DBNull.Value);

            // Assert

            Assert.AreEqual((LargeInteger) 0, result);
        }

        [TestMethod]
        public void UnKnownType_Throws()
        {
            Helpers.ExceptionTest(() =>
            {
                WriterDelegate x = this.writerDictionary[typeof(SubjectClass)];
            }, typeof(KeyNotFoundException), string.Format(Messages.PropertyKeyNotFound, typeof(SubjectClass)));
        }

        [TestMethod]
        public void String_Test()
        {
            // Arrange

            PropertyInfoProxy propertyInfo = typeof(ClassWithStringAutoPrimaryKey).GetPropertyInfoProxy("Key");
            const string value = "X";
            var expected = new LargeInteger(7);

            const string command = "XXXX";
            this.commandGeneratorMock.Setup(m => m.WriteString(propertyInfo)).Returns(command);

            this.encoderMock.Setup(m => m.Decode(value)).Returns(expected);

            // Act

            WriterDelegate stringWriter = this.writerDictionary[value.GetType()];
            DecoderDelegate stringDecoder = stringWriter(propertyInfo);
            LargeInteger result = stringDecoder(propertyInfo, value);

            // Assert

            Assert.AreEqual(expected, result);
            this.commandGeneratorMock.Verify(m => m.WriteString(propertyInfo), Times.Once);
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
            PropertyInfoProxy propertyInfo = typeof(ClassWithIntAutoPrimaryKey).GetPropertyInfoProxy("Key");
            const string input = "ABC";

            WriterDelegate stringWriter = this.writerDictionary[typeof(int)];
            DecoderDelegate decoder = stringWriter(propertyInfo);

            Helpers.ExceptionTest(() => decoder(propertyInfo, input), typeof(UnexpectedTypeException),
                string.Format(Messages.UnexpectedHandlerType, propertyInfo.GetExtendedMemberInfoString(), input));
        }

        [TestMethod]
        public void NotAStringException_Test()
        {
            PropertyInfoProxy propertyInfo = typeof(ClassWithStringAutoPrimaryKey).GetPropertyInfoProxy("Key");
            const int input = 5;

            WriterDelegate stringWriter = this.writerDictionary[typeof(string)];
            DecoderDelegate decoder = stringWriter(propertyInfo);

            Helpers.ExceptionTest(() => decoder(propertyInfo, input), typeof(UnexpectedTypeException),
                string.Format(Messages.UnexpectedHandlerType, propertyInfo, input));
        }
    }
}