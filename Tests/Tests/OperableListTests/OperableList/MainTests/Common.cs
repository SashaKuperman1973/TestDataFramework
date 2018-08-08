using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.OperableList.MainTests
{
    [TestClass]
    public class Common
    {
        private TestContext testContext;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
        }

        [TestMethod]
        public void AddToReferences_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();
            var references = new List<RecordReference>();

            // Act

            operableList.AddToReferences(references);

            // Assert

            Helpers.AssertSetsAreEqual(this.testContext.Inputs, references);
        }

        [TestMethod]
        public void AddGuaranteedPorpertySetter_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();
            var guaranteedValues = new GuaranteedValues();
            
            // Act

            operableList.AddGuaranteedPropertySetter(guaranteedValues);

            operableList.Populate();

            // Assert

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(operableList,
                It.Is<IEnumerable<GuaranteedValues>>(v => v.Single() == guaranteedValues),
                It.Is<IValueGauranteePopulatorContextService>(s => s is ExplicitPropertySetterContextService)));
        }

        [TestMethod]
        public void RecordObjects_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            foreach (Mock<RecordReference<ElementType>> inputMock in this.testContext.InputMocks)
            {
                inputMock.Setup(m => m.RecordObject).Returns(new ElementType());
            }

            // Act

            IEnumerable<ElementType> result = operableList.RecordObjects;

            // Assert

            Helpers.AssertSetsAreEqual(this.testContext.InputObjects, result);
        }
    }
}
