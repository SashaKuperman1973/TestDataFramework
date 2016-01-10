using System;
using System.Runtime.InteropServices;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator;

namespace Tests.Tests
{
    [TestClass]
    public class PopulatorFactoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();
        }

        [TestMethod]
        public void GetSqlClientPopulator_Test()
        {
            using (var factory = new PopulatorFactory())
            {
                IPopulator populator = factory.CreateSqlClientPopulator(string.Empty);
                Assert.AreEqual(typeof (StandardPopulator), populator.GetType());
            }
        }

        [TestMethod]
        public void GetSqlClientPopulator_WithTransactionOption()
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
                IPopulator populator = factory.CreateMemoryPopulator();
                Assert.AreEqual(typeof(StandardPopulator), populator.GetType());
            }
        }
    }
}
