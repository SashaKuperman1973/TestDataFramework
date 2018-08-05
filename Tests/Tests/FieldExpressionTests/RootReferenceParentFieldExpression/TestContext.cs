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
        public RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementTypeBase> 
            ReferenceParentFieldExpression;

        public Expression<Func<ElementType, ElementType.PropertyType>> Expression;

        public Mock<RootReferenceParentOperableList<ElementType, ElementTypeBase>>
            ReferenceParentOperableListMock;

        public Mock<IObjectGraphService> ObjectGraphServiceMock;

        public TestContext()
        {
            this.Expression = element => element.AProperty;

            this.ObjectGraphServiceMock = new Mock<IObjectGraphService>();

            this.ReferenceParentOperableListMock = Helpers
                .GetMock<RootReferenceParentOperableList<ElementType, ElementTypeBase>>();

            this.ReferenceParentFieldExpression =
                new RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementTypeBase>
                (
                    this.Expression,
                    this.ReferenceParentOperableListMock.Object,
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
            RootReferenceParentFieldExpression<ElementType, ElementType.PropertyType, ElementTypeBase> returnResult,
            int quantity,
            ValueCountRequestOption option
        )
        {
            Assert.IsNotNull(returnResult);
            Assert.AreEqual(this.ReferenceParentFieldExpression, returnResult);

            this.ObjectGraphServiceMock.Verify(m => m.GetObjectGraph(this.Expression));

            this.ReferenceParentOperableListMock.Verify(
                m => m.AddGuaranteedPropertySetter(It.Is<GuaranteedValues>(
                    guaranteedValues => TestContext.Check(guaranteedValues, quantity, option)
                )));
        }

    }
}
