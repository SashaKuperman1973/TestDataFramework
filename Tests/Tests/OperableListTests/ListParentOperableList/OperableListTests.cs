using System.Collections.Generic;
using System.Security.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.ListParentOperableList
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
        public void RootList_Test()
        {
            // Arrange

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> operableList =
                this.testContext.CreateOperableList();

            // Act

            OperableListEx<ElementParentType> rootList = operableList.RootList;

            // Assert

            Assert.IsNotNull(rootList);
            Assert.AreEqual(this.testContext.RootListMock.Object, rootList);
        }

        [TestMethod]
        public void ParentList_Test()
        {
            // Arrange

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> operableList =
                this.testContext.CreateOperableList();

            // Act

            OperableListEx<ElementParentType> parentList = operableList.ParentList;

            // Assert

            Assert.IsNotNull(parentList);
            Assert.AreEqual(this.testContext.ParentListMock.Object, parentList);
        }

        [TestMethod]
        public void Make_Test()
        {
            // Arrange

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> operableList =
                this.testContext.CreateOperableList();

            var rootObjects = new ElementParentType[0];
            this.testContext.RootListMock.Setup(m => m.RecordObjects).Returns(rootObjects);

            // Act

            IEnumerable<ElementParentType> result = operableList.Make();

            // Assert

            this.testContext.RootListMock.Verify(m => m.Populate());
            Assert.IsNotNull(result);
            Assert.AreEqual(rootObjects, result);
        }

        [TestMethod]
        public void BindAndMake_Test()
        {
            // Arrange

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> operableList =
                this.testContext.CreateOperableList();

            var rootObjects = new ElementParentType[0];
            this.testContext.RootListMock.Setup(m => m.RecordObjects).Returns(rootObjects);

            // Act

            IEnumerable<ElementParentType> result = operableList.BindAndMake();

            // Assert

            this.testContext.PopulatorMock.Verify(m => m.Bind());
            Assert.IsNotNull(result);
            Assert.AreEqual(rootObjects, result);
        }
    }
}
