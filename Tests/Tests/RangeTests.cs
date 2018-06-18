using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Populator.Concrete;

namespace Tests.Tests
{
    [TestClass]
    public class RangeTests
    {
        [TestMethod]
        public void Range_WithConstructor_Test()
        {
            var range = new Range(3, 5);

            Assert.AreEqual(3, range.StartPosition);
            Assert.AreEqual(5, range.EndPosition);
            Assert.AreEqual(3, range.Length);
        }

        [TestMethod]
        public void StartPositionOnly_GetLengthOrEndPosition_ThrowsIncompleteRange_Test()
        {
            var range = new Range();

            range.StartPosition = 3;

            Assert.AreEqual(3, range.StartPosition);

            Helpers.ExceptionTest(() =>
            {
                int x = range.EndPosition;
            }, typeof(InvalidOperationException));

            Helpers.ExceptionTest(() =>
            {
                int x = range.Length;
            }, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void EndPositionOnly_GetStartOrLength_ThrowsIncompleteRange_Test()
        {
            var range = new Range();

            range.EndPosition = 5;

            Helpers.ExceptionTest(() =>
            {
                int x = range.StartPosition;
            }, typeof(InvalidOperationException));

            Helpers.ExceptionTest(() =>
            {
                int x = range.Length;
            }, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void LengthOnly_GetStartOrLength_ThrowsIncompleteRange_Test()
        {
            var range = new Range();

            range.Length = 3;

            Helpers.ExceptionTest(() =>
            {
                int x = range.StartPosition;
            }, typeof(InvalidOperationException));

            Helpers.ExceptionTest(() =>
            {
                int x = range.EndPosition;
            }, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void SetStartAndEnd_TestStartEndLength()
        {
            var range = new Range();

            range.StartPosition = 3;
            range.EndPosition = 6;

            Assert.AreEqual(4, range.Length);
            Assert.AreEqual(3, range.StartPosition);
            Assert.AreEqual(6, range.EndPosition);
        }

        [TestMethod]
        public void SetEndAndStart_TestStartEndLength()
        {
            var range = new Range();

            range.EndPosition = 6;
            range.StartPosition = 3;

            Assert.AreEqual(4, range.Length);
            Assert.AreEqual(3, range.StartPosition);
            Assert.AreEqual(6, range.EndPosition);
        }

        [TestMethod]
        public void SetStartAndLength_TestStartEndLength()
        {
            var range = new Range();

            range.StartPosition = 3;
            range.Length = 4;

            Assert.AreEqual(4, range.Length);
            Assert.AreEqual(3, range.StartPosition);
            Assert.AreEqual(6, range.EndPosition);
        }

        [TestMethod]
        public void SetLengthAndStart_TestStartEndLength()
        {
            var range = new Range();

            range.Length = 4;
            range.StartPosition = 3;

            Assert.AreEqual(4, range.Length);
            Assert.AreEqual(3, range.StartPosition);
            Assert.AreEqual(6, range.EndPosition);
        }

        [TestMethod]
        public void SetEndAndLength_TestStartEndLength()
        {
            var range = new Range();

            range.EndPosition = 10;
            range.Length = 3;

            Assert.AreEqual(3, range.Length);
            Assert.AreEqual(8, range.StartPosition);
            Assert.AreEqual(10, range.EndPosition);
        }

        [TestMethod]
        public void SetLengthAndEnd_TestStartEndLength()
        {
            var range = new Range();

            range.Length = 3;
            range.EndPosition = 10;

            Assert.AreEqual(3, range.Length);
            Assert.AreEqual(8, range.StartPosition);
            Assert.AreEqual(10, range.EndPosition);
        }

        [TestMethod]
        public void ImplicitConversionFromInt_Test()
        {
            Range range = 5;

            Assert.AreEqual(5, range.StartPosition);
            Assert.AreEqual(5, range.EndPosition);
            Assert.AreEqual(1, range.Length);
        }
    }
}