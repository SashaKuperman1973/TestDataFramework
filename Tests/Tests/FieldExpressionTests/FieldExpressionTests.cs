using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete;

namespace Tests.Tests.FieldExpressionTests
{
    [TestClass]
    public partial class FieldExpressionTests
    {
        private Expression<Func<ElementType, ElementType.PropertyType>> expression;

        private FieldExpression<ElementType, ElementType.PropertyType> fieldExpression;

        private Mock<OperableList<ElementType>> operableListMock;
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

            this.operableListMock =
                new Mock<OperableList<ElementType>>(valueGuaranteePopulatorMock.Object, null, null, null, null, null, null);

            this.expression = element => element.AProperty;

            this.objectGraphServiceMock = new Mock<IObjectGraphService>();

            this.fieldExpression =
                new FieldExpression<ElementType, ElementType.PropertyType>(this.expression,
                    this.operableListMock.Object, this.objectGraphServiceMock.Object);
        }

        [TestMethod]
        public void RecordObjects_Test()
        {
            // Arrange
            var element = new ElementType();
            this.operableListMock.SetupGet(m => m.RecordObjects).Returns(new[] {element});

            // Act

            IEnumerable<ElementType> result = this.fieldExpression.RecordObjects;

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

            this.fieldExpression.Make();

            // Assert

            this.operableListMock.Verify(m => m.Make());
        }

        [TestMethod]
        public void BindAndMake()
        {
            // Act

            this.fieldExpression.BindAndMake();

            // Assert

            this.operableListMock.Verify(m => m.BindAndMake());
        }
    }
}