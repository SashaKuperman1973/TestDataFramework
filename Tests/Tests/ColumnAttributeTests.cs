using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;

namespace Tests.Tests
{
    [TestClass]
    public class ColumnAttributeTests
    {
        [TestMethod]
        public void ColumnAttribute_Test()
        {
            string name = "name";
            var columnAttribute = new ColumnAttribute(name);

            Assert.AreEqual(name, columnAttribute.Name);
        }
    }
}