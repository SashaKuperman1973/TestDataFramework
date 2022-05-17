/*
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
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class SqlClientInitialCountGeneratorTests
    {
        private Mock<LetterEncoder> encoderMock;
        private SqlClientInitialCountGenerator generator;
        private Mock<IWriterDictinary> writersMock;

        [TestInitialize]
        public void Initialize()
        {
            this.writersMock = new Mock<IWriterDictinary>();
            this.encoderMock = new Mock<LetterEncoder>();
            this.generator = new SqlClientInitialCountGenerator(this.writersMock.Object);
        }

        [TestMethod]
        public void FillData_Test()
        {
            // Arrange

            var returnValue1 = new LargeInteger(7);
            var returnValue2 = new LargeInteger(14);

            PropertyInfoProxy propertyInfo1 = typeof(ClassWithStringAutoPrimaryKey).GetPropertyInfoProxy("Key");
            PropertyInfoProxy propertyInfo2 = typeof(ClassWithIntAutoPrimaryKey).GetPropertyInfoProxy("Key");

            var data1 = new Data<LargeInteger>(null);
            var data2 = new Data<LargeInteger>(null);
            var dictionary = new Dictionary<PropertyInfoProxy, Data<LargeInteger>>
            {
                {propertyInfo1, data1},
                {propertyInfo2, data2}
            };

            WriterDelegate writer1 = writerPi => (decoderPi, input) => returnValue1;
            WriterDelegate writer2 = writerPi => (decoderPi, input) => returnValue2;

            this.writersMock.Setup(m => m[typeof(string)]).Returns(writer1);
            this.writersMock.Setup(m => m[typeof(int)]).Returns(writer2);

            this.writersMock.Setup(m => m.Execute()).Returns(new object[2]);

            // Act

            this.generator.FillData(dictionary);

            // Assert

            Assert.AreEqual(returnValue1 + 1, data1.Item);
            Assert.AreEqual(returnValue2 + 1, data2.Item);
        }

        [TestMethod]
        public void FillData_PropertyDataDictionary_Is_Empty()
        {
            // Arrange

            var dictionary = new Dictionary<PropertyInfoProxy, Data<LargeInteger>>();

            // Act

            this.generator.FillData(dictionary);

            // Assert

            this.writersMock.Verify(m => m[It.IsAny<Type>()], Times.Never);

            this.writersMock.Verify(m => m.Execute(), Times.Never);
        }

        [TestMethod]
        public void FillData_InternalError_InputVsDbContent_Count_Mismatch()
        {
            // Arrange

            var dictionary = new Dictionary<PropertyInfoProxy, Data<LargeInteger>>();

            PropertyInfoProxy property = typeof(SubjectClass).GetPropertyInfoProxy(nameof(SubjectClass.Integer));
            var data = new Data<LargeInteger>(null);
            dictionary.Add(property, data);

            DecoderDelegate WriterCall(PropertyInfoProxy propertyInfo) => (decoderPropertyInfo, input) => new LargeInteger();

            this.writersMock.Setup(m => m[typeof(int)]).Returns(WriterCall);
            this.writersMock.Setup(m => m.Execute()).Returns(new[] {new object(), new object()});

            // Act/Assert

            Helpers.ExceptionTest(() => this.generator.FillData(dictionary), typeof(DataLengthMismatchException),
                Messages.DataCountsDoNotMatch);
        }
    }
}