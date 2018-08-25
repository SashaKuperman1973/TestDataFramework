using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Moq;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.MakeableEnumerable;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.MakeableEnumerableTests
{
    [TestClass]
    public class ReferenceParentMakeableEnumerableTests
    {
        private List<List<ElementType.PropertyType>> setOfLists;
        private Mock<RecordReference<ElementParentType>> makeableMock;
        private Mock<IEnumerable<ElementType>> parentListMock;
        private Mock<RootReferenceParentOperableList<ElementType, ElementParentType>> rootListMock;

        private Func<ReferenceParentMakeableEnumerable<List<ElementType.PropertyType>, ElementParentType,
            IEnumerable<ElementType>, ElementType>> createMakeableEnumerable;

        [TestInitialize]
        public void Initialize()
        {
            this.setOfLists = new List<List<ElementType.PropertyType>>
            {
                new List<ElementType.PropertyType>(),
                new List<ElementType.PropertyType>(),
                new List<ElementType.PropertyType>(),
            };

            this.makeableMock = Helpers.GetMock<RecordReference<ElementParentType>>();
            this.parentListMock = new Mock<IEnumerable<ElementType>>();
            this.rootListMock = Helpers.GetMock<RootReferenceParentOperableList<ElementType, ElementParentType>>();

            this.createMakeableEnumerable = () =>
                new ReferenceParentMakeableEnumerable<List<ElementType.PropertyType>, ElementParentType,
                    IEnumerable<ElementType>, ElementType>(this.setOfLists,
                    this.makeableMock.Object, this.parentListMock.Object, this.rootListMock.Object);
        }

        [TestMethod]
        public void Make_Test()
        {
            // Act

            this.createMakeableEnumerable().Make();

            // Assert

            this.makeableMock.Verify(m => m.Make());
        }

        [TestMethod]
        public void BindAndMake_Test()
        {
            // Act

            this.createMakeableEnumerable().BindAndMake();

            // Assert

            this.makeableMock.Verify(m => m.BindAndMake());
        }

        [TestMethod]
        public void Set_Test()
        {
            // Act

            ReferenceParentMakeableEnumerable<List<ElementType.PropertyType>, ElementParentType,
                IEnumerable<ElementType>, ElementType> makeableEnumerable = this.createMakeableEnumerable();

            int count = 0;
            ReferenceParentMakeableEnumerable<List<ElementType.PropertyType>, ElementParentType, IEnumerable<ElementType>, ElementType> result =
                makeableEnumerable.Set(m =>
                {
                    count++;
                    return new SubjectClass();
                });

            // Assert

            Assert.AreEqual(3, count);
            Assert.AreEqual(makeableEnumerable, result);
        }

        [TestMethod]
        public void Take_Test()
        {
            // Act

            ReferenceParentMakeableEnumerable<List<ElementType.PropertyType>, ElementParentType,
                IEnumerable<ElementType>, ElementType> result = this.createMakeableEnumerable().Take(3);

            // Assert

            Assert.AreEqual(3, result.Count);

            Helpers.AssertSetsAreEqual(this.setOfLists, result);
        }

        [TestMethod]
        public void Skip_Test()
        {
            // Arrange

            // Add 3 more elements
            this.setOfLists.Add(new List<ElementType.PropertyType>());
            this.setOfLists.Add(new List<ElementType.PropertyType>());
            this.setOfLists.Add(new List<ElementType.PropertyType>());

            // Act

            ReferenceParentMakeableEnumerable<List<ElementType.PropertyType>, ElementParentType,
                IEnumerable<ElementType>, ElementType> result = this.createMakeableEnumerable().Skip(3);

            // Assert

            Assert.AreEqual(3, result.Count);

            Helpers.AssertSetsAreEqual(this.setOfLists.Skip(3), result);
        }
    }
}
