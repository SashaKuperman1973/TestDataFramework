using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class AttributeReadWriteTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Entity<AttributeReadWriteTestClass>.Decorate(c => c.Key1, new PrimaryKeyAttribute());
        }
    }
}
