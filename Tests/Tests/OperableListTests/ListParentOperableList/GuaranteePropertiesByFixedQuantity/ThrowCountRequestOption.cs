using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.ListParentOperableList.GuaranteePropertiesByFixedQuantity
{
    [TestClass]
    public class ThrowCountRequestOption
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
            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> operableList =
                this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] { () => new ElementType(), () => new ElementType(), };

            ListParentOperableList<ElementType,
                OperableListEx<ElementParentType>, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues);

            operableList.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 2,
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertyFuncs_ExpicitQuantity_Test()
        {
            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] { () => new ElementType(), () => new ElementType(), };

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5);

            operableList.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertiesAndFuncs_DefaultQuantity_Test()
        {
            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues);

            operableList.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertiesAndFuncs_ExplicitQuantity_Test()
        {
            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5);

            operableList.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void Properties_DefaultQuantity_Test()
        {
            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues);

            operableList.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void Properties_ExplicitQuantity_Test()
        {
            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5);

            operableList.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }
    }
}
