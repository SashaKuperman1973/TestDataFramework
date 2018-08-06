using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.ListParentOperableList.GuaranteePropertiesByPercentageOfTotal
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
            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType, OperableListEx<ElementTypeBase>, ElementTypeBase> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] { () => new ElementType(), () => new ElementType(), };

            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType, OperableListEx<ElementTypeBase>, ElementTypeBase> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            actual.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertyFuncs_ExpicitQuantity_Test()
        {
            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType, OperableListEx<ElementTypeBase>, ElementTypeBase> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] { () => new ElementType(), () => new ElementType(), };

            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType, OperableListEx<ElementTypeBase>, ElementTypeBase> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            actual.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_DefaultQuantity_Test()
        {
            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType,
                OperableListEx<ElementTypeBase>, ElementTypeBase> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType, OperableListEx<ElementTypeBase>, ElementTypeBase> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            actual.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_ExplicitQuantity_Test()
        {
            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType, OperableListEx<ElementTypeBase>, ElementTypeBase> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType, OperableListEx<ElementTypeBase>, ElementTypeBase> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            actual.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_DefaultQuantity_Test()
        {
            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType, OperableListEx<ElementTypeBase>, ElementTypeBase> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType, OperableListEx<ElementTypeBase>, ElementTypeBase> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            actual.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_ExplicitQuantity_Test()
        {
            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType, OperableListEx<ElementTypeBase>, ElementTypeBase> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            TestDataFramework.Populator.Concrete.OperableList.ListParentOperableList<ElementType, OperableListEx<ElementTypeBase>, ElementTypeBase> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            actual.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }
    }
}
