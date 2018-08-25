using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests
{
    [TestClass]
    public class OperationReferenceParentOperableListTests
    {
        private List<RecordReference<ElementType>> input;

        private void AddInputs(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.input.Add(Helpers.GetObject<RecordReference<ElementType>>());
            }
        }

        private ReferenceParentOperableList<
            ElementType,
            ReferenceParentOperableList<ElementType.PropertyType,
                RootReferenceParentOperableList<ElementType, ElementParentType>,
                ElementType, ElementParentType>,
            ElementType, ElementParentType> preOperationList;

        private Func<OperationReferenceParentOperableList<ElementType,
            ReferenceParentOperableList<
                ElementType.PropertyType,
                RootReferenceParentOperableList<ElementType, ElementParentType>,
                ElementType,
                ElementParentType>,
            ElementType,
            ElementParentType>> createList;

        [TestInitialize]
        public void Initialize()
        {
            this.input = new List<RecordReference<ElementType>>();

            this.preOperationList = new ReferenceParentOperableList<
                ElementType,
                ReferenceParentOperableList<ElementType.PropertyType,
                    RootReferenceParentOperableList<ElementType, ElementParentType>,
                    ElementType, ElementParentType>,
                ElementType, ElementParentType>(
                rootList: null,
                root: null,
                parentList: null,
                input: this.input,
                valueGuaranteePopulator: null,
                populator: null,
                objectGraphService: null,
                attributeDecorator: null,
                deepCollectionSettingConverter: null,
                typeGenerator: null,
                isShallowCopy: false);

            this.createList = () => new OperationReferenceParentOperableList<ElementType,
                ReferenceParentOperableList<
                    ElementType.PropertyType,
                    RootReferenceParentOperableList<ElementType, ElementParentType>,
                    ElementType,
                    ElementParentType>,
                ElementType,
                ElementParentType>(
                rootList: null,
                root: null,
                parentList: null,
                preOperationList: this.preOperationList,
                input: this.input,
                valueGuaranteePopulator: null,
                populator: null,
                objectGraphService: null,
                attributeDecorator: null,
                deepCollectionSettingConverter: null,
                typeGenerator: null,
                isShallowCopy: false);
        }

        [TestMethod]
        public void Take_Test()
        {
            this.AddInputs(7);

            ShortReferenceParentOperableList<ElementType, ReferenceParentOperableList<ElementType.PropertyType,
                    RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType>,
                ElementType, ElementParentType> result = this.createList().Take(4);

            Assert.AreEqual(4, result.Count);
            Helpers.AssertSetsAreEqual(this.input.Take(4), result);

            Assert.AreEqual(this.preOperationList, result.ParentList);
        }

        [TestMethod]
        public void Skip_Test()
        {
            this.AddInputs(7);

            ShortReferenceParentOperableList<ElementType, ReferenceParentOperableList<ElementType.PropertyType,
                    RootReferenceParentOperableList<ElementType, ElementParentType>, ElementType, ElementParentType>,
                ElementType, ElementParentType> result = this.createList().Skip(4);

            Assert.AreEqual(3, result.Count);
            Helpers.AssertSetsAreEqual(this.input.Skip(4), result);

            Assert.AreEqual(this.preOperationList, result.ParentList);
        }
    }
}
