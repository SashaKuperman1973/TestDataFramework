using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
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
                IDeferredValueGenerator<LargeInteger> deferredValueGenerator) : base(accumulator, deferredValueGenerator, throwIfUnhandledType: false)
            {
            }

            public new void DeferValue(PropertyInfo propertyInfo)
            {
                base.DeferValue(propertyInfo);
            }
        }

        private UniqueValueGenerator uniqueValueGenerator;
        private Mock<IPropertyValueAccumulator> propertyValueAccumulatorMock;
        private Mock<IDeferredValueGenerator<LargeInteger>> deferredValueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.propertyValueAccumulatorMock = new Mock<IPropertyValueAccumulator>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<LargeInteger>>();

            this.uniqueValueGenerator = new UniqueValueGenerator(this.propertyValueAccumulatorMock.Object,
                this.deferredValueGeneratorMock.Object);
        }

        [TestMethod]
        public void DeferValue_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(PrimaryTable).GetProperty("Text");

            DeferredValueGetterDelegate<LargeInteger> inputDelegate = null;

            this.deferredValueGeneratorMock.Setup(
                m => m.AddDelegate(propertyInfo, It.IsAny<DeferredValueGetterDelegate<LargeInteger>>()))
                .Callback<PropertyInfo, DeferredValueGetterDelegate<LargeInteger>>((pi, d) => inputDelegate = d).Verifiable();

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
                m => m.GetValue(propertyInfo, Helper.DefaultInitalCount), 
                Times.Once);
        }
    }
}
