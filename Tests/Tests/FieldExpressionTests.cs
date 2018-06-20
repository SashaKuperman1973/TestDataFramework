using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;
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
            this.rangeOperableListMock =
                new Mock<RangeOperableList<ElementType>>(null, null, null, null, null, null, null);

            this.expression = element => element.AProperty;

            this.objectGraphServiceMock = new Mock<IObjectGraphService>();

            this.fieldExpression =
                new FieldExpression<ElementType, ElementType.PropertyType>(this.expression,
                    this.rangeOperableListMock.Object, this.objectGraphServiceMock.Object);
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
            this.rangeOperableListMock.Setup(m => m.Set(this.expression))
                .Returns(this.fieldExpression);

            // Act

            FieldExpression<ElementType, ElementType.PropertyType> result = this.fieldExpression.Set(this.expression);

            // Assert

            this.rangeOperableListMock.Verify(m => m.Set(this.expression));
            Assert.AreEqual(this.fieldExpression, result);
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

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_TProperty_Test()
        {
            var values = new[] { new ElementType.PropertyType(), };

            Func<OperableList<ElementType>> action = () => this.fieldExpression.GuaranteePropertiesByFixedQuantity(values, 5);

            this.GuaranteePropertiesByFixedQuantity_Test(action, values);
        }

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_Func_Test()
        {
            var values = new[] {new ElementType.PropertyType()};

            IEnumerable<Func<ElementType.PropertyType>> funcs =
                values.Select<ElementType.PropertyType, Func<ElementType.PropertyType>>(value => () => value);

            Func<OperableList<ElementType>> action = () => this.fieldExpression.GuaranteePropertiesByFixedQuantity(funcs, 5);

            this.GuaranteePropertiesByFixedQuantity_Test(action, values);
        }

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_Mixed_TypeAndFunc_Test()
        {
            var values = new[] { new ElementType.PropertyType(), new ElementType.PropertyType() };
            var objects = new object[] { values[0], (Func<ElementType.PropertyType>)(() => values[1]) };

            Func<OperableList<ElementType>> action = () => this.fieldExpression.GuaranteePropertiesByFixedQuantity(objects, 5);

            this.GuaranteePropertiesByFixedQuantity_Test(action, values);
        }

        private void GuaranteePropertiesByFixedQuantity_Test(Func<OperableList<ElementType>> action, ElementType.PropertyType[] values)
        {
            var objectGraph = new List<PropertyInfo> { typeof(ElementType).GetProperty(nameof(ElementType.AProperty)) };

            this.objectGraphServiceMock.Setup(m => m.GetObjectGraph(this.expression)).Returns(objectGraph);

            // Act

            OperableList<ElementType> operableList = action();

            // Assert

            Assert.AreEqual(this.rangeOperableListMock.Object, operableList);

            GuaranteedValues guaranteedValue = this.rangeOperableListMock.Object.GuaranteedPropertySetters.Single();

            Assert.AreEqual(5, guaranteedValue.TotalFrequency);
            Assert.IsNull(guaranteedValue.FrequencyPercentage);

            for (int i=0; i < values.Length; i++)
            {
                var propertySettersEnumerable = guaranteedValue.Values as IEnumerable<ExplicitPropertySetter>;
                Assert.IsNotNull(propertySettersEnumerable);
                List<ExplicitPropertySetter> propertySetters = propertySettersEnumerable.ToList();

                Assert.AreEqual(objectGraph, propertySetters[i].PropertyChain);

                var element = new ElementType();
                propertySetters[i].Action(element);
                Assert.AreEqual(values[i], element.AProperty);
            }
        }
    }
}