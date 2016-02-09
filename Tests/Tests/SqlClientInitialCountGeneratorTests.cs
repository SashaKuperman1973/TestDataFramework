/*
    Copyright 2016 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class SqlClientInitialCountGeneratorTests
    {
        private SqlClientInitialCountGenerator generator;
        private Mock<IWriterDictinary> writersMock;
        private Mock<LetterEncoder> encoderMock;

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

            PropertyInfo propertyInfo1 = typeof(ClassWithStringAutoPrimaryKey).GetProperty("Key");
            PropertyInfo propertyInfo2 = typeof(ClassWithIntAutoPrimaryKey).GetProperty("Key");

            var data1 = new Data<LargeInteger>(null);
            var data2 = new Data<LargeInteger>(null);
            var dictionary = new Dictionary<PropertyInfo, Data<LargeInteger>>
            {
                {propertyInfo1, data1},
                {propertyInfo2, data2},
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
    }
}
