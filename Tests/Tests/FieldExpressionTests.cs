using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator.Concrete;
using Range = TestDataFramework.Populator.Concrete.Range;

namespace Tests.Tests
{
    [TestClass]
    public class FieldExpressionTests
    {
        private Expression<Func<ElementType, ElementType.PropertyType>> expression;

        private FieldExpression<ElementType, ElementType.PropertyType> fieldExpression;

        private Mock<RangeOperableList<ElementType>> rangeOperableListMock;

        [TestInitialize]
        public void Initialize()
        {
            this.rangeOperableListMock =
                new Mock<RangeOperableList<ElementType>>(null, null, null, null, null, null, null);
            this.expression = element => element.AProperty;

            this.fieldExpression =
                new FieldExpression<ElementType, ElementType.PropertyType>(this.expression,
                    this.rangeOperableListMock.Object);
        }

        [TestMethod]
        public void RecordObjects_Test()
        {
            // Arrange
            var element = new ElementType();
            this.rangeOperableListMock.SetupGet(m => m.RecordObjects).Returns(new[] {element});

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

            this.rangeOperableListMock.Verify(m => m.Make());
        }

        [TestMethod]
        public void BindAndMake()
        {
            // Act

            this.fieldExpression.BindAndMake();

            // Assert

            this.rangeOperableListMock.Verify(m => m.BindAndMake());
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_MixedFunc_Test()
        {
            // Arrange

            var values = new object[] {new ElementType(), (Func<ElementType>) (() => new ElementType())};
            this.rangeOperableListMock.Setup(m => m.GuaranteeByFixedQuantity(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByFixedQuantity(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByFixedQuantity(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_Func_Test()
        {
            // Arrange

            var values = new Func<ElementType>[] {() => new ElementType(), () => new ElementType()};
            this.rangeOperableListMock.Setup(m => m.GuaranteeByFixedQuantity(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByFixedQuantity(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByFixedQuantity(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_TListElement_Test()
        {
            // Arrange

            var values = new[] {new ElementType(), new ElementType()};
            this.rangeOperableListMock.Setup(m => m.GuaranteeByFixedQuantity(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByFixedQuantity(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByFixedQuantity(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_TValue_Test()
        {
            // Arrange

            var values = new[] {new ElementType(), new ElementType()};
            this.rangeOperableListMock.Setup(m => m.GuaranteeByPercentageOfTotal(values, 5))
                .Returns(this.rangeOperableListMock.Object);
            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByPercentageOfTotal(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByPercentageOfTotal(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_MixedFunc_Test()
        {
            // Arrange

            var values = new object[] {new ElementType(), (Func<ElementType>) (() => new ElementType())};

            this.rangeOperableListMock.Setup(m => m.GuaranteeByPercentageOfTotal(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByPercentageOfTotal(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByPercentageOfTotal(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_Func_Test()
        {
            // Arrange

            var values = new Func<ElementType>[] {() => new ElementType(), () => new ElementType()};
            this.rangeOperableListMock.Setup(m => m.GuaranteeByPercentageOfTotal(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByPercentageOfTotal(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByPercentageOfTotal(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_TListElement_Test()
        {
            // Arrange

            var values = new[] {new ElementType(), new ElementType()};
            this.rangeOperableListMock.Setup(m => m.GuaranteeByPercentageOfTotal(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByPercentageOfTotal(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByPercentageOfTotal(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void Ignore_Test()
        {
            // Arrange

            Expression<Func<ElementType, ElementType.PropertyType>> expression =
                listElement => new ElementType.PropertyType();

            // Act

            this.fieldExpression.Ignore(expression);

            // Assert

            this.rangeOperableListMock.Setup(m => m.Ignore(expression));
        }

        [TestMethod]
        public void Set_TValueProperty_Test()
        {
            // Arrange

            Expression<Func<ElementType, ElementType.PropertyType>> expression = element => element.AProperty;
            var value = new ElementType.PropertyType();
            var range = new Range();

            this.rangeOperableListMock.Setup(m => m.Set(expression, value, range))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            RangeOperableList<ElementType> result = this.fieldExpression.Set(expression, value, range);

            // Assert

            this.rangeOperableListMock.Verify(m => m.Set(expression, value, range));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void Set_ValueFunc_Test()
        {
            // Arrange

            Expression<Func<ElementType, ElementType.PropertyType>> expression = element => element.AProperty;
            Func<ElementType.PropertyType> value = () => new ElementType.PropertyType();
            var range = new Range();

            this.rangeOperableListMock.Setup(m => m.Set(expression, value, range))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            RangeOperableList<ElementType> result = this.fieldExpression.Set(expression, value, range);

            // Assert

            this.rangeOperableListMock.Verify(m => m.Set(expression, value, range));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void Set_ChangeValueType_Test()
        {
            Expression<Func<ElementType, ElementType.PropertyType>> expression = element => element.AProperty;

            var returnFieldExpression =
                new FieldExpression<ElementType, ElementType.PropertyType>(element => element.AProperty,
                    this.rangeOperableListMock.Object);

            this.rangeOperableListMock.Setup(m => m.Set(expression))
                .Returns(returnFieldExpression);

            // Act

            FieldExpression<ElementType, ElementType.PropertyType> result = this.fieldExpression.Set(expression);

            // Assert

            this.rangeOperableListMock.Verify(m => m.Set(expression));
            Assert.AreEqual(returnFieldExpression, result);
        }

        [TestMethod]
        public void Value_Test()
        {
            // Arrange

            var range = new Range();
            var value = new ElementType.PropertyType();

            // Act

            FieldExpression<ElementType, ElementType.PropertyType> result =
                this.fieldExpression.Value(value, range);

            // Assert

            this.rangeOperableListMock.Verify(m => m.Set(this.expression, value, range));
            Assert.AreEqual(this.fieldExpression, result);
        }

        [TestMethod]
        public void Value_Func_Test()
        {
            // Arrange

            var range = new Range();
            Func<ElementType.PropertyType> valueFunc = () => new ElementType.PropertyType();

            // Act

            FieldExpression<ElementType, ElementType.PropertyType> result =
                this.fieldExpression.Value(valueFunc, range);

            // Assert

            this.rangeOperableListMock.Verify(m => m.Set(this.expression, valueFunc, range));
            Assert.AreEqual(this.fieldExpression, result);
        }

        public class ElementType
        {
            public PropertyType AProperty { get; set; }

            public class PropertyType
            {
            }
        }
    }
}