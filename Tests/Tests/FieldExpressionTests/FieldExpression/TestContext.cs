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

namespace Tests.Tests.FieldExpressionTests.FieldExpression
{
    public class TestContext
    {
        public FieldExpression<ElementType, ElementType.PropertyType> FieldExpression;

        public Expression<Func<ElementType, ElementType.PropertyType>> Expression;

        public Mock<OperableListEx<ElementType>> OperableListMock;

        public Mock<IObjectGraphService> ObjectGraphServiceMock;

        public TestContext()
        {
            this.Expression = element => element.AProperty;

            this.ObjectGraphServiceMock = new Mock<IObjectGraphService>();

            this.OperableListMock = Helpers
                .GetMock<OperableListEx<ElementType>>();

            this.FieldExpression =
                new FieldExpression<ElementType, ElementType.PropertyType>(
                    this.Expression,
                    this.OperableListMock.Object,
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

        public void AssertPercentage(FieldExpression<ElementType, ElementType.PropertyType> returnResult,
            int percentage,
            ValueCountRequestOption option)
        {
            this.DoAssert(returnResult, null, percentage, option);
        }

        public void AssertTotal(FieldExpression<ElementType, ElementType.PropertyType> returnResult,
            int total,
            ValueCountRequestOption option)
        {
            this.DoAssert(returnResult, total, null, option);
        }

        private void DoAssert(
            FieldExpression<ElementType, ElementType.PropertyType> returnResult,
            int? total, int? percentage,
            ValueCountRequestOption option
        )
        {
            Assert.IsNotNull(returnResult);
            Assert.AreEqual(this.FieldExpression, returnResult);

            this.ObjectGraphServiceMock.Verify(m => m.GetObjectGraph(this.Expression));

            this.OperableListMock.Verify(
                m => m.AddGuaranteedPropertySetter(It.Is<GuaranteedValues>(
                    guaranteedValues => TestContext.Check(guaranteedValues, total, percentage, option)
                )));
        }
    }
}
