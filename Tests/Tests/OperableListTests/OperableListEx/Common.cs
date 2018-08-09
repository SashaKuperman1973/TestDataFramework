using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.MakeableEnumerable;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.OperableListEx
{
    [TestClass]
    public class Common
    {
        private TestContext testContext;
        private OperableListEx<ElementType> operableList;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
            this.operableList = this.testContext.CreateOperableList();
        }

        [TestMethod]
        public void Make_Test()
        {
            // Act

            IEnumerable<ElementType> result = this.operableList.Make();

            // Assert

            this.testContext.InputMocks.ForEach(m => m.Verify(n => n.Populate()));

            Assert.IsNotNull(result);
            Helpers.AssertSetsAreEqual(this.testContext.InputObjects, result);
        }

        [TestMethod]
        public void BindAndMake_Test()
        {
            // Act

            IEnumerable<ElementType> result = this.operableList.BindAndMake();

            // Assert

            this.testContext.PopulatorMock.Verify(m => m.Bind());

            Assert.IsNotNull(result);
            Helpers.AssertSetsAreEqual(this.testContext.InputObjects, result);
        }

        [TestMethod]
        public void Take_Test()
        {
            // Arrange

            ListParentOperableList<ElementType, OperableListEx<ElementType>, ElementType> result = this.operableList.Take(2);

            // Assert

            Helpers.AssertSetsAreEqual(this.testContext.Inputs.Take(2), result);
        }

        [TestMethod]
        public void Skip_Test()
        {
            // Arrange

            ListParentOperableList<ElementType, OperableListEx<ElementType>, ElementType> result = this.operableList.Skip(1);

            // Assert

            Helpers.AssertSetsAreEqual(this.testContext.Inputs.Skip(1), result);
        }

        [TestMethod]
        public void Set_Value_Test()
        {
            OperableListEx<ElementType> result = this.operableList.Set(m => m.AProperty, new ElementType.PropertyType());

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Set_ValueFactory_Test()
        {
            OperableListEx<ElementType> result = this.operableList.Set(m => m.AProperty, () => new ElementType.PropertyType());

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

            ListParentMakeableEnumerable<ListParentOperableList<ElementType.PropertyType, OperableListEx<ElementType>,
                ElementType>, ElementType> result = this.operableList.SelectListSet(m => m.AnEnumerable, 4);

            // Assert

            Assert.AreEqual(this.operableList.Count, result.Count);
            result.ForEach(r => Assert.AreEqual(4, r.Count));
        }

        [TestMethod]
        public void Ignore_Test()
        {
            // Act

            OperableListEx<ElementType> result = this.operableList.Ignore(m => m.AProperty);

            // Assert

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void Set_ForAnIndividualProperty_Test()
        {
            // Act

            FieldExpression<ElementType, ElementType.PropertyType> fieldExpression = this.operableList.Set(m => m.AProperty);

            // Assert

            Assert.IsNotNull(fieldExpression);
        }
    }
}
