using ExplicitlySettingProperties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Concrete.OperableList;

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
                .Add<Subject>(5).Set(m => m.DeepA.TextA).Take(3).SetRange(m => m.DeepA.TextA, new[] {"A", "B", "C"})
                .Make();
        }

        [TestMethod]
        public void Explicitly_Setting_Collection_Element_Properties()
        {
            Subject aSubject = StaticPopulatorFactory.CreateMemoryPopulator()

                .Add<Subject>()

                .SetList(subject => subject.DeepA.DeepBCollection, 10)

                .Set(deepB => deepB.DeepC.AnInteger, 7)

                .Make();
        }

        [TestMethod]
        public void Reference_Root_Guaranteed_Collection_With_Take_And_Skip()
        {
            Subject subject = StaticPopulatorFactory.CreateMemoryPopulator()
                .Add<Subject>()
                .SetList(s => s.DeepA.StringCollection, 10).Skip(2).Take(5)
                .GuaranteeByFixedQuantity(new[] { "Hello", "Goodbye" }, 3)
                .Make();
        }

        [TestMethod]
        public void List_Root_Guaranteed_Collection_With_Take_And_Skip()
        {
            ListParentOperableList<string> list = StaticPopulatorFactory.CreateMemoryPopulator()
                .Add<string>(20);

            list.Skip(2).Take(5).GuaranteeByFixedQuantity(new[] { "Hello", "Goodbye" }, 3);
            IEnumerable<string> result = list.Skip(12).Take(8).GuaranteeByFixedQuantity(new[] {"Greetings", "Fairwell"}, 4)
                .Make();
        }
    }
}
