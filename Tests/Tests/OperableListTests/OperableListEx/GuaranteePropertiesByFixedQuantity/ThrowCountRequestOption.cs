using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.OperableListEx.GuaranteePropertiesByFixedQuantity
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
            OperableListEx<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] { () => new ElementType(), () => new ElementType(), };

            OperableListEx<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertyFuncs_ExpicitQuantity_Test()
        {
            OperableListEx<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] { () => new ElementType(), () => new ElementType(), };

            OperableListEx<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertiesAndFuncs_DefaultQuantity_Test()
        {
            OperableListEx<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            OperableListEx<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertiesAndFuncs_ExplicitQuantity_Test()
        {
            OperableListEx<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            OperableListEx<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void Properties_DefaultQuantity_Test()
        {
            OperableListEx<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            OperableListEx<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void Properties_ExplicitQuantity_Test()
        {
            OperableListEx<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            OperableListEx<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }
    }
}
