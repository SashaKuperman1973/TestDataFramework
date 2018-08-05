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

namespace Tests.Tests.FieldExpressionTests.ListParentFieldExpression
{
    public class TestContext
    {
        public ListParentFieldExpression<ElementType, OperableListEx<ElementType>, ElementType,
            ElementType.PropertyType> ListParentFieldExpression;

        public Expression<Func<ElementType, ElementType.PropertyType>> Expression;

        public Mock<ListParentOperableList<ElementType, OperableListEx<ElementType>, ElementType>> ListParentOperableListMock;

        public Mock<IObjectGraphService> ObjectGraphServiceMock;

        public TestContext()
        {
            this.Expression = element => element.AProperty;

            this.ObjectGraphServiceMock = new Mock<IObjectGraphService>();

            this.ListParentOperableListMock = Helpers
                .GetMock<ListParentOperableList<ElementType, OperableListEx<ElementType>, ElementType>>();

            this.ListParentFieldExpression =
                new ListParentFieldExpression<ElementType, OperableListEx<ElementType>, ElementType,
                    ElementType.PropertyType>(
                    this.Expression,
                    this.ListParentOperableListMock.Object,
                    this.ObjectGraphServiceMock.Object);
        }

        private static bool Check(GuaranteedValues guaranteedValues, int count, ValueCountRequestOption option)
        {
            bool result = guaranteedValues.FrequencyPercentage == null &&
                          guaranteedValues.TotalFrequency == count &&
                          guaranteedValues.ValueCountRequestOption == option &&

                          guaranteedValues.Values != null &&
                          guaranteedValues.Values.Count() == 2;

            return result;
        }

        public void DoAssert(
            ListParentFieldExpression<ElementType, OperableListEx<ElementType>, ElementType, ElementType.PropertyType> returnResult,
            int quantity,
            ValueCountRequestOption option
        )
        {
            Assert.IsNotNull(returnResult);
            Assert.AreEqual(this.ListParentFieldExpression, returnResult);

            this.ObjectGraphServiceMock.Verify(m => m.GetObjectGraph(this.Expression));

            this.ListParentOperableListMock.Verify(
                m => m.AddGuaranteedPropertySetter(It.Is<GuaranteedValues>(
                    guaranteedValues => TestContext.Check(guaranteedValues, quantity, option)
                )));
        }

    }
}
