using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.ReferenceParentOperableList.GuaranteePropertiesByPercentageOfTotal
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
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] { () => new ElementType(), () => new ElementType(), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            operableList.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertyFuncs_ExpicitQuantity_Test()
        {
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] { () => new ElementType(), () => new ElementType(), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            operableList.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_DefaultQuantity_Test()
        {
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            operableList.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_ExplicitQuantity_Test()
        {
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            operableList.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_DefaultQuantity_Test()
        {
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            operableList.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_ExplicitQuantity_Test()
        {
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> actual =
                operableList.GuaranteeByPercentageOfTotal(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            operableList.Populate();

            // Assert

            this.testContext.AssertPercentage(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }
    }
}
