using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Helpers;

namespace Tests.Tests
{
    [TestClass]
    public class LargeIntegerTests
    {
        [TestMethod]
        public void LargeIntegerTest()
        {
            var largeInteger = new LargeInteger();

            for (ulong i = 0; i < ulong.MaxValue; i++)
            {
                //Assert.AreEqual(i, (ulong)largeInteger++);
            }
        }

    }
}
