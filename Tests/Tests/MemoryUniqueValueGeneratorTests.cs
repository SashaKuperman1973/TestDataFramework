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

using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.UniqueValueGenerator.Concrete;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class MemoryUniqueValueGeneratorTests
    {
        private IAttributeDecorator attributeDecorator;
        private Mock<IDeferredValueGenerator<LargeInteger>> deferredValueGeneratorMock;
        private MemoryUniqueValueGenerator generator;
        private Mock<IPropertyValueAccumulator> propertyValueAccumulatorMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.attributeDecorator = new StandardAttributeDecorator(null, new AssemblyWrapper(), new Schema());

            this.propertyValueAccumulatorMock = new Mock<IPropertyValueAccumulator>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<LargeInteger>>();

            this.generator = new MemoryUniqueValueGenerator(this.propertyValueAccumulatorMock.Object,
                this.attributeDecorator,
                this.deferredValueGeneratorMock.Object, false);
        }

        [TestMethod]
        public void GetValue_AutoKey_Test()
        {
            // Arrange

            PropertyInfo keyPropertyInfo = typeof(ClassWithIntAutoPrimaryKey).GetProperty("Key");

            // Act

            this.generator.GetValue(keyPropertyInfo);

            // Assert

            this.deferredValueGeneratorMock.Verify(
                m => m.AddDelegate(keyPropertyInfo, It.IsAny<DeferredValueGetterDelegate<LargeInteger>>()), Times.Once);
        }

        [TestMethod]
        public void GetValue_ManualKey_Test()
        {
            // Arrange

            PropertyInfo keyPropertyInfo = typeof(ClassWithIntManualPrimaryKey).GetProperty("Key");

            // Act

            this.generator.GetValue(keyPropertyInfo);

            // Assert

            this.deferredValueGeneratorMock.Verify(
                m => m.AddDelegate(keyPropertyInfo, It.IsAny<DeferredValueGetterDelegate<LargeInteger>>()), Times.Once);
        }

        [TestMethod]
        public void GetValue_NoSupportedKeyType_Test()
        {
            // Arrange

            PropertyInfo keyPropertyInfo = typeof(ClassWithIntUnsupportedPrimaryKey).GetProperty("Key");

            // Act

            this.generator.GetValue(keyPropertyInfo);

            // Assert

            this.deferredValueGeneratorMock.Verify(
                m => m.AddDelegate(It.IsAny<PropertyInfo>(), It.IsAny<DeferredValueGetterDelegate<LargeInteger>>()),
                Times.Never);
        }

        [TestMethod]
        public void NotAPrimaryKey_Test()
        {
            // Arrange

            PropertyInfo keyPropertyInfo = typeof(SubjectClass).GetProperty("Integer");

            // Act

            this.generator.GetValue(keyPropertyInfo);
            this.generator.GetValue(keyPropertyInfo);

            // Assert

            this.propertyValueAccumulatorMock.Verify(m => m.GetValue(keyPropertyInfo, Helper.DefaultInitalCount),
                Times.Exactly(2));
        }
    }
}