using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.ReferenceParentTests
{
    [TestClass]
    public partial class ReferenceParentFieldExpressionTests
    {
        private Expression<Func<ElementType, ElementType.PropertyType>> expression;

        private ReferenceParentFieldExpression<ElementType, ElementType.PropertyType,
                RootReferenceParentOperableList<ElementType, SubjectClass>, ElementType, SubjectClass>
            referenceParentFieldExpression;

        private Mock<ReferenceParentOperableList<ElementType,
                RootReferenceParentOperableList<ElementType, SubjectClass>, ElementType, SubjectClass>>
            referenceParentOperableListMock;

        private Mock<IObjectGraphService> objectGraphServiceMock;

        [TestInitialize]
        public void Initialize()
        {
            this.expression = element => element.AProperty;

            this.objectGraphServiceMock = new Mock<IObjectGraphService>();

            this.referenceParentOperableListMock = Helpers.GetMock<ReferenceParentOperableList<ElementType,
                RootReferenceParentOperableList<ElementType, SubjectClass>, ElementType, SubjectClass>>();

            this.referenceParentFieldExpression =
                new ReferenceParentFieldExpression<ElementType, ElementType.PropertyType,
                    RootReferenceParentOperableList<ElementType, SubjectClass>, ElementType, SubjectClass>(
                    this.expression,
                    this.referenceParentOperableListMock.Object,
                    this.objectGraphServiceMock.Object);
        }

        [TestMethod]
        public void RecordObjects_Test()
        {
            // Arrange
            var element = new ElementType();
            this.referenceParentOperableListMock.SetupGet(m => m.RecordObjects).Returns(new[] {element});

            // Act

            IEnumerable<ElementType> result = this.referenceParentFieldExpression.RecordObjects;

            // Assert

            Assert.IsNotNull(result);

            ElementType[] resultArray = result.ToArray();

            Assert.AreEqual(1, resultArray.Length);
            Assert.AreEqual(element, resultArray[0]);
        }

        [TestMethod]
        public void Make_Test()
        {
            // Act

            this.referenceParentFieldExpression.Make();

            // Assert

            this.referenceParentOperableListMock.Verify(m => m.Make());
        }

        [TestMethod]
        public void BindAndMake()
        {
            // Act

            this.referenceParentFieldExpression.BindAndMake();

            // Assert

            this.referenceParentOperableListMock.Verify(m => m.BindAndMake());
        }
    }
}
