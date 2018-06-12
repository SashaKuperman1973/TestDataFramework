using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.ValueGenerator.Concrete;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class MemoryValueGeneratorTests
    {
        [TestMethod]
        public void Generate_Guid_Test()
        {
            var memoryValueGenerator = new MemoryValueGenerator(null, null, null, null, null);

            object result = memoryValueGenerator.GetValue(null, typeof(Guid));
            Assert.IsTrue(result is Guid);
            var guid1 = (Guid) result;
            Assert.AreNotEqual(Guid.Empty, guid1);

            result = memoryValueGenerator.GetValue(null, typeof(Guid));
            Assert.IsTrue(result is Guid);
            var guid2 = (Guid) result;
            Assert.AreNotEqual(Guid.Empty, guid2);

            Assert.AreNotEqual(guid1, guid2);
        }
    }
}