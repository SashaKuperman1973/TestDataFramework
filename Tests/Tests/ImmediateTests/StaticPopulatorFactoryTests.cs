using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Interfaces;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class StaticPopulatorFactoryTests
    {
        [TestMethod]
        public void CreateSqlClientPopulator_Test()
        {
            IPopulator populator = StaticPopulatorFactory.CreateSqlClientPopulator("dummyConnectionString");

            Assert.IsNotNull(populator);
        }

        [TestMethod]
        public void CreateMemoryPopulator_Test()
        {
            IPopulator populator = StaticPopulatorFactory.CreateMemoryPopulator();

            Assert.IsNotNull(populator);
        }
    }
}
