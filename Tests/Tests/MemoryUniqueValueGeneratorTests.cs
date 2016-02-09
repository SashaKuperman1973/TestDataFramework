using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        private MemoryUniqueValueGenerator generator;
        private Mock<IPropertyValueAccumulator> propertyValueAccumulatorMock;
        private Mock<IDeferredValueGenerator<LargeInteger>> deferredValueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.propertyValueAccumulatorMock = new Mock<IPropertyValueAccumulator>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<LargeInteger>>();

            this.generator = new MemoryUniqueValueGenerator(this.propertyValueAccumulatorMock.Object,
                this.deferredValueGeneratorMock.Object, throwIfUnhandledType: false);
        }

        [TestMethod]
        public void GetValue_AutoKey_Test()
        {
            // Arrange

            PropertyInfo keyPropertyInfo = typeof(ClassWithIntAutoPrimaryKey).GetProperty("Key");

            // Act

            this.generator.GetValue(keyPropertyInfo);

            // Assert

            this.deferredValueGeneratorMock.Verify(m => m.AddDelegate(keyPropertyInfo, It.IsAny<DeferredValueGetterDelegate<LargeInteger>>()), Times.Once);
        }

        [TestMethod]
        public void GetValue_ManualKey_Test()
        {
            // Arrange

            PropertyInfo keyPropertyInfo = typeof(ClassWithIntManualPrimaryKey).GetProperty("Key");

            // Act

            this.generator.GetValue(keyPropertyInfo);

            // Assert

            this.deferredValueGeneratorMock.Verify(m => m.AddDelegate(keyPropertyInfo, It.IsAny<DeferredValueGetterDelegate<LargeInteger>>()), Times.Once);
        }

        [TestMethod]
        public void GetValue_NoSupportedKeyType_Test()
        {
            // Arrange

            PropertyInfo keyPropertyInfo = typeof(ClassWithIntUnsupportedPrimaryKey).GetProperty("Key");

            // Act

            this.generator.GetValue(keyPropertyInfo);

            // Assert

            this.deferredValueGeneratorMock.Verify(m => m.AddDelegate(It.IsAny<PropertyInfo>(), It.IsAny<DeferredValueGetterDelegate<LargeInteger>>()), Times.Never);
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

            this.propertyValueAccumulatorMock.Verify(m => m.GetValue(keyPropertyInfo, Helper.DefaultInitalCount), Times.Exactly(2));
        }
    }
}
