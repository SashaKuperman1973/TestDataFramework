using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.FieldExpression;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.RootReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal
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

            RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementParentType> 
                returnResult = this.testContext.RootReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new Func<ElementType.PropertyType>[]
                        {() => new ElementType.PropertyType(), () => new ElementType.PropertyType()});

            // Assert

            this.testContext.AssertPercentage(returnResult, 10, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertyFuncs_ExplicitQuantity_Test()
        {
            // Act

            RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementParentType> 
                returnResult = this.testContext.RootReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
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

            RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementParentType> 
                returnResult = this.testContext.RootReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
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

            RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementParentType> 
                returnResult = this.testContext.RootReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
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

            RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementParentType> 
                returnResult = this.testContext.RootReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new object[]
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

            RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementParentType> 
                returnResult = this.testContext.RootReferenceParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new object[]
                    {
                        new ElementType.PropertyType(),
                        new ElementType.PropertyType(),
                    }, 5);

            // Assert

            this.testContext.AssertPercentage(returnResult, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }
    }
}
