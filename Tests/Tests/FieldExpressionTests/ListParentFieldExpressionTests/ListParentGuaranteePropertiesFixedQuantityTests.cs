using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.FieldExpression;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.ListParentFieldExpressionTests
{
    public partial class ListParentFieldExpressionTests
    {
        private static bool Check(GuaranteedValues guaranteedValues, int count)
        {
            bool result = guaranteedValues.FrequencyPercentage == null &&
                          guaranteedValues.TotalFrequency == count &&
                          guaranteedValues.ValueCountRequestOption ==
                          ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall &&

                          guaranteedValues.Values != null &&
                          guaranteedValues.Values.Count() == 2;

            return result;
        }

        [TestMethod]
        public void PropertyFuncs_DefaultQuantity_Test()
        {
            // Act

            ListParentFieldExpression<ElementType, OperableList<ElementType>, ElementType, ElementType.PropertyType>
                returnResult = this.listParentFieldExpression.GuaranteePropertiesByFixedQuantity(
                    new Func<ElementType.PropertyType>[]
                        {() => new ElementType.PropertyType(), () => new ElementType.PropertyType()});

            // Assert

            Assert.IsNotNull(returnResult);
            Assert.AreEqual(this.listParentFieldExpression, returnResult);

            this.objectGraphServiceMock.Verify(m => m.GetObjectGraph(this.expression));

            this.listParentOperableListMock.Verify(m => m.AddGuaranteedPropertySetter(It.IsAny<GuaranteedValues>()));

            this.listParentOperableListMock.Verify(
                m => m.AddGuaranteedPropertySetter(It.Is<GuaranteedValues>(
                    guaranteedValues => ListParentFieldExpressionTests.Check(guaranteedValues, 2)
                )));
        }

        [TestMethod]
        public void PropertyFuncs_ExplicitQuantity_Test()
        {
            // Act

            ListParentFieldExpression<ElementType, OperableList<ElementType>, ElementType, ElementType.PropertyType>
                returnResult = this.listParentFieldExpression.GuaranteePropertiesByFixedQuantity(
                    new Func<ElementType.PropertyType>[]
                        {() => new ElementType.PropertyType(), () => new ElementType.PropertyType()},
                    5);

            // Assert

            Assert.IsNotNull(returnResult);
            Assert.AreEqual(this.listParentFieldExpression, returnResult);

            this.objectGraphServiceMock.Verify(m => m.GetObjectGraph(this.expression));

            this.listParentOperableListMock.Verify(m => m.AddGuaranteedPropertySetter(It.IsAny<GuaranteedValues>()));

            this.listParentOperableListMock.Verify(
                m => m.AddGuaranteedPropertySetter(It.Is<GuaranteedValues>(
                    guaranteedValues => ListParentFieldExpressionTests.Check(guaranteedValues, 5)
                )));
        }
    }
}
