using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.OperableList.GuaranteePropertiesByPercentageOfTotal
{
    [TestClass]
    public class DoNotThrowCountRequestOption
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
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] { () => new ElementType(), () => new ElementType(), };

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertyFuncs_ExpicitQuantity_Test()
        {
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] { () => new ElementType(), () => new ElementType(), };

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_DefaultQuantity_Test()
        {
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_ExplicitQuantity_Test()
        {
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_DefaultQuantity_Test()
        {
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_ExplicitQuantity_Test()
        {
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }
    }
}
