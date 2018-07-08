using System.Linq;
using CommonIntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Interfaces;

namespace CommonIntegrationTests.Tests.GuaranteePropertiesTests
{
    [TestClass]
    public class ReferenceParent_GuaranteePropertiesTests
    {
        private PopulatorFactory factory;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.factory = new PopulatorFactory();
        }

        private static void AssertGuaranteedValues(DeepA result, int fixedQuantityToCheckFor)
        {
            Assert.IsTrue(result.DeepB.DeepCList.Exists(l => l.DeepString == "A"));
            Assert.IsTrue(result.DeepB.DeepCList.Exists(l => l.DeepString == "B"));
            Assert.IsTrue(result.DeepB.DeepCList.Exists(l => l.DeepString == "C"));

            Assert.AreEqual(fixedQuantityToCheckFor,
                result.DeepB.DeepCList.Count(l => new[] {"A", "B", "C"}.Contains(l.DeepString)));
        }

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new[] {"A", "B", "C"}).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 3);
        }

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new[] { "A", "B", "C" }, 7).Make();

            Assert.IsTrue(result.DeepB.DeepCList.Exists(l => l.DeepString == "A"));
            Assert.IsTrue(result.DeepB.DeepCList.Exists(l => l.DeepString == "B"));
            Assert.IsTrue(result.DeepB.DeepCList.Exists(l => l.DeepString == "C"));

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 7);
        }

        [TestMethod]
        public void GuaranteePropertiesByPercentage_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new[] { "A", "B", "C" }, 15).Make();

            Assert.IsTrue(result.DeepB.DeepCList.Exists(l => l.DeepString == "A"));
            Assert.IsTrue(result.DeepB.DeepCList.Exists(l => l.DeepString == "B"));
            Assert.IsTrue(result.DeepB.DeepCList.Exists(l => l.DeepString == "C"));

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 9);
        }
    }
}
