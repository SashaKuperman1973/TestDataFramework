using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.UniqueValueGenerator;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class BaseUniqueValueGeneratorTests
    {
        private class UniqueValueGenerator : BaseUniqueValueGenerator
        {
            public UniqueValueGenerator(IPropertyValueAccumulator accumulator,
                IDeferredValueGenerator<ulong> deferredValueGenerator) : base(accumulator, deferredValueGenerator)
            {
            }

            public new void DeferValue(PropertyInfo propertyInfo)
            {
                base.DeferValue(propertyInfo);
            }
        }

        private UniqueValueGenerator uniqueValueGenerator;
        private Mock<IPropertyValueAccumulator> propertyValueAccumulatorMock;
        private Mock<IDeferredValueGenerator<ulong>> deferredValueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.propertyValueAccumulatorMock = new Mock<IPropertyValueAccumulator>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<ulong>>();

            this.uniqueValueGenerator = new UniqueValueGenerator(this.propertyValueAccumulatorMock.Object,
                this.deferredValueGeneratorMock.Object);
        }

        [TestMethod]
        public void DeferValue_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(PrimaryTable).GetProperty("Text");

            DeferredValueGetterDelegate<ulong> inputDelegate = null;

            this.deferredValueGeneratorMock.Setup(
                m => m.AddDelegate(propertyInfo, It.IsAny<DeferredValueGetterDelegate<ulong>>()))
                .Callback<PropertyInfo, DeferredValueGetterDelegate<ulong>>((pi, d) => inputDelegate = d).Verifiable();

            const long initialCount = 5;

            this.propertyValueAccumulatorMock.Setup(m => m.GetValue(propertyInfo, initialCount)).Verifiable();

            // Act

            this.uniqueValueGenerator.DeferValue(propertyInfo);
            inputDelegate(initialCount);

            // Assert

            this.deferredValueGeneratorMock.Verify();
            this.propertyValueAccumulatorMock.Verify();
        }

        [TestMethod]
        public void GetValue_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof (PrimaryTable).GetProperty("Text");

            // Act

            this.uniqueValueGenerator.GetValue(propertyInfo);

            // Assert

            this.propertyValueAccumulatorMock.Verify(
                m => m.GetValue(propertyInfo, TestDataFramework.Helpers.Helper.DefaultInitalCount), 
                Times.Once);
        }
    }
}
