using System;
using System.Collections.Generic;
using System.Linq;
using CommonIntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Interfaces;

namespace CommonIntegrationTests.Tests.GuaranteePropertiesTests
{
    [TestClass]
    public class ListParent_GuaranteePropertiesTests
    {
        private PopulatorFactory factory;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.factory = new PopulatorFactory();
        }

        private static void AssertGuaranteedValues(IEnumerable<DeepC> result, int fixedQuantityToCheckFor)
        {
            List<DeepC> resultList = result.ToList();

            Assert.IsTrue(resultList.Exists(l => l.DeepString == "A"));
            Assert.IsTrue(resultList.Exists(l => l.DeepString == "B"));
            Assert.IsTrue(resultList.Exists(l => l.DeepString == "C"));

            Assert.AreEqual(fixedQuantityToCheckFor,
                resultList.Count(l => new[] {"A", "B", "C"}.Contains(l.DeepString)));
        }

        // Straight

        [TestMethod]
        public void FixedQuantity_ByValue_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new[] {"A", "B", "C"}).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 3);
        }

        [TestMethod]
        public void FixedQuantity_ByValue_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new[] { "A", "B", "C" }, 7).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 7);
        }

        [TestMethod]
        public void ByPercentage_ByValue_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new[] { "A", "B", "C" }, 15).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 9);
        }

        [TestMethod]
        public void ByPercentage_ByValue_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new[] { "A", "B", "C" }).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 6);
        }

        // Funky

        [TestMethod]
        public void FixedQuantity_ByFunc_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new Func<string>[] {() => "A", () => "B", () => "C"}).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 3);
        }

        [TestMethod]
        public void FixedQuantity_ByFunc_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new Func<string>[] { () => "A", () => "B", () => "C" }, 7).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 7);
        }

        [TestMethod]
        public void ByPercentage_ByFunc_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new Func<string>[] { () => "A", () => "B", () => "C" }, 15).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 9);
        }

        [TestMethod]
        public void ByPercentage_ByFunc_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new Func<string>[] { () => "A", () => "B", () => "C" }).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 6);
        }

        // Mixed

        [TestMethod]
        public void FixedQuantity_Mixed_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new object[] { "A", (Func<string>)(() => "B"), "C" }).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 3);
        }

        [TestMethod]
        public void FixedQuantity_Mixed_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new object[] { "A", (Func<string>)(() => "B"), "C" }, 7).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 7);
        }

        [TestMethod]
        public void ByPercentage_Mixed_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new object[] { "A", (Func<string>)(() => "B"), "C" }, 15).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 9);
        }

        [TestMethod]
        public void ByPercentage_Mixed_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<DeepC> result = populator.Add<DeepC>(60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new object[] { "A", (Func<string>)(() => "B"), "C" }).Make();

            ListParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 6);
        }
    }
}
