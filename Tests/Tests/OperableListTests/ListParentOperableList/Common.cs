using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.MakeableEnumerable;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.ListParentOperableList
{
    [TestClass]
    public class Common
    {
        private TestContext testContext;
        private ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> operableList;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
            this.operableList = this.testContext.CreateOperableList();
        }

        [TestMethod]
        public void RootList_Test()
        {
            // Act

            OperableListEx<ElementParentType> rootList = this.operableList.RootList;

            // Assert

            Assert.IsNotNull(rootList);
            Assert.AreEqual(this.testContext.RootListMock.Object, rootList);
        }

        [TestMethod]
        public void ParentList_Test()
        {
            OperableListEx<ElementParentType> parentList = this.operableList.ParentList;

            // Assert

            Assert.IsNotNull(parentList);
            Assert.AreEqual(this.testContext.ParentListMock.Object, parentList);
        }

        [TestMethod]
        public void Make_Test()
        {
            // Arrange

            var rootObjects = new ElementParentType[0];
            this.testContext.RootListMock.Setup(m => m.RecordObjects).Returns(rootObjects);

            // Act

            IEnumerable<ElementParentType> result = this.operableList.Make();

            // Assert

            this.testContext.RootListMock.Verify(m => m.Populate());
            Assert.IsNotNull(result);
            Assert.AreEqual(rootObjects, result);
        }

        [TestMethod]
        public void BindAndMake_Test()
        {
            // Arrange

            var rootObjects = new ElementParentType[0];
            this.testContext.RootListMock.Setup(m => m.RecordObjects).Returns(rootObjects);

            // Act

            IEnumerable<ElementParentType> result = this.operableList.BindAndMake();

            // Assert

            this.testContext.PopulatorMock.Verify(m => m.Bind());
            Assert.IsNotNull(result);
            Assert.AreEqual(rootObjects, result);
        }

        [TestMethod]
        public void Take_Test()
        {
            // Arrange

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> result =
                this.operableList.Take(2);

            // Assert

            Helpers.AssertSetsAreEqual(this.testContext.Inputs.Take(2), result);
        }

        [TestMethod]
        public void Skip_Test()
        {
            // Arrange

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> result =
                this.operableList.Skip(1);

            // Assert

            Helpers.AssertSetsAreEqual(this.testContext.Inputs.Skip(1), result);
        }

        [TestMethod]
        public void Set_Value_Test()
        {
            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> result =
                this.operableList.Set(m => m.AProperty, new ElementType.PropertyType());

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Set_ValueFactory_Test()
        {
            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> result =
                this.operableList.Set(m => m.AProperty, () => new ElementType.PropertyType());

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SelectListSet_Test()
        {
            // Arrange

            this.testContext.ObjectGraphServiceMock.Setup(
                m => m.GetObjectGraph(
                    It.IsAny<Expression<Func<ElementType, ElementType.PropertyType>>>())
            ).Returns(new[] {typeof(ElementType).GetProperty(nameof(ElementType.AnEnumerable))}.ToList);

            // Act

            ListParentMakeableEnumerable<ListParentOperableList<ElementType.PropertyType,
                    ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType>,
                    ElementParentType>, ElementParentType,
                ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType>> result =
                this.operableList.SelectListSet(m => m.AnEnumerable, 4);

            // Assert

            Assert.AreEqual(this.operableList.Count, result.Count);
            result.ForEach(r => Assert.AreEqual(4, r.Count));
        }

        [TestMethod]
        public void Ignore_Test()
        {
            // Act

            ListParentOperableList<ElementType, OperableListEx<ElementParentType>, ElementParentType> result =
                this.operableList.Ignore(m => m.AProperty);

            // Assert

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void Set_ForAnIndividualProperty_Test()
        {
            // Act

            ListParentFieldExpression<ElementType, OperableListEx<ElementParentType>, ElementParentType,
                ElementType.PropertyType> fieldExpression = this.operableList.Set(m => m.AProperty);

            // Assert

            Assert.IsNotNull(fieldExpression);
        }
    }
}
