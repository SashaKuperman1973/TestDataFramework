using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator;

namespace Tests.Tests
{
    [TestClass]
    public class PopulatorFactoryTests
    {
        [TestMethod]
        public void GetSqlServerPopulator_Test()
        {
            using (var factory = new PopulatorFactory())
            {
                IPopulator populator = factory.CreateSqlClientPopulator(string.Empty);
                Assert.AreEqual(typeof (StandardPopulator), populator.GetType());
            }
        }

        [TestMethod]
        public void GetSqlServerPopulator_WithTransactionOption()
        {
            using (var factory = new PopulatorFactory())
            {
                IPopulator populator = factory.CreateSqlClientPopulator(string.Empty, false);
                Assert.AreEqual(typeof(StandardPopulator), populator.GetType());
            }
        }

        [TestMethod]
        public void GetMemoryPopulator_Test()
        {
            using (var factory = new PopulatorFactory())
            {
                throw new NotImplementedException();
            }
        }
    }
}
