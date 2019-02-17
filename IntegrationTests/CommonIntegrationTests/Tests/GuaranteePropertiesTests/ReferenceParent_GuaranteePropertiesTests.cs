/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using IntegrationTests.CommonIntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Interfaces;

namespace IntegrationTests.CommonIntegrationTests.Tests.GuaranteePropertiesTests
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
                result.DeepB.DeepCList.Count(l => new[] { "A", "B", "C" }.Contains(l.DeepString)));
        }

        // Straight

        [TestMethod]
        public void FixedQuantity_ByValue_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new[] {"A", "B", "C"}).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 3);
        }

        [TestMethod]
        public void FixedQuantity_ByValue_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new[] { "A", "B", "C" }, 7).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 7);
        }

        [TestMethod]
        public void ByPercentage_ByValue_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new[] { "A", "B", "C" }, 15).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 9);
        }

        [TestMethod]
        public void ByPercentage_ByValue_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new[] { "A", "B", "C" }).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 6);
        }

        // Funky

        [TestMethod]
        public void FixedQuantity_ByFunc_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new Func<string>[] {() => "A", () => "B", () => "C"}).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 3);
        }

        [TestMethod]
        public void FixedQuantity_ByFunc_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new Func<string>[] { () => "A", () => "B", () => "C" }, 7).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 7);
        }

        [TestMethod]
        public void ByPercentage_ByFunc_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new Func<string>[] { () => "A", () => "B", () => "C" }, 15).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 9);
        }

        [TestMethod]
        public void ByPercentage_ByFunc_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new Func<string>[] { () => "A", () => "B", () => "C" }).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 6);
        }

        // Mixed

        [TestMethod]
        public void FixedQuantity_Mixed_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new object[] { "A", (Func<string>)(() => "B"), "C" }).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 3);
        }

        [TestMethod]
        public void FixedQuantity_Mixed_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 10).Set(m => m.DeepString)
                .GuaranteePropertiesByFixedQuantity(new object[] { "A", (Func<string>)(() => "B"), "C" }, 7).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 7);
        }

        [TestMethod]
        public void ByPercentage_Mixed_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new object[] { "A", (Func<string>)(() => "B"), "C" }, 15).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 9);
        }

        [TestMethod]
        public void ByPercentage_Mixed_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(n => n.DeepB.DeepCList, 60).Set(m => m.DeepString)
                .GuaranteePropertiesByPercentageOfTotal(new object[] { "A", (Func<string>)(() => "B"), "C" }).Make();

            ReferenceParent_GuaranteePropertiesTests.AssertGuaranteedValues(result, 6);
        }
    }
}
