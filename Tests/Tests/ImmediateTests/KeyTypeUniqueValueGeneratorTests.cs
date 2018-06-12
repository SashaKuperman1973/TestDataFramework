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
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.UniqueValueGenerator.Concrete;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class KeyTypeUniqueValueGeneratorTests
    {
        private Mock<IDeferredValueGenerator<LargeInteger>> deferredValueGeneratorMock;
        private KeyTypeUniqueValueGenerator generator;

        private Mock<IPropertyValueAccumulator> propertyValueAccumulatorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.propertyValueAccumulatorMock = new Mock<IPropertyValueAccumulator>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<LargeInteger>>();

            this.generator = new KeyTypeUniqueValueGenerator(this.propertyValueAccumulatorMock.Object,
                new StandardAttributeDecorator(null, new AssemblyWrapper(null), new Schema()),
                this.deferredValueGeneratorMock.Object,
                false);
        }

        [TestMethod]
        public void GetValue_Test()
        {
            new[]
                {
                    typeof(ByteKeyClass),
                    typeof(IntKeyClass),
                    typeof(ShortKeyClass),
                    typeof(LongKeyClass),
                    typeof(StringKeyClass),
                    typeof(UIntKeyClass),
                    typeof(UShortKeyClass),
                    typeof(ULongKeyClass)
                }
                .ToList().ForEach(this.PrimaryKeyTest);
        }

        private void PrimaryKeyTest(Type @class)
        {
            // Arrange

            this.Initialize();

            PropertyInfo keyPropertyInfo = @class.GetProperty("Key");

            // Act

            object result = this.generator.GetValue(keyPropertyInfo);

            // Assert

            this.deferredValueGeneratorMock.Verify(
                m => m.AddDelegate(keyPropertyInfo, It.IsAny<DeferredValueGetterDelegate<LargeInteger>>()),
                Times.Once);

            object expected = keyPropertyInfo.PropertyType.IsValueType
                ? Activator.CreateInstance(keyPropertyInfo.PropertyType)
                : null;

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetValue_DoesNotDefer_Test()
        {
            this.DoesNotDeferTest(typeof(AutoKeyClass));
            this.DoesNotDeferTest(typeof(NoneKeyClass));
            this.DoesNotDeferTest(typeof(UnhandledKeyClass));
        }

        private void DoesNotDeferTest(Type @class)
        {
            // Arrange

            this.Initialize();

            PropertyInfo keyPropertyInfo = @class.GetProperty("Key");

            // Act

            this.generator.GetValue(keyPropertyInfo);

            // Assert

            this.deferredValueGeneratorMock.Verify(
                m => m.AddDelegate(It.IsAny<PropertyInfo>(), It.IsAny<DeferredValueGetterDelegate<LargeInteger>>()),
                Times.Never);
        }

        [TestMethod]
        public void GetValue_NotPrimaryKey_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("LongInteger");

            this.propertyValueAccumulatorMock.Setup(m => m.GetValue(propertyInfo, It.IsAny<LargeInteger>())).Returns(5);

            // Act

            object result = this.generator.GetValue(propertyInfo);

            // Assert

            Assert.AreEqual(5, result);
        }
    }
}