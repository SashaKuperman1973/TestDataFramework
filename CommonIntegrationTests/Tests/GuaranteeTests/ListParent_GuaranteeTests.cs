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
    public class ListParent_GuaranteeTests
    {
        private PopulatorFactory factory;

        private static readonly SubjectClass[] Subjects = new[]{ new SubjectClass(), new SubjectClass(), new SubjectClass(), };

        private Func<SubjectClass>[] FunkySubjects =>
            ListParent_GuaranteeTests.Subjects.Select<SubjectClass, Func<SubjectClass>>(s => () => s).ToArray();

        private object[] MixedSubjects => new object[]
        {
            ListParent_GuaranteeTests.Subjects[0],

            (Func<SubjectClass>) (() => ListParent_GuaranteeTests.Subjects[1]),

            ListParent_GuaranteeTests.Subjects[2]
        };

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.factory = new PopulatorFactory();
        }

        private static void AssertGuaranteedValues(IEnumerable<SubjectClass> set, int targetCount)
        {
            List<SubjectClass> resultList = set.ToList();

            ListParent_GuaranteeTests.Subjects.ToList().ForEach(s => Assert.IsTrue(resultList.Exists(l => l == s)));

            Assert.AreEqual(targetCount, resultList.Count(ListParent_GuaranteeTests.Subjects.Contains));
        }

        // Straight

        [TestMethod]
        public void FixedQuantity_ByValue_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(10)
                .GuaranteeByFixedQuantity(ListParent_GuaranteeTests.Subjects).Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 3);
        }

        [TestMethod]
        public void FixedQuantity_ByValue_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(10)
                .GuaranteeByFixedQuantity(ListParent_GuaranteeTests.Subjects, 5).Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 5);
        }

        [TestMethod]
        public void ByPercentage_ByValue_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(60)
                .GuaranteeByPercentageOfTotal(ListParent_GuaranteeTests.Subjects, 15).Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 9);
        }

        [TestMethod]
        public void ByPercentage_ByValue_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(60)
                .GuaranteeByPercentageOfTotal(ListParent_GuaranteeTests.Subjects).Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 6);
        }

        // Funky

        [TestMethod]
        public void FixedQuantity_ByFunc_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(10)
                .GuaranteeByFixedQuantity(this.FunkySubjects).Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 3);
        }

        [TestMethod]
        public void FixedQuantity_ByFunc_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(10)
                .GuaranteeByFixedQuantity(this.FunkySubjects, 5)
                .Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 5);
        }

        [TestMethod]
        public void ByPercentage_ByFunc_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(60)
                .GuaranteeByPercentageOfTotal(
                    this.FunkySubjects, 15).Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 9);
        }

        [TestMethod]
        public void ByPercentage_ByFunc_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(60)
                .GuaranteeByPercentageOfTotal(this.FunkySubjects).Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 6);
        }

        // Mixed

        [TestMethod]
        public void FixedQuantity_Mixed_DefaultElementCount_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(10)
                .GuaranteeByFixedQuantity(this.MixedSubjects).Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 3);
        }

        [TestMethod]
        public void FixedQuantity_Mixed_ExplictQuantity_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(10)
                .GuaranteeByFixedQuantity(this.MixedSubjects, 5)
                .Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 5);
        }

        [TestMethod]
        public void FixedQuantity_Mixed_ExpicitFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(60)
                .GuaranteeByPercentageOfTotal(
                    this.FunkySubjects, 15).Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 9);
        }

        [TestMethod]
        public void FixedQuantity_Mixed_DefaultFrequency_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IEnumerable<SubjectClass> result = populator.Add<SubjectClass>(60)
                .GuaranteeByPercentageOfTotal(this.MixedSubjects).Make();

            ListParent_GuaranteeTests.AssertGuaranteedValues(result, 6);
        }
    }
}
