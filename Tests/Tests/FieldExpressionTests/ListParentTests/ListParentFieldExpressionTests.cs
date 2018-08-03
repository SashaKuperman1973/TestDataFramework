using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.ListParentTests
{
    [TestClass]
    public partial class ListParentFieldExpressionTests
    {
        private Expression<Func<ElementType, ElementType.PropertyType>> expression;

        private ListParentFieldExpression<ElementType, OperableList<ElementType>, ElementType,
            ElementType.PropertyType> listParentFieldExpression;

        private Mock<ListParentOperableList<ElementType, OperableList<ElementType>, ElementType>> listParentOperableListMock;

        private Mock<IObjectGraphService> objectGraphServiceMock;

        [TestInitialize]
        public void Initialize()
        {
            this.expression = element => element.AProperty;

            this.objectGraphServiceMock = new Mock<IObjectGraphService>();

            this.listParentOperableListMock = Helpers.GetMock<ListParentOperableList<ElementType, OperableList<ElementType>, ElementType>>();

            this.listParentFieldExpression =
                new ListParentFieldExpression<ElementType, OperableList<ElementType>, ElementType,
                    ElementType.PropertyType>(
                    this.expression,
                    this.listParentOperableListMock.Object,
                    this.objectGraphServiceMock.Object);
        }

        [TestMethod]
        public void RecordObjects_Test()
        {
            // Arrange
            var element = new ElementType();
            this.listParentOperableListMock.SetupGet(m => m.RecordObjects).Returns(new[] {element});

            // Act

            IEnumerable<ElementType> result = this.listParentFieldExpression.RecordObjects;

            // Assert

            Assert.IsNotNull(result);

            ElementType[] resultArray = result.ToArray();

            Assert.AreEqual(1, resultArray.Length);
            Assert.AreEqual(element, resultArray[0]);
        }

        [TestMethod]
        public void Make_Test()
        {
            // Act

            this.listParentFieldExpression.Make();

            // Assert

            this.listParentOperableListMock.Verify(m => m.Make());
        }

        [TestMethod]
        public void BindAndMake()
        {
            // Arrange

            this.listParentOperableListMock.Setup(m => m.BindAndMake()).Returns(new ElementType[0]);

            // Act

            this.listParentFieldExpression.BindAndMake();

            // Assert

            this.listParentOperableListMock.Verify(m => m.BindAndMake());
        }
    }
}
