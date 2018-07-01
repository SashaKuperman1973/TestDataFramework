using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.OperableList;

namespace Tests.Tests.FieldExpressionTests.ListParentTests
{
    [TestClass]
    public partial class ListParentFieldExpressionTests
    {
        private Expression<Func<ElementType, ElementType.PropertyType>> expression;

        private ListParentFieldExpression<ElementType, ElementType.PropertyType> listParentFieldExpression;

        private Mock<ListParentOperableList<ElementType>> listParentOperableListMock;

        private Mock<IObjectGraphService> objectGraphServiceMock;

        public class ElementType
        {
            public PropertyType AProperty { get; set; }

            public class PropertyType
            {
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            var valueGuaranteePopulatorMock = new Mock<ValueGuaranteePopulator>();

            this.expression = element => element.AProperty;

            this.objectGraphServiceMock = new Mock<IObjectGraphService>();

            this.listParentOperableListMock =
                new Mock<ListParentOperableList<ElementType>>(valueGuaranteePopulatorMock.Object, null, null, null, null, null, null);
            this.listParentFieldExpression =
                new ListParentFieldExpression<ElementType, ElementType.PropertyType>(this.expression,
                    this.listParentOperableListMock.Object, this.objectGraphServiceMock.Object);
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
            // Act

            this.listParentFieldExpression.BindAndMake();

            // Assert

            this.listParentOperableListMock.Verify(m => m.BindAndMake());
        }
    }
}
