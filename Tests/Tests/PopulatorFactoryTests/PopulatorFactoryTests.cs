using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Interfaces;

namespace Tests.Tests.PopulatorFactoryTests
{
    [TestClass]
    public class PopulatorFactoryTests
    {
        [TestMethod]
        public void CreateMemoryPopulator_Test()
        {
            var factory = new PopulatorFactory();
            IPopulator reference1 = factory.CreateMemoryPopulator();

            IPopulator reference2 = factory.CreateMemoryPopulator();

            Assert.IsNotNull(reference1);
            Assert.IsNotNull(reference2);

            Assert.AreEqual(reference1, reference2);

            factory.Dispose();
        }

        [TestMethod]
        public void CreateMemoryPopulator_Dispose_Test()
        {
            var factory = new PopulatorFactory();
            IPopulator reference1 = factory.CreateMemoryPopulator();
            factory.Dispose();

            IPopulator reference2 = factory.CreateMemoryPopulator();

            Assert.IsNotNull(reference1);
            Assert.IsNotNull(reference2);

            Assert.AreNotEqual(reference1, reference2);

            factory.Dispose();
        }

        [TestMethod]
        public void CreateSqlClientPopulator_Test()
        {
            var factory = new PopulatorFactory();
            IPopulator reference1 = factory.CreateSqlClientPopulator("connection string");

            IPopulator reference2 = factory.CreateSqlClientPopulator("connection string");

            Assert.IsNotNull(reference1);
            Assert.IsNotNull(reference2);

            Assert.AreEqual(reference1, reference2);

            factory.Dispose();
        }

        [TestMethod]
        public void CreateSqlClientPopulator_Dispose_Test()
        {
            var factory = new PopulatorFactory();
            IPopulator reference1 = factory.CreateSqlClientPopulator("connection string");
            factory.Dispose();

            IPopulator reference2 = factory.CreateSqlClientPopulator("connection string");

            Assert.IsNotNull(reference1);
            Assert.IsNotNull(reference2);

            Assert.AreNotEqual(reference1, reference2);

            factory.Dispose();
        }
    }
}
