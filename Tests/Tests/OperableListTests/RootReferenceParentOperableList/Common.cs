using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.MakeableEnumerable;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.RootReferenceParentOperableList
{
    [TestClass]
    public class Common
    {
        private TestContext testContext;
        private RootReferenceParentOperableList<ElementType, ElementParentType> operableList;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
            this.operableList = this.testContext.CreateOperableList();
        }

        [TestMethod]
        public void Make_Test()
        {
            // Arrange

            this.testContext.RootMock.SetupGet(m => m.RecordObject).Returns(new ElementParentType());

            // Act

            ElementParentType result = this.operableList.Make();

            // Assert

            this.testContext.RootMock.Verify(m => m.Populate());

            Assert.IsNotNull(result);
            Assert.AreEqual(this.testContext.RootMock.Object.RecordObject, result);
        }

        [TestMethod]
        public void BindAndMake_Test()
        {
            // Arrange

            this.testContext.RootMock.SetupGet(m => m.RecordObject).Returns(new ElementParentType());

            // Act

            ElementParentType result = this.operableList.BindAndMake();

            // Assert

            this.testContext.PopulatorMock.Verify(m => m.Bind());

            Assert.IsNotNull(result);
            Assert.AreEqual(this.testContext.RootMock.Object.RecordObject, result);
        }

        [TestMethod]
        public void Take_Test()
        {
            // Arrange

            ReferenceParentOperableList<ElementType, RootReferenceParentOperableList<ElementType, ElementParentType>,
                ElementType, ElementParentType> result = this.operableList.Take(2);

            // Assert

            Helpers.AssertSetsAreEqual(this.testContext.Inputs.Take(2), result);
        }

        [TestMethod]
        public void Skip_Test()
        {
            // Arrange

            ReferenceParentOperableList<ElementType, RootReferenceParentOperableList<ElementType, ElementParentType>,
                ElementType, ElementParentType> result = this.operableList.Skip(1);

            // Assert

            Helpers.AssertSetsAreEqual(this.testContext.Inputs.Skip(1), result);
        }

        [TestMethod]
        public void Set_Value_Test()
        {
            RootReferenceParentOperableList<ElementType, ElementParentType> result =
                this.operableList.Set(m => m.AProperty, new ElementType.PropertyType());

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Set_ValueFactory_Test()
        {
            RootReferenceParentOperableList<ElementType, ElementParentType> result =
                this.operableList.Set(m => m.AProperty, () => new ElementType.PropertyType());

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SelectListSet_Test()
        {
            // Arrange

            this.testContext.ObjectGraphServiceMock.Setup(
                m => m.GetObjectGraph(
                    It.IsAny<Expression<Func<ElementSubType, IEnumerable<int>>>>())
            ).Returns(new[] {typeof(ElementType).GetProperty(nameof(ElementType.AnEnumerable))}.ToList);

            // Act

            var result = this.operableList.SelectListSet(m => m.AnEnumerable, 4);

            // Assert

            Assert.AreEqual(this.operableList.Count, result.Count);
            result.ForEach(r => Assert.AreEqual(4, r.Count));
        }

        [TestMethod]
        public void Ignore_Test()
        {
            // Act

            RootReferenceParentOperableList<ElementType, ElementParentType> result =
                this.operableList.Ignore(m => m.AProperty);

            // Assert

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void Set_ForAnIndividualProperty_Test()
        {
            // Act

            var fieldExpression = this.operableList.Set(m => m.AProperty);

            // Assert

            Assert.IsNotNull(fieldExpression);
        }
    }
}
