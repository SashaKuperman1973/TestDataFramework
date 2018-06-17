using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;

namespace Tests.Tests
{
    [TestClass]
    public class MinAttributeTests
    {
        [TestMethod]
        public void MinAttribute_Long_Type_Test()
        {
            long x = 45;
            var minAttribute = new MinAttribute(x);

            Assert.AreEqual(x, minAttribute.Min);
        }

        [TestMethod]
        public void MinAttribute_DateTime_Test()
        {
            DateTime dateTime = DateTime.Now;
            var minAttribute = new MinAttribute(dateTime);

            Assert.AreEqual(dateTime.Ticks, minAttribute.Min);
        }
    }
}
