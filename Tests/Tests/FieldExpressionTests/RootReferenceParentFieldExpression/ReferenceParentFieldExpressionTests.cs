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
            RootReferenceParentOperableList<ElementType, ElementTypeBase> actual =
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

            var expected = new ElementTypeBase();
            this.testContext.RootReferenceParentOperableListMock.Setup(m => m.Make()).Returns(expected);

            // Act

            ElementTypeBase actual = this.testContext.RootReferenceParentFieldExpression.Make();

            // Assert

            this.testContext.RootReferenceParentOperableListMock.Verify(m => m.Make());

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BindAndMake()
        {
            // Arrange

            var expected = new ElementTypeBase();
            this.testContext.RootReferenceParentOperableListMock.Setup(m => m.BindAndMake()).Returns(expected);

            // Act

            ElementTypeBase actual = this.testContext.RootReferenceParentFieldExpression.BindAndMake();

            // Assert

            this.testContext.RootReferenceParentOperableListMock.Verify(m => m.BindAndMake());

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetRange_RangeFactory_Test()
        {
            // Act

            var rangeFactory = (Func<IEnumerable<ElementType.PropertyType>>)(() => new[]
            {
                new ElementType.PropertyType(), new ElementType.PropertyType(), new ElementType.PropertyType(),
            });

            RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementTypeBase> 
                actual = this.testContext.RootReferenceParentFieldExpression.SetRange(m => m.AProperty, rangeFactory);

            // Assert

            Assert.IsNotNull(actual);
            Assert.AreEqual(this.testContext.RootReferenceParentFieldExpression, actual);

            this.testContext.RootReferenceParentOperableListMock.Verify(m => m.AddRange(n => n.AProperty, rangeFactory));
        }

        [TestMethod]
        public void SetRange_Range_Test()
        {
            // Act

            var range = new[]
            {
                new ElementType.PropertyType(), new ElementType.PropertyType(), new ElementType.PropertyType(),
            };

            RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementTypeBase> 
                actual = this.testContext.RootReferenceParentFieldExpression.SetRange(m => m.AProperty, range);

            // Assert

            Assert.IsNotNull(actual);
            Assert.AreEqual(this.testContext.RootReferenceParentFieldExpression, actual);

            this.testContext.RootReferenceParentOperableListMock.Verify(m => m.AddRange(n => n.AProperty, range));
        }
    }
}
