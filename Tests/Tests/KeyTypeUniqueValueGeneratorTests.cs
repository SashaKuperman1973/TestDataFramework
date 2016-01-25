using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.UniqueValueGenerator.Concrete;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class KeyTypeUniqueValueGeneratorTests
    {
        private KeyTypeUniqueValueGenerator generator;

        private Mock<IPropertyValueAccumulator> propertyValueAccumulatorMock;
        private Mock<IDeferredValueGenerator<ulong>> deferredValueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.propertyValueAccumulatorMock = new Mock<IPropertyValueAccumulator>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<ulong>>();

            this.generator = new KeyTypeUniqueValueGenerator(this.propertyValueAccumulatorMock.Object,
                this.deferredValueGeneratorMock.Object);
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
                typeof(ULongKeyClass),
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
                m => m.AddDelegate(keyPropertyInfo, It.IsAny<DeferredValueGetterDelegate<ulong>>()),
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
                m => m.AddDelegate(It.IsAny<PropertyInfo>(), It.IsAny<DeferredValueGetterDelegate<ulong>>()),
                Times.Never);
        }

        [TestMethod]
        public void GetValue_NotPrimaryKey_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("LongInteger");

            this.propertyValueAccumulatorMock.Setup(m => m.GetValue(propertyInfo, It.IsAny<ulong>())).Returns(5);

            // Act

            object result = this.generator.GetValue(propertyInfo);

            // Assert

            Assert.AreEqual(5, result);
        }
    }
}
