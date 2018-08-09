using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.ReferenceParentOperableList.GuaranteePropertiesByFixedQuantity
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
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementSubType>[] { () => new ElementSubType(), () => new ElementSubType(), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertyFuncs_ExpicitQuantity_Test()
        {
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementSubType>[] { () => new ElementSubType(), () => new ElementSubType(), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertiesAndFuncs_DefaultQuantity_Test()
        {
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementSubType(), (Func<ElementSubType>)(() => new ElementSubType()), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void PropertiesAndFuncs_ExplicitQuantity_Test()
        {
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementSubType(), (Func<ElementSubType>)(() => new ElementSubType()), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void Properties_DefaultQuantity_Test()
        {
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementSubType(), new ElementSubType(), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }

        [TestMethod]
        public void Properties_ExplicitQuantity_Test()
        {
            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementSubType(), new ElementSubType(), };

            ReferenceParentOperableList<ElementSubType, RootReferenceParentOperableList<ElementType, ElementParentType>,
                ElementType, ElementParentType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5);

            actual.Populate();

            // Assert

            this.testContext.AssertTotal(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall);
        }
    }
}
