using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.RootReferenceParentFieldExpression
{
    [TestClass]
    public class ReferenceParentFieldExpressionTests
    {
        private TestContext testContext;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
        }

        [TestMethod]
        public void OperableList_Test()
        {
            RootReferenceParentOperableList<ElementType, ElementParentType> actual =
                this.testContext.RootReferenceParentFieldExpression.OperableList;

            Assert.IsNotNull(actual);
            Assert.AreEqual(this.testContext.RootReferenceParentOperableListMock.Object, actual);
        }

        [TestMethod]
        public void RecordObjects_Test()
        {
            // Arrange
            var element = new ElementType();
            this.testContext.RootReferenceParentOperableListMock.SetupGet(m => m.RecordObjects).Returns(new[] {element});

            // Act

            IEnumerable<ElementType> result = this.testContext.RootReferenceParentFieldExpression.RecordObjects;

            // Assert

            Assert.IsNotNull(result);

            ElementType[] resultArray = result.ToArray();

            Assert.AreEqual(1, resultArray.Length);
            Assert.AreEqual(element, resultArray[0]);
        }

        [TestMethod]
        public void Make_Test()
        {
            // Arrange

            var expected = new ElementParentType();
            this.testContext.RootReferenceParentOperableListMock.Setup(m => m.Make()).Returns(expected);

            // Act

            ElementParentType actual = this.testContext.RootReferenceParentFieldExpression.Make();

            // Assert

            this.testContext.RootReferenceParentOperableListMock.Verify(m => m.Make());

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BindAndMake()
        {
            // Arrange

            var expected = new ElementParentType();
            this.testContext.RootReferenceParentOperableListMock.Setup(m => m.BindAndMake()).Returns(expected);

            // Act

            ElementParentType actual = this.testContext.RootReferenceParentFieldExpression.BindAndMake();

            // Assert

            this.testContext.RootReferenceParentOperableListMock.Verify(m => m.BindAndMake());

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }
    }
}
