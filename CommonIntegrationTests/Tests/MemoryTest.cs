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
using System.Collections.Generic;
using System.Linq;
using CommonIntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.Factories;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using TestDataFramework.Populator.Interfaces;
using Tests.TestModels;
using SubjectClass = CommonIntegrationTests.TestModels.SubjectClass;

namespace CommonIntegrationTests.Tests
{
    [TestClass]
    public class MemoryTest
    {
        private PopulatorFactory factory;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.factory = new PopulatorFactory();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.factory.Dispose();
        }

        [TestMethod]
        public void StructOnConstructor_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            RecordReference<AClassWithAStructOnConstructor> reference = populator.Add<AClassWithAStructOnConstructor>();
            populator.Bind();

            Assert.IsNotNull(reference.RecordObject.AStruct.AValue);
        }

        [TestMethod]
        public void EnumOnConstructor_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            RecordReference<AClassWithAnEnumOnConstructor> reference = populator.Add<AClassWithAnEnumOnConstructor>();
            populator.Bind();

            Console.WriteLine(reference.RecordObject.AnEnum);
        }

        [TestMethod]
        public void InstrinsicValueAsRoot_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            string aString = populator.Add<string>().Make();

            Console.WriteLine(aString);
        }

        [TestMethod]
        public void GetObject_WithExplicitConstructor_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();
            RecordReference<TwoParameterConstructor> reference = populator.Add<TwoParameterConstructor>();
            populator.Bind();

            TwoParameterConstructor result = reference.RecordObject;

            Assert.IsNotNull(result.Subject);
            Assert.IsNotNull(result.SubjectReference);
            Assert.AreEqual(result.SubjectReference, result.Subject);

            Assert.IsNotNull(result.OneParameterConstructor);

            Assert.IsNotNull(result.OneParameterConstructor.DefaultConstructor);
            Assert.IsNotNull(result.OneParameterConstructor.DefaultConstructorReference);
            Assert.AreNotEqual(result.OneParameterConstructor.DefaultConstructorReference,
                result.OneParameterConstructor.DefaultConstructor);
        }

        [TestMethod]
        public void Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IList<RecordReference<SubjectClass>> subjectReference = populator.Add<SubjectClass>(2);
            RecordReference<ForeignSubjectClass> foreignReference =
                populator.Add<ForeignSubjectClass>(subjectReference[1]);
            populator.Bind();

            Helpers.Dump(subjectReference[0].RecordObject);
            Helpers.Dump(subjectReference[1].RecordObject);
            Helpers.Dump(foreignReference.RecordObject);
        }

        [TestMethod]
        public void PropertyWith_No_Setter_Is_Ignored()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            SubjectClass subjectClass = populator.Add<SubjectClass>().Make();
            Assert.IsNull(subjectClass.GetterOnly);
        }

        // This is a test of value types in general.
        [TestMethod]
        public void Multiple_KeyValuePair_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IList<RecordReference<KeyValuePair<SubjectClass, ForeignSubjectClass>>> kvpRefs =
                populator.Add<KeyValuePair<SubjectClass, ForeignSubjectClass>>(10);

            populator.Bind();

            Helpers.Dump(kvpRefs.First().RecordObject.Key);
            Helpers.Dump(kvpRefs.First().RecordObject.Value);
            kvpRefs.ToList().ForEach(r => Helpers.Dump(r.RecordObject));
        }

        [TestMethod]
        public void Dictionary_UniqueValueTypeKeys_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            RecordReference<ClassWithHandledTypes> recordReference = populator.Add<ClassWithHandledTypes>();
            populator.Bind();

            IDictionary<KeyValuePair<int, string>, object> dictionary = recordReference.RecordObject.ADictionary;

            foreach (KeyValuePair<KeyValuePair<int, string>, object> item in dictionary)
                Console.WriteLine(item.Key);
        }

        [TestMethod]
        public void GauranteedValueTest()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IList<RecordReference<SqlSubjectClass>> subjectReference = populator.Add<SqlSubjectClass>(2);
            OperableList<ForeignSubjectClass> foreignReference =
                populator.Add<ForeignSubjectClass>(20, subjectReference[1])
                    .GuaranteeByPercentageOfTotal(new object[]
                    {
                        new ForeignSubjectClass {SecondInteger = 777},
                        //(Func<ForeignSubjectClass>)
                        //(() => new ForeignSubjectClass {SecondInteger = subjectReference[1].RecordObject.IntegerWithMax}),
                        new ForeignSubjectClass {SecondInteger = 999}
                    }, 50)
                    .GuaranteeByFixedQuantity(new object[]
                    {
                        new ForeignSubjectClass {SecondInteger = 111},
                        (Func<ForeignSubjectClass>)
                        (() => new ForeignSubjectClass
                        {
                            SecondInteger = subjectReference[0].RecordObject.IntegerWithMax
                        }),
                        new ForeignSubjectClass {SecondInteger = 222}
                    }, 5);

            populator.Bind();

            Console.WriteLine("SubjectClass[1].IntegerWithMax: " + subjectReference[1].RecordObject.IntegerWithMax);
            Console.WriteLine("SubjectClass[0].IntegerWithMax: " + subjectReference[0].RecordObject.IntegerWithMax);
            int i = 1;
            foreignReference.ToList().ForEach(r => Console.WriteLine(i++ + ".\r\n" + r.RecordObject.ToString()));
        }

        [TestMethod]
        public void GauranteedValue_FixedQuantity_Too_Large()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            OperableList<ForeignSubjectClass> foreignReference =
                populator.Add<ForeignSubjectClass>(20)
                    .GuaranteeByFixedQuantity(new object[]
                    {
                        new ForeignSubjectClass {SecondInteger = 111},
                        (Func<ForeignSubjectClass>)
                        (() => new ForeignSubjectClass {SecondInteger = 222}),
                        new ForeignSubjectClass {SecondInteger = 333}
                    }, 21);

            global::Tests.Helpers.ExceptionTest(() => populator.Bind(), typeof(ValueGuaranteeException),
                Messages.TooFewReferencesForValueGuarantee);
        }

        [TestMethod]
        public void GauranteedValue_Percentage_Too_Large()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            OperableList<ForeignSubjectClass> foreignReference =
                populator.Add<ForeignSubjectClass>(20)
                    .GuaranteeByPercentageOfTotal(new object[]
                    {
                        new ForeignSubjectClass {SecondInteger = 111},
                        (Func<ForeignSubjectClass>)
                        (() => new ForeignSubjectClass {SecondInteger = 222}),
                        new ForeignSubjectClass {SecondInteger = 333}
                    }, 105);

            global::Tests.Helpers.ExceptionTest(() => populator.Bind(), typeof(ValueGuaranteeException),
                Messages.TooFewReferencesForValueGuarantee);
        }

        [TestMethod]
        public void GauranteedValue_FixedQuantity_Plus_Percentage_Too_Large()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            populator.Add<ForeignSubjectClass>(20)
                .GuaranteeByPercentageOfTotal(new object[]
                {
                    new ForeignSubjectClass {SecondInteger = 777},
                    (Func<ForeignSubjectClass>)
                    (() => new ForeignSubjectClass {SecondInteger = 888}),
                    new ForeignSubjectClass {SecondInteger = 999}
                }, 80)
                .GuaranteeByFixedQuantity(new object[]
                {
                    new ForeignSubjectClass {SecondInteger = 111},
                    (Func<ForeignSubjectClass>)
                    (() => new ForeignSubjectClass {SecondInteger = 222}),
                    new ForeignSubjectClass {SecondInteger = 333}
                }, 5);

            global::Tests.Helpers.ExceptionTest(() => populator.Bind(), typeof(ValueGuaranteeException),
                Messages.TooFewReferencesForValueGuarantee);
        }

        [TestMethod]
        public void Dictionary_Test()
        {
            IPopulator populator = StaticPopulatorFactory.CreateMemoryPopulator();
            populator.Add<ClassWithDictionary>();
            populator.Bind();
        }

        private class ClassWithDictionary
        {
            public Dictionary<string, string> ADictionary { get; set; }
        }

        [TestMethod]
        public void GuaranteedValueAndExplicitSetting_Test()
        {
            IPopulator populator = StaticPopulatorFactory.CreateMemoryPopulator();

            OperableList<SubjectClass> subjectReferences = populator.Add<SubjectClass>(4)
                .GuaranteeByPercentageOfTotal(
                    new[] {new SubjectClass {AnEmailAddress = "myemailAddress@here.com", Text = "Guaranteed Text"}},
                    75);

            subjectReferences[1].Set(p => p.Text, "Hello");

            populator.Bind();

            subjectReferences.ToList()
                .ForEach(
                    reference =>
                        Console.WriteLine(reference.RecordObject.AnEmailAddress + "\r\n" + reference.RecordObject.Text +
                                          "\r\n"));
        }

        [TestMethod]
        public void StandardPopulator_RecordReference_Make_Test()
        {
            // Arrange

            IPopulator populator = StaticPopulatorFactory.CreateMemoryPopulator();

            // Act

            RecordReference<SubjectClass> reference = populator.Add<SubjectClass>();
            RecordReference<SubjectClass> subjectReference = populator.Add<SubjectClass>();
            SubjectClass subject = subjectReference.Make();

            // Assert

            Assert.IsNotNull(subject);
            Assert.IsNull(reference.RecordObject);

            // Act

            populator.Bind();

            // Assert

            Assert.IsNotNull(reference.RecordObject);
            Assert.IsNotNull(subject);
            Assert.AreEqual(subject, subjectReference.RecordObject);
        }

        [TestMethod]
        public void SingleResult_BindAndMake_Test()
        {
            MemoryTest.MemoryPopulator_BindAndMake_Test(
                populator => populator.Add<SubjectClass>().BindAndMake(),
                Assert.IsNotNull
            );
        }

        [TestMethod]
        public void ResultSet_BindAndMake_Test()
        {
            MemoryTest.MemoryPopulator_BindAndMake_Test(
                populator => populator.Add<SubjectClass>(4).BindAndMake(),
                resultSet => resultSet.ToList().ForEach(Assert.IsNotNull)
            );
        }

        private static void MemoryPopulator_BindAndMake_Test<T>(Func<IPopulator, T> bindAndMake, Action<T> assertion)
        {
            // Arrange

            IPopulator populator = StaticPopulatorFactory.CreateMemoryPopulator();

            // Act

            RecordReference<SubjectClass> singleSubjectReferenceBeforeBind = populator.Add<SubjectClass>();
            OperableList<SubjectClass> subjectReferenceSetBeforeBind = populator.Add<SubjectClass>(4);

            T result = bindAndMake(populator);

            SubjectClass singleSubjectBeforeBind = singleSubjectReferenceBeforeBind.RecordObject;

            IEnumerable<SubjectClass> subjectSetBeforeBind =
                subjectReferenceSetBeforeBind.Select(reference => reference.RecordObject).ToList();

            RecordReference<SubjectClass> singleSubjectAfterBind = populator.Add<SubjectClass>();
            OperableList<SubjectClass> subjectSetAfterBind = populator.Add<SubjectClass>(4);

            // Assert

            assertion(result);

            // Assert that BindAndMake() results in previously added objects being populated.
            Assert.IsNotNull(singleSubjectReferenceBeforeBind.RecordObject);
            subjectReferenceSetBeforeBind.ToList().ForEach(reference => Assert.IsNotNull(reference.RecordObject));

            // Assert that RecordReferences added after BindAndMake() are not populated.
            Assert.IsNull(singleSubjectAfterBind.RecordObject);
            subjectSetAfterBind.ToList().ForEach(subject => Assert.IsNull(subject.RecordObject));

            // Act

            populator.Bind();

            // Assert

            // Assert that objects populated by BindAndMake are not repopulated/reprocessed during Bind().
            Assert.AreEqual(singleSubjectBeforeBind, singleSubjectReferenceBeforeBind.RecordObject);
            for (int i = 0; i < subjectSetBeforeBind.Count(); i++)
                Assert.AreEqual(subjectSetBeforeBind.ElementAt(i), subjectReferenceSetBeforeBind[i].RecordObject);

            // Assert that RecordRefernces that are not populated after BindAndMake() are populated after Bind().
            Assert.IsNotNull(singleSubjectAfterBind.RecordObject);
            subjectSetAfterBind.ToList().ForEach(subject => Assert.IsNotNull(subject.RecordObject));
        }

        [TestMethod]
        public void SingleResult_Make_Test()
        {
            MemoryTest.MemoryPopulator_Make_Test(
                populator => populator.Add<SubjectClass>().Make(),
                Assert.IsNotNull
            );
        }

        [TestMethod]
        public void ResultSet_Make_Test()
        {
            MemoryTest.MemoryPopulator_Make_Test(
                populator => populator.Add<SubjectClass>(4).Make(),
                resultSet => resultSet.ToList().ForEach(Assert.IsNotNull)
            );
        }

        private static void MemoryPopulator_Make_Test<T>(Func<IPopulator, T> make, Action<T> assertion)
        {
            // Arrange

            IPopulator populator = StaticPopulatorFactory.CreateMemoryPopulator();

            // Act

            RecordReference<SubjectClass> singleSubjectReferenceBeforeBind = populator.Add<SubjectClass>();
            OperableList<SubjectClass> subjectReferenceSetBeforeBind = populator.Add<SubjectClass>(4);

            T result = make(populator);

            RecordReference<SubjectClass> singleSubjectAfterBind = populator.Add<SubjectClass>();
            OperableList<SubjectClass> subjectSetAfterBind = populator.Add<SubjectClass>(4);

            // Assert

            assertion(result);

            // Assert that Make() results in previously added objects not being populated.
            Assert.IsNull(singleSubjectReferenceBeforeBind.RecordObject);
            subjectReferenceSetBeforeBind.ToList().ForEach(reference => Assert.IsNull(reference.RecordObject));

            // Assert that RecordReferences added after Make() are not populated.
            Assert.IsNull(singleSubjectAfterBind.RecordObject);
            subjectSetAfterBind.ToList().ForEach(subject => Assert.IsNull(subject.RecordObject));

            // Act

            populator.Bind();

            // Assert

            // Assert that objects not populated by Make() are populated during Bind().
            Assert.IsNotNull(singleSubjectReferenceBeforeBind.RecordObject);
            subjectReferenceSetBeforeBind.ToList().ForEach(reference => Assert.IsNotNull(reference.RecordObject));

            // Assert that RecordRefernces that are not populated after Make() are populated after Bind().
            Assert.IsNotNull(singleSubjectAfterBind.RecordObject);
            subjectSetAfterBind.ToList().ForEach(subject => Assert.IsNotNull(subject.RecordObject));
        }

        [TestMethod]
        public void DeepSet_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA deep = populator.Add<DeepA>()
                .Set(m => m.DeepB.DeepC.DeepString, "ABCDEFG")
                .Set(m => m.DeepB.String, "QWERTY")
                .Set(m => m.DeepB.DeepC.IntList, () =>
                {
                    List<int> l = populator.Add<int>(5).Make().ToList();
                    l[2] = 22;
                    return l;
                })
                .Make();

            Console.WriteLine(deep.DeepB.DeepC.DeepString);
            Console.WriteLine(deep.DeepB.String);
            Console.WriteLine(deep.Integer);
            deep.DeepB.DeepC.IntList.ForEach(Console.WriteLine);
            Console.WriteLine(deep.DeepB.DeepC.IntList.Count);
            Console.WriteLine(deep.DeepB.DeepAList[0]);
        }

        [TestMethod]
        public void Set_List_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            RecordReference<ListSetterBaseType> listSetterBaseTypeReference = populator.Add<ListSetterBaseType>();

            RootReferenceParentOperableList<ListElementType, ListSetterBaseType> operableList = 
                listSetterBaseTypeReference.SetList(p => p.B.WithCollection.ElementList, 10);

            MemoryTest.PopulateForSetTest(operableList, populator);

            Assert.AreEqual("Me",
                listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementList[2].SubElement.AString);
            Assert.AreEqual("Me",
                listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementList[3].SubElement.AString);
            Assert.AreEqual("Me",
                listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementList[4].SubElement.AString);

            Assert.AreEqual("Hello", listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementList[5].AString);
            Assert.AreEqual("Hello", listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementList[6].AString);
            Assert.AreEqual("Hello", listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementList[7].AString);
            Assert.AreEqual("Hello", listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementList[8].AString);

            Assert.AreEqual(7,
                listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementList[9].SubElement.AnInt);
        }

        [TestMethod]
        public void Set_Array_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            RecordReference<ListSetterBaseType> listSetterBaseTypeReference = populator.Add<ListSetterBaseType>();

            RootReferenceParentOperableList<ListElementType, ListSetterBaseType> operableList =
                listSetterBaseTypeReference.SetList(p => p.B.WithCollection.ElementArray, 10);

            MemoryTest.PopulateForSetTest(operableList, populator);

            Assert.AreEqual("Me",
                listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementArray[2].SubElement.AString);
            Assert.AreEqual("Me",
                listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementArray[3].SubElement.AString);
            Assert.AreEqual("Me",
                listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementArray[4].SubElement.AString);

            Assert.AreEqual("Hello", listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementArray[5].AString);
            Assert.AreEqual("Hello", listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementArray[6].AString);
            Assert.AreEqual("Hello", listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementArray[7].AString);
            Assert.AreEqual("Hello", listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementArray[8].AString);

            Assert.AreEqual(7,
                listSetterBaseTypeReference.RecordObject.B.WithCollection.ElementArray[9].SubElement.AnInt);
        }

        private static void PopulateForSetTest(
            RootReferenceParentOperableList<ListElementType, ListSetterBaseType> operableList, IPopulator populator)
        {
            operableList.Skip(2).Set(p => p.SubElement.AString, "Me")
                .Skip(3).Set(p => p.AString, "Hello")
                .Skip(4).Take(1).Set(p => p.SubElement.AnInt, 7);

            populator.Bind();
        }

        private IMakeable<DeepA> GetMakeable(IPopulator populator)
        {
            IMakeable<DeepA> makeable = populator.Add<DeepA>().SetList(m => m.DeepB.DeepCList, 5)
                .Select(q => q.DeepDList, 5, 3).Skip(1).Take(3)
                .Set(r => r.Skip(1).Set(m => m.Integer, 7));

            return makeable;
        }

        private static void AssertParentDeepA(DeepA deepA)
        {
            deepA.DeepB.DeepCList.Skip(1).Take(3).ToList()
                .ForEach(deepC => deepC.DeepDList.Skip(1).ToList()
                    .ForEach(deepD => Assert.AreEqual(7, deepD.Integer)));
        }

        [TestMethod]
        public void DeepPropertySetting_ReferenceParent_Make_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();
            IMakeable<DeepA> makeable = this.GetMakeable(populator);

            DeepA result = makeable.Make();
            MemoryTest.AssertParentDeepA(result);
        }

        [TestMethod]
        public void DeepPropertySetting_ReferenceParent_MakeAndBind_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();
            IMakeable<DeepA> makeable = this.GetMakeable(populator);

            RecordReference<SubjectClass> subjectReference = populator.Add<SubjectClass>();

            DeepA result = makeable.BindAndMake();
            MemoryTest.AssertParentDeepA(result);
            Assert.IsNotNull(subjectReference.RecordObject);
        }

        [TestMethod]
        public void ExplicitlySetAListViaFunction_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            populator.Add<ListSetterBaseTypeB>().Set(p => p.WithCollection.ElementList, () =>
            {
                var result = new List<ListElementType>();
                return result;
            });

            populator.Bind();
        }

        [TestMethod]
        public void ExplicitlySetAList_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            populator.Add<ListSetterBaseTypeB>().Set(p => p.WithCollection.ElementList, new List<ListElementType>());

            populator.Bind();
        }

        [TestMethod]
        public void ValueType_DirectRequest_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            RecordReference<int> resultRecord = populator.Add<int>();
            populator.Bind();
            int result = resultRecord.RecordObject;

            Assert.IsTrue(result != 0);
        }

        private IMakeableCollectionContainer<DeepA> DeepPropertySetting_ListParent_SettersTest(IPopulator populator)
        {
            IMakeableCollectionContainer<DeepA> result = populator.Add<DeepA>(5)
                .Select(q => q.DeepB.DeepCList, 5, 3).Skip(2).Take(3)
                .Set(r => r.Skip(2).Take(2).Set(s => s.DeepString, "I"))
                .Set(r => r.Skip(4).Take(2).Set(s => s.DeepString, "II"));

            return result;
        }

        [TestMethod]
        public void DeepPropertySetting_Make_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IMakeableCollectionContainer<DeepA> makeable = this.DeepPropertySetting_ListParent_SettersTest(populator);
            IEnumerable<DeepA> result = makeable.Make();

            MemoryTest.DeepPropertySetting_Test(result);
        }

        [TestMethod]
        public void DeepPropertySetting_BindAndMake_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            RecordReference<SubjectClass> subjectReference = populator.Add<SubjectClass>();

            IMakeableCollectionContainer<DeepA> makeable = this.DeepPropertySetting_ListParent_SettersTest(populator);
            IEnumerable<DeepA> result = makeable.BindAndMake();

            MemoryTest.DeepPropertySetting_Test(result);

            SubjectClass subject = subjectReference.RecordObject;
            Assert.IsNotNull(subject);
            Assert.IsNotNull(subject.AnEmailAddress);
            Assert.AreNotEqual(0, subject.Integer);
        }

        private static void DeepPropertySetting_Test(IEnumerable<DeepA> makeable)
        {
            List<DeepA> list = makeable.ToList();

            Assert.AreEqual(5, list.Count);

            int deepCCount;
            int deepACount = 0;
            list.ForEach(deepA =>
            {
                Assert.AreEqual(10, deepA.DeepB.DeepCList.Count);

                if (deepACount < 2)
                {
                    deepA.DeepB.DeepCList.ForEach(deepC =>
                    {
                        Assert.AreNotEqual("I", deepC.DeepString);
                        Assert.AreNotEqual("II", deepC.DeepString);
                    });

                    deepACount++;
                    return;
                }

                deepCCount = 0;
                deepA.DeepB.DeepCList.ForEach(deepC =>
                {
                    if (deepCCount < 2 || deepCCount >= 6)
                    {
                        Assert.AreNotEqual("I", deepC.DeepString);
                        Assert.AreNotEqual("II", deepC.DeepString);
                    }
                    else if (deepCCount >= 2 && deepCCount < 4)
                    {
                        Assert.AreEqual("I", deepC.DeepString);
                    }
                    else if (deepCCount >= 4 && deepCCount < 6)
                    {
                        Assert.AreEqual("II", deepC.DeepString);
                    }
                    else
                    {
                        throw new Exception($"Range error. Count = {deepCCount}");
                    }

                    deepCCount++;
                });

                deepACount++;
            });

            Assert.AreEqual(5, deepACount);
        }

        [TestMethod]
        public void Test2()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            OperableList<ManualKeyPrimaryTableClass> resultReference = populator.Add<ManualKeyPrimaryTableClass>(2);

            populator.Bind();;
        }

        [TestMethod]
        public void Explicit_Property_Assignment_In_Recursive_Branch_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            DeepA deepA = populator.Add<DeepA>().SetList(m => m.DeepB.DeepAList, 5).Set(m => m.DeepB.String, "Hello")
                .Make();
        }
    }
}