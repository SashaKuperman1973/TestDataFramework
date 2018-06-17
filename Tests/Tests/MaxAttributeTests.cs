using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;

namespace Tests.Tests
{
    [TestClass]
    public class MaxAttributeTests
    {
        [TestMethod]
        public void MaxAttribute_Long_Type_Test()
        {
            long x = 5;
            var maxAttribute = new MaxAttribute(x);

            Assert.AreEqual(x, maxAttribute.Max);
        }

        [TestMethod]
        public void MaxAttribute_DateTime_Test()
        {
            DateTime dateTime = DateTime.Now;
            var maxAttribute = new MaxAttribute(dateTime);

            Assert.AreEqual(dateTime.Ticks, maxAttribute.Max);
        }
    }
}
