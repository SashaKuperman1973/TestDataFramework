using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.ListParentTests
{
    [TestClass]
    public partial class ListParentFieldExpressionTests
    {
        private Expression<Func<TestModels.ElementType, TestModels.ElementType.PropertyType>> expression;

        private ListParentFieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType> listParentFieldExpression;

        private Mock<ListParentOperableList<TestModels.ElementType>> listParentOperableListMock;

        private Mock<IObjectGraphService> objectGraphServiceMock;

        [TestInitialize]
        public void Initialize()
        {
            var valueGuaranteePopulatorMock = new Mock<ValueGuaranteePopulator>();

            this.expression = element => element.AProperty;

            this.objectGraphServiceMock = new Mock<IObjectGraphService>();

            this.listParentOperableListMock =
                new Mock<ListParentOperableList<TestModels.ElementType>>(valueGuaranteePopulatorMock.Object, null, null, null, null, null, null);
            this.listParentFieldExpression =
                new ListParentFieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType>(this.expression,
                    this.listParentOperableListMock.Object, this.objectGraphServiceMock.Object);
        }

        [TestMethod]
        public void RecordObjects_Test()
        {
            // Arrange
            var element = new TestModels.ElementType();
            this.listParentOperableListMock.SetupGet(m => m.RecordObjects).Returns(new[] {element});

            // Act

            IEnumerable<TestModels.ElementType> result = this.listParentFieldExpression.RecordObjects;

            // Assert

            Assert.IsNotNull(result);

            TestModels.ElementType[] resultArray = result.ToArray();

            Assert.AreEqual(1, resultArray.Length);
            Assert.AreEqual(element, resultArray[0]);
        }

        [TestMethod]
        public void Make_Test()
        {
            // Act

            this.listParentFieldExpression.Make();

            // Assert

            this.listParentOperableListMock.Verify(m => m.Make());
        }

        [TestMethod]
        public void BindAndMake()
        {
            // Act

            this.listParentFieldExpression.BindAndMake();

            // Assert

            this.listParentOperableListMock.Verify(m => m.BindAndMake());
        }
    }
}
