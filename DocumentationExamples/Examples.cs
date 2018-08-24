using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
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
        public void Explicitly_Setting_Collection_Element_Properties()
        {
            Subject aSubject = StaticPopulatorFactory.CreateMemoryPopulator()

                .Add<Subject>()

                .SetList(subject => subject.DeepA.DeepBCollection, 20)

                .Skip(3).Take(4)

                .Set(deepB => deepB.DeepC.AnInteger, () => 7)


                .ParentList

                
                .Skip(10).Take(3)

                .Set(deepB => deepB.DeepC.AnInteger, 13)

                .Make();
        }

        [TestMethod]
        public void SetRange_RecordReference_Test()
        {
            ExclusiveRangeTest subject = StaticPopulatorFactory.CreateMemoryPopulator().Add<ExclusiveRangeTest>()

                .SetRange(obj => obj.AnInteger, new[] {2, 4, 6, 8})

                .SetRange(obj => obj.AString, new[] {"Yes", "No", "Maybe"})

                .SetRange(obj => obj.AnotherString, () => new [] {"Yes", "No", "Maybe"})

                .Make();
        }

        [TestMethod]
        public void SetDeepRange_RecordReference_Test()
        {
            DeepB result = StaticPopulatorFactory.CreateMemoryPopulator().Add<DeepB>()
                .SetRange(m => m.DeepC.ATextProperty, new[] {"Hello", "GoodBye"})
                .SetRange(m => m.DeepC.AnInteger, new[] {7, 11})
                .Make();
        }

        [TestMethod]
        public void Reference_Root_Guaranteed_Collection_With_Take_And_Skip()
        {
            Subject subject = StaticPopulatorFactory.CreateMemoryPopulator()
                
                .Add<Subject>()

                .SetList(s => s.DeepA.StringCollection, 15)

                .Skip(2).Take(6).GuaranteeByFixedQuantity(new[] { "Hello", "Goodbye" }, 4)
                
                .ParentList
                
                .Skip(10).Take(4).GuaranteeByFixedQuantity(new [] {"Greetings", "Fairwell"}, 3)
                
                .Make();
        }

        [TestMethod]
        public void List_Root_Guaranteed_Collection_With_Take_And_Skip()
        {
            IEnumerable<string> result = StaticPopulatorFactory.CreateMemoryPopulator()
                
            .Add<string>(20)

            .Skip(2).Take(5).GuaranteeByFixedQuantity(new[] { "Hello", "Goodbye" }, 3)

            .ParentList

            .Skip(12).Take(8).GuaranteeByFixedQuantity(new[] { "Greetings", "Fairwell" }, 4)

            .Make();
        }

        [TestMethod]
        public void List_Root_Guaranteed_Collection_With_Take_Skip_And_Chaining_With_Reference_To_Root()
        {
            IPopulator populator = StaticPopulatorFactory.CreateMemoryPopulator();

            IEnumerable<string> list = populator.Add<string>(15)

                .Skip(2).Take(5).GuaranteeByFixedQuantity(new[] {"Hello", "Goodbye"}, 3).ParentList

                .Skip(8).Take(6).GuaranteeByFixedQuantity(new[] { "Greetings", "Fairwell" }, 4)
                
                .BindAndMake();            

            list.ToList().ForEach(Console.WriteLine);
        }

        [TestMethod]
        public void Parent_RootList_and_Root()
        {
            Subject subject = StaticPopulatorFactory.CreateMemoryPopulator()

                .Add<Subject>()

                .SetList(s => s.DeepA.StringCollection, 30)

                .Skip(2).Take(10).GuaranteeByPercentageOfTotal(new[] {"Hello", "Goodbye"}, 60)

                    .Skip(2).Take(3).GuaranteeByFixedQuantity(new[] {"Greetings", "Fairwell"})

                
                .ParentList

                
                .Skip(4).GuaranteeByFixedQuantity(new [] {"One", "Two"})

                
                .RootList

                
                .Skip(20).Take(5).GuaranteeByFixedQuantity(new [] {"Last", "Show", "Today"})

                
                .Root
                
                
                .Set(m => m.DeepA.TextA, "Sample Text")

                .Make();
        }

        [TestMethod]
        public void Set_Subproperties_Entire_List_Test()
        {
            IEnumerable<Subject> subjectCollection = StaticPopulatorFactory.CreateMemoryPopulator()

                .Add<Subject>(5)
                
                .Set(m => m.DeepA.TextA, "Foo")

                .Make();
        }

        [TestMethod]
        public void Set_Subproperty_Test()
        {
            Subject subject = StaticPopulatorFactory.CreateMemoryPopulator()

                .Add<Subject>()

                .SetList(m => m.DeepA.DeepBCollection, 20)
                
                .Take(10).Set(m => m.DeepC.ATextProperty)

                    .GuaranteePropertiesByFixedQuantity(new[] { "Foo", "Bar" })

                    .GuaranteePropertiesByPercentageOfTotal(new [] {"Stop", "Go"}, 40)

                .OperableList
                .ParentList
                
                .Skip(10).Take(3).Set(m => m.DeepC.AnInteger, 7)

                .Make();
        }

        [TestMethod]
        public void Lambda_Test()
        {
            IPopulator populator = StaticPopulatorFactory.CreateMemoryPopulator();

            RecordReference<Subject> subject = populator.Add<Subject>().Set(m => m.Text, () => "Some text");

            DeepC deepC = populator.Add<DeepC>().BindAndMake();
        }

        [TestMethod]
        public void Test()
        {
            IEnumerable<DeepA> result = StaticPopulatorFactory.CreateMemoryPopulator().Add<DeepA>(3)
                .SelectListSet(deepA => deepA.DeepB.DeepCCollection, 7)
                .Set(deepCList => deepCList.Skip(2).Take(2).Set(deepC => deepC.ATextProperty, "I"))
                .Set(deepCList => deepCList.Skip(4).Take(2).Set(deepC => deepC.ATextProperty, "II"))
                .Make();
        }
    }
}
