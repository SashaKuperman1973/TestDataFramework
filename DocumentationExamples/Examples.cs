using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using ExampleTypes;
using TestDataFramework.Factories;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using TestDataFramework.Populator.Interfaces;

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
                .Add<Subject>(5).Set(m => m.DeepA.TextA).SetRange(m => m.DeepA.TextA, new[] {"A", "B", "C"})
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
            OperableListEx<string> list =
                StaticPopulatorFactory.CreateMemoryPopulator().Add<string>(20);

            list.Skip(2).Take(5).GuaranteeByFixedQuantity(new[] { "Hello", "Goodbye" }, 3);
            IEnumerable<string> result = list.Skip(12).Take(8).GuaranteeByFixedQuantity(new[] { "Greetings", "Fairwell" }, 4)
                .Make();
        }

        [TestMethod]
        public void List_Root_Guaranteed_Collection_With_Take_Skip_And_Chaining_With_Reference_To_Root()
        {
            IPopulator populator = StaticPopulatorFactory.CreateMemoryPopulator();

            IEnumerable<string> list = populator.Add<string>(10)

                .Skip(2).Take(5).GuaranteeByFixedQuantity(new[] {"Hello", "Goodbye"}, 3).ParentList

                .Skip(12).Take(8).GuaranteeByFixedQuantity(new[] { "Greetings", "Fairwell" }, 4)
                
                .BindAndMake();            

            list.ToList().ForEach(Console.WriteLine);
        }

        [TestMethod]
        public void ListOffAList()
        {
            Subject subject = StaticPopulatorFactory.CreateMemoryPopulator()
                
                .Add<Subject>()

                .SetList(s => s.DeepA.DeepBCollection, 20)
                .GuaranteeByFixedQuantity(new[] {new DeepB {TextC = "I"}, new DeepB {TextC = "II"}}, 3)

                    .SetList(deepB => deepB.DeepCCollection, 10)
                    .GuaranteeByFixedQuantity(new[] {new DeepC {ATextProperty = "X"}, new DeepC {ATextProperty = "XX"},}, 5)

                        .SetList(deepC => deepC.DeepCStringCollection, 10)
                        .GuaranteeByFixedQuantity(new [] {"V", "VV"})

                .RootList
                .Set(deepA => deepA)
                .Make();
        }
    }
}
