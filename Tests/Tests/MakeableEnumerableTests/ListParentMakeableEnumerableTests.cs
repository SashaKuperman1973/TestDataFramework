using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Populator.Concrete.MakeableEnumerable;
using Tests.TestModels;
using TestDataFramework.Populator.Concrete.OperableList;

namespace Tests.Tests.MakeableEnumerableTests
{
    [TestClass]
    public class ListParentMakeableEnumerableTests
    {
        [TestMethod]
        public void Make_Test()
        {
            // Arrange

            var integerList = new List<int>();
            var setOfLists = new List<List<int>> {integerList};

            var makeableCollecitonMock = Helpers.GetMock<OperableListEx<SubjectClass>>();

            var makeableEnumerable =
                new ListParentMakeableEnumerable<List<int>, SubjectClass, OperableListEx<SubjectClass>>(
                    setOfLists, makeableCollecitonMock.Object, makeableCollecitonMock.Object);

            // Act

            makeableEnumerable.Make();

            // Assert

            makeableCollecitonMock.Verify(m => m.Make());
        }

        [TestMethod]
        public void BindAndMake_Test()
        {
            // Arrange

            var integerList = new List<int>();
            var setOfLists = new List<List<int>> { integerList };

            var makeableCollecitonMock = Helpers.GetMock<OperableListEx<SubjectClass>>();

            var makeableEnumerable =
                new ListParentMakeableEnumerable<List<int>, SubjectClass, OperableListEx<SubjectClass>>(
                    setOfLists, makeableCollecitonMock.Object, makeableCollecitonMock.Object);

            // Act

            makeableEnumerable.BindAndMake();

            // Assert

            makeableCollecitonMock.Verify(m => m.BindAndMake());
        }

        [TestMethod]
        public void Set_Test()
        {
            // Arrange

            var setOfLists = new List<List<int>> {new List<int>(), new List<int>()};
            var makeableCollecitonMock = Helpers.GetMock<OperableListEx<SubjectClass>>();

            var makeableEnumerable =
                new ListParentMakeableEnumerable<List<int>, SubjectClass, OperableListEx<SubjectClass>>(
                    setOfLists, makeableCollecitonMock.Object, makeableCollecitonMock.Object);

            // Act

            int count = 0;
            ListParentMakeableEnumerable<List<int>, SubjectClass, OperableListEx<SubjectClass>> result =
                makeableEnumerable.Set(m =>
                {
                    count++;
                    return new SubjectClass();
                });

            // Assert

            Assert.AreEqual(2, count);
            Assert.AreEqual(makeableEnumerable, result);
        }

        [TestMethod]
        public void Take_Test()
        {
            // Arrange

            var setOfLists = new List<List<int>>
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
            };

            var makeableCollecitonMock = Helpers.GetMock<OperableListEx<SubjectClass>>();

            var makeableEnumerable =
                new ListParentMakeableEnumerable<List<int>, SubjectClass, OperableListEx<SubjectClass>>(
                    setOfLists, makeableCollecitonMock.Object, makeableCollecitonMock.Object);


            // Act

            ListParentMakeableEnumerable<List<int>, SubjectClass, OperableListEx<SubjectClass>> result = makeableEnumerable.Take(3);

            // Assert

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(setOfLists[0], result[0]);
            Assert.AreEqual(setOfLists[1], result[1]);
            Assert.AreEqual(setOfLists[2], result[2]);
        }

        [TestMethod]
        public void Skip_Test()
        {
            // Arrange

            var setOfLists = new List<List<int>>
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
            };

            var makeableCollecitonMock = Helpers.GetMock<OperableListEx<SubjectClass>>();

            var makeableEnumerable =
                new ListParentMakeableEnumerable<List<int>, SubjectClass, OperableListEx<SubjectClass>>(
                    setOfLists, makeableCollecitonMock.Object, makeableCollecitonMock.Object);

            // Act

            ListParentMakeableEnumerable<List<int>, SubjectClass, OperableListEx<SubjectClass>> result = makeableEnumerable.Skip(3);

            // Assert

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(setOfLists[3], result[0]);
            Assert.AreEqual(setOfLists[4], result[1]);
            Assert.AreEqual(setOfLists[5], result[2]);
        }
    }
}
