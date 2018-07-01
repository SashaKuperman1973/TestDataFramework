using CommonIntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Interfaces;

namespace CommonIntegrationTests.Tests
{
    [TestClass]
    public class GuaranteePropertiesTests
    {
        private PopulatorFactory factory;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.factory = new PopulatorFactory();
        }

        [TestMethod]
        public void Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new[] {"A", "B", "C"}).Make();
        }
    }
}
