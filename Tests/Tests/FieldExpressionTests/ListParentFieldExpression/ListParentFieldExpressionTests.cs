using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.ListParentFieldExpression
{
    [TestClass]
    public class ListParentFieldExpressionTests
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
            ListParentOperableList<ElementType, OperableListEx<ElementType>, ElementType> actual =
                this.testContext.ListParentFieldExpression.OperableList;

            Assert.IsNotNull(actual);
            Assert.AreEqual(this.testContext.ListParentOperableListMock.Object, actual);
        }

        [TestMethod]
        public void RecordObjects_Test()
        {
            // Arrange
            var element = new ElementType();
            this.testContext.ListParentOperableListMock.SetupGet(m => m.RecordObjects).Returns(new[] {element});

            // Act

            IEnumerable<ElementType> result = this.testContext.ListParentFieldExpression.RecordObjects;

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

            var expected = new ElementType[0];
            this.testContext.ListParentOperableListMock.Setup(m => m.Make()).Returns(expected);

            // Act

            IEnumerable<ElementType> actual = this.testContext.ListParentFieldExpression.Make();

            // Assert

            this.testContext.ListParentOperableListMock.Verify(m => m.Make());

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BindAndMake()
        {
            // Arrange

            var expected = new ElementType[0];
            this.testContext.ListParentOperableListMock.Setup(m => m.BindAndMake()).Returns(expected);

            // Act

            IEnumerable<ElementType> actual = this.testContext.ListParentFieldExpression.BindAndMake();

            // Assert

            this.testContext.ListParentOperableListMock.Verify(m => m.BindAndMake());

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

            ListParentFieldExpression<ElementType, OperableListEx<ElementType>, ElementType, ElementType.PropertyType>
                actual = this.testContext.ListParentFieldExpression.SetRange(m => m.AProperty, rangeFactory);

            // Assert

            Assert.IsNotNull(actual);
            Assert.AreEqual(this.testContext.ListParentFieldExpression, actual);

            this.testContext.ListParentOperableListMock.Verify(m => m.AddRange(n => n.AProperty, rangeFactory));
        }

        [TestMethod]
        public void SetRange_Range_Test()
        {
            // Act

            var range = new[]
            {
                new ElementType.PropertyType(), new ElementType.PropertyType(), new ElementType.PropertyType(),
            };

            ListParentFieldExpression<ElementType, OperableListEx<ElementType>, ElementType, ElementType.PropertyType>
                actual = this.testContext.ListParentFieldExpression.SetRange(m => m.AProperty, range);

            // Assert

            Assert.IsNotNull(actual);
            Assert.AreEqual(this.testContext.ListParentFieldExpression, actual);

            this.testContext.ListParentOperableListMock.Verify(m => m.AddRange(n => n.AProperty, range));
        }
    }
}
