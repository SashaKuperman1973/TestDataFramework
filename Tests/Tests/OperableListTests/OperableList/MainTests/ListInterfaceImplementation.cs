using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.OperableList.MainTests
{
    [TestClass]
    public class ListInterfaceImplementation
    {
        private OperableList<ElementType> operableList;
        private TestContext testContext;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
            this.operableList = this.testContext.CreateOperableList();
        }

        [TestMethod]
        public void GetEnumeratorOfT_Test()
        {
            using (IEnumerator<RecordReference<ElementType>> enumerator = this.operableList.GetEnumerator())
            {
                enumerator.MoveNext();
                RecordReference<ElementType> reference = enumerator.Current;
                Assert.AreEqual(this.testContext.Inputs.First(), reference);
            }
        }

        [TestMethod]
        public void GetEnumerator_Test()
        {
            IEnumerator enumerator = ((IEnumerable) this.operableList).GetEnumerator();
            enumerator.MoveNext();
            object reference =  enumerator.Current;
            Assert.AreEqual(this.testContext.Inputs.First(), reference);
        }

        [TestMethod]
        public void Add_Test()
        {
            var item = Helpers.GetObject<RecordReference<ElementType>>();

            Helpers.ExceptionTest(() => this.operableList.Add(item), typeof(NotSupportedException),
                Messages.OperableListIsReadOnly);
        }

        [TestMethod]
        public void Count_Test()
        {
            Assert.AreEqual(this.testContext.Inputs.Count, this.operableList.Count);
        }

        [TestMethod]
        public void IsReadOnly_Test()
        {
            Assert.IsTrue(this.operableList.IsReadOnly);
        }

        [TestMethod]
        public void Clear_Test()
        {
            Helpers.ExceptionTest(() => this.operableList.Clear(), typeof(NotSupportedException),
                Messages.OperableListIsReadOnly);
        }

        [TestMethod]
        public void Contains_Test()
        {
            Assert.IsTrue(this.operableList.Contains(this.testContext.Inputs.First()));
            Assert.IsFalse(this.operableList.Contains(Helpers.GetObject<RecordReference<ElementType>>()));
        }

        [TestMethod]
        public void CopyTo_Test()
        {
            var array = new RecordReference<ElementType>[this.testContext.Inputs.Count];
            this.operableList.CopyTo(array, 0);

            Helpers.AssertSetsAreEqual(this.testContext.Inputs, array);
        }

        [TestMethod]
        public void Remove_Test()
        {
            Helpers.ExceptionTest(() => this.operableList.Remove(Helpers.GetObject<RecordReference<ElementType>>()),
                typeof(NotSupportedException),
                Messages.OperableListIsReadOnly);
        }

        [TestMethod]
        public void IndexOf_Test()
        {
            int index = this.operableList.IndexOf(this.testContext.Inputs[1]);
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void Insert_Test()
        {
            Helpers.ExceptionTest(() => this.operableList.Insert(0, Helpers.GetObject<RecordReference<ElementType>>()),
                typeof(NotSupportedException),
                Messages.OperableListIsReadOnly);
        }

        [TestMethod]
        public void RemoveAt_Test()
        {
            Helpers.ExceptionTest(() => this.operableList.RemoveAt(0),
                typeof(NotSupportedException),
                Messages.OperableListIsReadOnly);
        }

        [TestMethod]
        public void Index_Read_Test()
        {
            RecordReference<ElementType> reference = this.operableList[1];
            Assert.AreEqual(this.testContext.Inputs[1], reference);
        }

        [TestMethod]
        public void Index_Write_Test()
        {
            Helpers.ExceptionTest(() => this.operableList[0] = Helpers.GetObject<RecordReference<ElementType>>(),
                typeof(NotSupportedException),
                Messages.OperableListIsReadOnly);
        }
    }
}
