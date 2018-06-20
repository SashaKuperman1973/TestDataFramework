using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.DeepSetting;
using TestDataFramework.Populator.Concrete;

namespace Tests.Tests.FieldExpressionTests
{
    public partial class FieldExpressionTests
    {
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
            var values = new[] { new ElementType.PropertyType() };

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

            for (int i = 0; i < values.Length; i++)
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

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_Func_Default_FixedQuantity_Test()
        {
            // Arrange

            var guaranteedValues = new List<Func<ElementType.PropertyType>>();

            const int count = 5;

            for (int i = 0; i < count; i++)
            {
                var elementPropertyValue = new ElementType.PropertyType();
                guaranteedValues.Add(() => elementPropertyValue);
            }

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteePropertiesByFixedQuantity(guaranteedValues);

            // Assert

            Assert.AreEqual(count, result.GuaranteedPropertySetters.Single().TotalFrequency);
        }

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_TProperty_Default_FixedQuantity_Test()
        {
            // Arrange

            var guaranteedValues = new List<ElementType.PropertyType>();

            const int count = 5;

            for (int i = 0; i < count; i++)
            {
                var elementPropertyValue = new ElementType.PropertyType();
                guaranteedValues.Add(elementPropertyValue);
            }

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteePropertiesByFixedQuantity(guaranteedValues);

            // Assert

            Assert.AreEqual(count, result.GuaranteedPropertySetters.Single().TotalFrequency);
        }

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_Mixed_FuncAndTProperty_Default_FixedQuantity_Test()
        {
            // Arrange

            var guaranteedValues = new List<object>();

            const int count = 5;

            for (int i = 0; i < count; i++)
            {
                object elementPropertyValue = i % 2 == 0
                    ? new ElementType.PropertyType()
                    : (object)(Func<ElementType.PropertyType>)(() => new ElementType.PropertyType());

                guaranteedValues.Add(elementPropertyValue);
            }

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteePropertiesByFixedQuantity(guaranteedValues);

            // Assert

            Assert.AreEqual(count, result.GuaranteedPropertySetters.Single().TotalFrequency);
        }

    }
}
