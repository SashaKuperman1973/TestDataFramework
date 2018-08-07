using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.OperableList
{
    [TestClass]
    public class OperableListTests
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
            Mock<Populatable> populatableMock = Helpers.GetMock<Populatable>();

            operableList.AddChild(populatableMock.Object);
            operableList.AddGuaranteedPropertySetter(new GuaranteedValues());
            //operableList

            // Act

            operableList.Populate();

            // Assert

            populatableMock.Verify(m => m.Populate());
        }
    }
}
