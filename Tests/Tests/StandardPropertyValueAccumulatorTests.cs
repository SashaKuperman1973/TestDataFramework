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
using System.Linq;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardPropertyValueAccumulatorTests
    {
        private StandardPropertyValueAccumulator accumulator;
        private Mock<LetterEncoder> stringGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.stringGeneratorMock = new Mock<LetterEncoder>();
            this.accumulator = new StandardPropertyValueAccumulator(this.stringGeneratorMock.Object,
                new StandardAttributeDecorator(null, new AssemblyWrapper(), new Schema()));
        }

        [TestMethod]
        public void IntegerTest()
        {
            this.IntegerTest(typeof(ClassWithIntAutoPrimaryKey));
            this.IntegerTest(typeof(ClassWithShortAutoPrimaryKey));
            this.IntegerTest(typeof(ClassWithLongAutoPrimaryKey));
            this.IntegerTest(typeof(ClassWithByteAutoPrimaryKey));
        }

        private void IntegerTest(Type inputClass)
        {
            object[] resultArray = this.Test(inputClass);

            StandardPropertyValueAccumulatorTests.AreEqual(5, resultArray[0]);
            StandardPropertyValueAccumulatorTests.AreEqual(6, resultArray[1]);
        }

        [TestMethod]
        public void StringTest()
        {
            this.stringGeneratorMock.Setup(m => m.Encode(5, It.IsAny<int>())).Returns("A");
            this.stringGeneratorMock.Setup(m => m.Encode(6, It.IsAny<int>())).Returns("B");

            object[] resultArray = this.Test(typeof(ClassWithStringAutoPrimaryKey));

            StandardPropertyValueAccumulatorTests.AreEqual("A", resultArray[0]);
            StandardPropertyValueAccumulatorTests.AreEqual("B", resultArray[1]);
        }

        private object[] Test(Type inputClass)
        {
            PropertyInfo keyPropertyInfo = inputClass.GetProperty("Key");

            var resultArray = new object[2];

            // Act

            resultArray[0] = this.accumulator.GetValue(keyPropertyInfo, 5);
            resultArray[1] = this.accumulator.GetValue(keyPropertyInfo, 5);

            return resultArray;
        }

        private static void AreEqual(object expected, object actual)
        {
            if (expected == null)
                Assert.IsNull(actual);
            else
                Assert.AreEqual(Convert.ChangeType(expected, actual.GetType()), actual);
        }

        [TestMethod]
        public void IsTypeHandled_Test()
        {
            var included = new[]
            {
                typeof(string),
                typeof(byte), typeof(int), typeof(short), typeof(long),
                typeof(uint), typeof(ushort), typeof(ulong)
            };

            included.ToList().ForEach(type => Assert.IsTrue(this.accumulator.IsTypeHandled(type)));
            Assert.IsFalse(this.accumulator.IsTypeHandled(typeof(float)));
        }
    }
}