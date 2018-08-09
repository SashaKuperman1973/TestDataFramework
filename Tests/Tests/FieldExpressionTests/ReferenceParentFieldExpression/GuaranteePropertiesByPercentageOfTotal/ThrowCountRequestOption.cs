using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.ReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal
{
    [TestClass]
    public class Throw
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

            ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, OperableListEx<ElementType>, ElementType, ElementParentType>
                returnResult = this.testContext.ReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new Func<ElementType.PropertyType>[]
                        {() => new ElementType.PropertyType(), () => new ElementType.PropertyType()});

            // Assert

            this.testContext.AssertPercentage(returnResult, 10, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertyFuncs_ExplicitQuantity_Test()
        {
            // Act

            ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, OperableListEx<ElementType>, ElementType, ElementParentType>
                returnResult = this.testContext.ReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new Func<ElementType.PropertyType>[]
                        {() => new ElementType.PropertyType(), () => new ElementType.PropertyType()},
                    5);

            // Assert

            this.testContext.AssertPercentage(returnResult, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }
        
        [TestMethod]
        public void PropertiesAndFuncs_DefaultQuantity_Test()
        {
            // Act

            ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, OperableListEx<ElementType>, ElementType, ElementParentType>
                returnResult = this.testContext.ReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new object[]
                    {
                        new ElementType.PropertyType(),
                        (Func<ElementType.PropertyType>) (() => new ElementType.PropertyType())
                    });

            // Assert

            this.testContext.AssertPercentage(returnResult, 10, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertiesAndFuncs_ExplicitQuantity_Test()
        {
            // Act

            ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, OperableListEx<ElementType>, ElementType, ElementParentType>
                returnResult = this.testContext.ReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new object[]
                    {
                        new ElementType.PropertyType(),
                        (Func<ElementType.PropertyType>) (() => new ElementType.PropertyType())
                    }, 5);

            // Assert

            this.testContext.AssertPercentage(returnResult, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void Properties_DefaultQuantity_Test()
        {
            // Act

            ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, OperableListEx<ElementType>, ElementType, ElementParentType>
                returnResult = this.testContext.ReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new []
                    {
                        new ElementType.PropertyType(),
                        new ElementType.PropertyType(),
                    });

            // Assert

            this.testContext.AssertPercentage(returnResult, 10, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void Properties_ExplicitQuantity_Test()
        {
            // Act

            ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, OperableListEx<ElementType>, ElementType, ElementParentType>
                returnResult = this.testContext.ReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new []
                    {
                        new ElementType.PropertyType(),
                        new ElementType.PropertyType(),
                    }, 5);

            // Assert

            this.testContext.AssertPercentage(returnResult, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }
    }
}
