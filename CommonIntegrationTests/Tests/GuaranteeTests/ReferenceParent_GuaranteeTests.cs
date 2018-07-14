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

using CommonIntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Interfaces;

namespace CommonIntegrationTests.Tests.GuaranteeTests
{
    [TestClass]
    public class ReferenceParent_GuaranteeTests
    {
        private PopulatorFactory factory;

        private static readonly DeepC[] Subjects = { new DeepC {DeepString = "ds1"}, new DeepC { DeepString = "ds2" }, new DeepC { DeepString = "ds3" }, };

        private Func<DeepC>[] FunkySubjects =>
            ReferenceParent_GuaranteeTests.Subjects.Select<DeepC, Func<DeepC>>(s => () => s).ToArray();

        private object[] MixedSubjects => new object[]
        {
            ReferenceParent_GuaranteeTests.Subjects[0],

            (Func<DeepC>) (() => ReferenceParent_GuaranteeTests.Subjects[1]),

            ReferenceParent_GuaranteeTests.Subjects[2]
        };

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.factory = new PopulatorFactory();
        }

        private static void AssertGuaranteedValues(IEnumerable<DeepC> set, int targetCount)
        {
            List<DeepC> resultList = set.ToList();

            ReferenceParent_GuaranteeTests.Subjects.ToList().ForEach(s => Assert.IsTrue(resultList.Exists(l => l == s)));

            Assert.AreEqual(targetCount, resultList.Count(ReferenceParent_GuaranteeTests.Subjects.Contains));
        }

        // Straight

        [TestMethod]
        public void FixedQuantity_ByValue_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 10)
                .GuaranteeByFixedQuantity(ReferenceParent_GuaranteeTests.Subjects).Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 3);
        }

        [TestMethod]
        public void FixedQuantity_ByValue_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 10)
                .GuaranteeByFixedQuantity(ReferenceParent_GuaranteeTests.Subjects, 5).Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 5);
        }

        [TestMethod]
        public void ByPercentage_ByValue_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 60)
                .GuaranteeByPercentageOfTotal(ReferenceParent_GuaranteeTests.Subjects, 15).Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 9);
        }

        [TestMethod]
        public void ByPercentage_ByValue_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 60)
                .GuaranteeByPercentageOfTotal(ReferenceParent_GuaranteeTests.Subjects).Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 6);
        }

        // Funky

        [TestMethod]
        public void FixedQuantity_ByFunc_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 10)
                .GuaranteeByFixedQuantity(this.FunkySubjects).Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 3);
        }

        [TestMethod]
        public void FixedQuantity_ByFunc_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 10)
                .GuaranteeByFixedQuantity(this.FunkySubjects, 5)
                .Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 5);
        }

        [TestMethod]
        public void ByPercentage_ByFunc_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 60)
                .GuaranteeByPercentageOfTotal(this.FunkySubjects, 15).Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 9);
        }

        [TestMethod]
        public void ByPercentage_ByFunc_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 60)
                .GuaranteeByPercentageOfTotal(this.FunkySubjects).Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 6);
        }

        // Mixed

        [TestMethod]
        public void FixedQuantity_Mixed_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 10)
                .GuaranteeByFixedQuantity(this.MixedSubjects).Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 3);
        }

        [TestMethod]
        public void FixedQuantity_Mixed_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 10)
                .GuaranteeByFixedQuantity(this.MixedSubjects, 5)
                .Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 5);
        }

        [TestMethod]
        public void ByPercentage_Mixed_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 60)
                .GuaranteeByPercentageOfTotal(this.MixedSubjects, 15).Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 9);
        }

        [TestMethod]
        public void ByPercentage_Mixed_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA result = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 60)
                .GuaranteeByPercentageOfTotal(this.MixedSubjects).Make();

            ReferenceParent_GuaranteeTests.AssertGuaranteedValues(result.DeepB.DeepCList, 6);
        }
    }
}
