using ExplicitlySettingProperties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestDataFramework.Factories;

namespace DocumentationExamples
{
    [TestClass]
    public class Examples
    {
        [TestMethod]
        public void MakeAnObject()
        {
            Subject aSubject = StaticPopulatorFactory.CreateMemoryPopulator().Make<Subject>();
        }

        [TestMethod]
        public void MakeACollection()
        {
            IEnumerable<Subject> subjectColection = StaticPopulatorFactory.CreateMemoryPopulator().Make<Subject>(5);
        }

        [TestMethod]
        public void ExplicitlyPresettingProperties()
        {
            Subject aSubject = StaticPopulatorFactory.CreateMemoryPopulator()
                .Add<Subject>().Set(m => m.DeepA.TextA, "Hello").Make();
        }

        [TestMethod]
        public void SetRange_OnCollection()
        {
            IEnumerable<Subject> subjectCollection = StaticPopulatorFactory.CreateMemoryPopulator()
                .Add<Subject>(5).SetRange(m => m.DeepA.TextA, new[] { "A", "B", "C" }).Make();
        }

        [TestMethod]
        public void Explicitly_Setting_Collection_Element_Properties()
        {
            Subject aSubject = StaticPopulatorFactory.CreateMemoryPopulator()

                .Add<Subject>()

                .SetList(m => m.DeepA.DeepBCollection, 10)

                .Set(m => m.DeepC.AnInteger, 7)

                .Make();
        }
    }
}
