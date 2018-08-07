using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.RootReferenceParentFieldExpression
{
    public class TestContext
    {
        public RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementParentType> 
            RootReferenceParentFieldExpression;

        public Expression<Func<ElementType, ElementType.PropertyType>> Expression;

        public Mock<RootReferenceParentOperableList<ElementType, ElementParentType>>
            RootReferenceParentOperableListMock;

        public Mock<IObjectGraphService> ObjectGraphServiceMock;

        public TestContext()
        {
            this.Expression = element => element.AProperty;

            this.ObjectGraphServiceMock = new Mock<IObjectGraphService>();

            this.RootReferenceParentOperableListMock = Helpers
                .GetMock<RootReferenceParentOperableList<ElementType, ElementParentType>>();

            this.RootReferenceParentFieldExpression =
                new RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementParentType>
                (
                    this.Expression,
                    this.RootReferenceParentOperableListMock.Object,
                    this.ObjectGraphServiceMock.Object);
        }

        private static bool Check(GuaranteedValues guaranteedValues, int? total, int? percentage, ValueCountRequestOption option)
        {
            bool result = guaranteedValues.FrequencyPercentage == percentage &&
                          guaranteedValues.TotalFrequency == total &&
                          guaranteedValues.ValueCountRequestOption == option &&

                          guaranteedValues.Values != null &&
                          guaranteedValues.Values.Count() == 2;

            return result;
        }

        public void AssertPercentage(RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementParentType> returnResult,
            int percentage,
            ValueCountRequestOption option)
        {
            this.DoAssert(returnResult, null, percentage, option);
        }

        public void AssertTotal(RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementParentType> returnResult,
            int total,
            ValueCountRequestOption option)
        {
            this.DoAssert(returnResult, total, null, option);
        }

        private void DoAssert(
            RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementParentType> returnResult,
            int? total, int? percentage,
            ValueCountRequestOption option
        )
        {
            Assert.IsNotNull(returnResult);
            Assert.AreEqual(this.RootReferenceParentFieldExpression, returnResult);

            this.ObjectGraphServiceMock.Verify(m => m.GetObjectGraph(this.Expression));

            this.RootReferenceParentOperableListMock.Verify(
                m => m.AddGuaranteedPropertySetter(It.Is<GuaranteedValues>(
                    guaranteedValues => TestContext.Check(guaranteedValues, total, percentage, option)
                )));
        }
    }
}
