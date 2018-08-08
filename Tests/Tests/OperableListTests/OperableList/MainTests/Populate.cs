using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Populator;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.OperableList.MainTests
{
    [TestClass]
    public class Populate
    {
        private TestContext testContext;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
        }
        
        [TestMethod]
        public void Populate_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            // Act

            operableList.Populate();

            // Assert

            this.testContext.InputMocks.ForEach(m => m.Verify(n => n.Populate()));

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(
                It.IsAny<OperableList<ElementType>>(),
                It.IsAny<IEnumerable<GuaranteedValues>>(),
                It.IsAny<IValueGauranteePopulatorContextService>()
                ), 
                Times.Never);

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(
                operableList,
                It.IsAny<IEnumerable<GuaranteedValues>>(),
                It.Is<IValueGauranteePopulatorContextService>(
                    s => s is ValueSetContextService)
            ), Times.Never);

            Assert.IsTrue(operableList.IsPopulated);
        }

        [TestMethod]
        public void Populate_GuarantedPropertiesAreSet_Test()
        {
            // Arrange

            var guaranteedValues = new GuaranteedValues();

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();
            operableList.AddGuaranteedPropertySetter(guaranteedValues);

            // Act

            operableList.Populate();

            // Assert

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(
                operableList,
                It.Is<IEnumerable<GuaranteedValues>>(v => v.Single() == guaranteedValues),
                It.Is<IValueGauranteePopulatorContextService>(
                    s => s is ExplicitPropertySetterContextService)
            ));

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(
                operableList,
                It.IsAny<IEnumerable<GuaranteedValues>>(),
                It.Is<IValueGauranteePopulatorContextService>(
                    s => s is ValueSetContextService)
            ), Times.Never);
        }

        [TestMethod]
        public void Populate_GuaranteedValueAreSet_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();
            operableList.GuaranteeByFixedQuantity(Enumerable.Empty<ElementType>());

            // Act

            operableList.Populate();

            // Assert

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(
                operableList,
                It.IsAny<IEnumerable<GuaranteedValues>>(),
                It.Is<IValueGauranteePopulatorContextService>(
                    s => s is ExplicitPropertySetterContextService)
            ), Times.Never);

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(
                operableList,
                It.IsAny<IEnumerable<GuaranteedValues>>(),
                It.Is<IValueGauranteePopulatorContextService>(
                    s => s is ValueSetContextService)
            ));
        }

        [TestMethod]
        public void Populate_DoesNotPopualte_IfAlreadyPopulated_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();
            operableList.IsPopulated = true;

            operableList.GuaranteeByFixedQuantity(Enumerable.Empty<ElementType>());
            operableList.AddGuaranteedPropertySetter(It.IsAny<GuaranteedValues>());

            // Act

            operableList.Populate();

            // Assert

            this.testContext.InputMocks.ForEach(m => m.Verify(n => n.Populate(), Times.Never));

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(
                operableList,
                It.IsAny<IEnumerable<GuaranteedValues>>(),
                It.Is<IValueGauranteePopulatorContextService>(
                    s => s is ExplicitPropertySetterContextService)
            ));

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(
                operableList,
                It.IsAny<IEnumerable<GuaranteedValues>>(),
                It.Is<IValueGauranteePopulatorContextService>(
                    s => s is ValueSetContextService)
            ));
        }

        [TestMethod]
        public void Populate_DoesNotPopualte_IfShallowCopy_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList(isShallowCopy: true);

            operableList.GuaranteeByFixedQuantity(Enumerable.Empty<ElementType>());
            operableList.AddGuaranteedPropertySetter(It.IsAny<GuaranteedValues>());

            // Act

            operableList.Populate();

            // Assert

            this.testContext.InputMocks.ForEach(m => m.Verify(n => n.Populate(), Times.Never));

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(
                operableList,
                It.IsAny<IEnumerable<GuaranteedValues>>(),
                It.Is<IValueGauranteePopulatorContextService>(
                    s => s is ExplicitPropertySetterContextService)
            ));

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(
                operableList,
                It.IsAny<IEnumerable<GuaranteedValues>>(),
                It.Is<IValueGauranteePopulatorContextService>(
                    s => s is ValueSetContextService)
            ));
        }
    }
}
