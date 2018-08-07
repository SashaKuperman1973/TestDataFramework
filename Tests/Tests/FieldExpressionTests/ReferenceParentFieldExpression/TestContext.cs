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

namespace Tests.Tests.FieldExpressionTests.ReferenceParentFieldExpression
{
    public class TestContext
    {
        public ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, OperableListEx<ElementType>, ElementType, ElementParentType> 
            ReferenceParentFieldExpression;

        public Expression<Func<ElementType, ElementType.PropertyType>> Expression;

        public Mock<ReferenceParentOperableList<ElementType, OperableListEx<ElementType>, ElementType, ElementParentType>>
            ReferenceParentOperableListMock;

        public Mock<IObjectGraphService> ObjectGraphServiceMock;

        public TestContext()
        {
            this.Expression = element => element.AProperty;

            this.ObjectGraphServiceMock = new Mock<IObjectGraphService>();

            this.ReferenceParentOperableListMock = Helpers
                .GetMock<ReferenceParentOperableList<ElementType, OperableListEx<ElementType>, ElementType, ElementParentType>>();

            this.ReferenceParentFieldExpression =
                new ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, OperableListEx<ElementType>,
                    ElementType, ElementParentType>(
                    this.Expression,
                    this.ReferenceParentOperableListMock.Object,
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

        public void AssertPercentage(ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, OperableListEx<ElementType>, ElementType, ElementParentType> returnResult,
            int percentage,
            ValueCountRequestOption option)
        {
            this.DoAssert(returnResult, null, percentage, option);
        }

        public void AssertTotal(ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, OperableListEx<ElementType>, ElementType, ElementParentType> returnResult,
            int total,
            ValueCountRequestOption option)
        {
            this.DoAssert(returnResult, total, null, option);
        }

        private void DoAssert(
            ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, OperableListEx<ElementType>, ElementType, ElementParentType> returnResult,
            int? total, int? percentage,
            ValueCountRequestOption option
        )
        {
            Assert.IsNotNull(returnResult);
            Assert.AreEqual(this.ReferenceParentFieldExpression, returnResult);

            this.ObjectGraphServiceMock.Verify(m => m.GetObjectGraph(this.Expression));

            this.ReferenceParentOperableListMock.Verify(
                m => m.AddGuaranteedPropertySetter(It.Is<GuaranteedValues>(
                    guaranteedValues => TestContext.Check(guaranteedValues, total, percentage, option)
                )));
        }
    }
}
