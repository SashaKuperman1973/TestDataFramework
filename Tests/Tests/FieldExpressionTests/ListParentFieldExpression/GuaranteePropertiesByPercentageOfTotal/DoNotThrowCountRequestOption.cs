using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.ListParentFieldExpression.GuaranteePropertiesByPercentageOfTotal
{
    [TestClass]
    public class DoNotThrow
    {
        private TestContext testContext;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
        }

        [TestMethod]
        public void PropertyFuncs_DefaultQuantity_Test()
        {
            // Act

            ListParentFieldExpression<ElementType, OperableListEx<ElementType>, ElementType, ElementType.PropertyType>
                returnResult = this.testContext.ListParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new Func<ElementType.PropertyType>[]
                        {() => new ElementType.PropertyType(), () => new ElementType.PropertyType()},
                    ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertyFuncs_ExplicitQuantity_Test()
        {
            // Act

            ListParentFieldExpression<ElementType, OperableListEx<ElementType>, ElementType, ElementType.PropertyType>
                returnResult = this.testContext.ListParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new Func<ElementType.PropertyType>[]
                        {() => new ElementType.PropertyType(), () => new ElementType.PropertyType()},
                    5, ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_DefaultQuantity_Test()
        {
            // Act

            ListParentFieldExpression<ElementType, OperableListEx<ElementType>, ElementType, ElementType.PropertyType>
                returnResult = this.testContext.ListParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new object[]
                    {
                        new ElementType.PropertyType(),
                        (Func<ElementType.PropertyType>) (() => new ElementType.PropertyType())
                    }, ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_ExplicitQuantity_Test()
        {
            // Act

            ListParentFieldExpression<ElementType, OperableListEx<ElementType>, ElementType, ElementType.PropertyType>
                returnResult = this.testContext.ListParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new object[]
                    {
                        new ElementType.PropertyType(),
                        (Func<ElementType.PropertyType>) (() => new ElementType.PropertyType())
                    }, 5, ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_DefaultQuantity_Test()
        {
            // Act

            ListParentFieldExpression<ElementType, OperableListEx<ElementType>, ElementType, ElementType.PropertyType>
                returnResult = this.testContext.ListParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new object[]
                    {
                        new ElementType.PropertyType(),
                        new ElementType.PropertyType(),
                    }, ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_ExplicitQuantity_Test()
        {
            // Act

            ListParentFieldExpression<ElementType, OperableListEx<ElementType>, ElementType, ElementType.PropertyType>
                returnResult = this.testContext.ListParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new object[]
                    {
                        new ElementType.PropertyType(),
                        new ElementType.PropertyType(),
                    }, 5, ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 5, ValueCountRequestOption.DoNotThrow);
        }
    }
}