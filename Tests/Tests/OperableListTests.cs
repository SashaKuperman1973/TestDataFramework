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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests
{
    [TestClass]
    public class OperableListTests
    {
        private OperableList<SubjectClass> GetOperableList()
        {
            return new OperableList<SubjectClass>(this.SubjectClassRecords,
                this.valueGuaranteePopulatorMock.Object, this.populatorMock.Object,
                null,
                null,
                null,
                null);
        }

        private List<RecordReference<SubjectClass>> SubjectClassRecords => this.subjectClassRecordMocks
            .Select(r => r.Object).ToList();

        private void AddSubjectClass(SubjectClass subject)
        {
            Mock<RecordReference<SubjectClass>> recordReferenceMock = Helpers.GetMock<RecordReference<SubjectClass>>();
            this.subjectClassRecordMocks.Add(recordReferenceMock);
            recordReferenceMock.Setup(m => m.RecordObject).Returns(subject);
        }

        private Mock<BasePopulator> populatorMock;
        private Mock<ValueGuaranteePopulator> valueGuaranteePopulatorMock;
        private Mock<ITypeGenerator> typeGeneratorMock;

        private List<Mock<RecordReference<SubjectClass>>> subjectClassRecordMocks;

        [TestInitialize]
        public void Initialize()
        {
            this.subjectClassRecordMocks = new List<Mock<RecordReference<SubjectClass>>>();
            this.populatorMock = Helpers.GetMock<BasePopulator>();
            this.valueGuaranteePopulatorMock = Helpers.GetMock<ValueGuaranteePopulator>();
            this.typeGeneratorMock = new Mock<ITypeGenerator>();
        }

        [TestMethod]
        public void RecordObjects_Test()
        {
            // Arrange

            var subject = new SubjectClass();
            this.AddSubjectClass(subject);

            // Act

            OperableList<SubjectClass> operableList = this.GetOperableList();

            // Assert

            Assert.AreEqual(subject, operableList.RecordObjects.Single());
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_ByValue_DefaultPercentage_Test()
        {
            void Call(OperableList<SubjectClass> collection, SubjectClass[] data)
            {
                collection.GuaranteeByPercentageOfTotal(data);
            }

            this.GuaranteeByPercentageOfTotal_ByValue_Test(Call, 10);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_ByValue_Test()
        {
            void Call(OperableList<SubjectClass> collection, SubjectClass[] data)
            {
                collection.GuaranteeByPercentageOfTotal(data, 15);
            }

            this.GuaranteeByPercentageOfTotal_ByValue_Test(Call, 15);
        }

        private void GuaranteeByPercentageOfTotal_ByValue_Test(Action<OperableList<SubjectClass>, SubjectClass[]> call,
            int frequencyPercentage)
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            var subject = new SubjectClass();
            SubjectClass[] data = {subject};

            // Act

            call(operableList, data);

            // Assert

            Assert.AreEqual(subject, operableList.GuaranteedValues.First().Values.First());
            Assert.AreEqual(frequencyPercentage, operableList.GuaranteedValues.First().FrequencyPercentage);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_ByObject_DefaultPercentage_Test()
        {
            void Call(OperableList<SubjectClass> collection, object[] values)
            {
                collection.GuaranteeByPercentageOfTotal(values);
            }

            this.GuaranteeByPercentageOfTotal_ByObject_Test(Call);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_ByObject_Test()
        {
            void Call(OperableList<SubjectClass> collection, object[] values)
            {
                collection.GuaranteeByPercentageOfTotal(values, 15);
            }

            this.GuaranteeByPercentageOfTotal_ByObject_Test(Call);
        }

        private void GuaranteeByPercentageOfTotal_ByObject_Test(Action<OperableList<SubjectClass>, object[]> call)
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();

            var a = new SubjectClass();
            Func<SubjectClass> b = () => new SubjectClass();

            // Act

            call(operableList, new object[] {a, b});

            // Assert

            Assert.AreEqual(a, operableList.GuaranteedValues.First().Values.ElementAt(0));
            Assert.AreEqual(b, operableList.GuaranteedValues.First().Values.ElementAt(1));
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_ByValueFunc_DefaultPercentage_Test()
        {
            void Call(OperableList<SubjectClass> subject, Func<SubjectClass>[] data)
            {
                subject.GuaranteeByPercentageOfTotal(data);
            }

            this.GuaranteeByPercentageOfTotal_ByValueFunc_Test(Call, 10);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_ByValueFunc_Test()
        {
            void Call(OperableList<SubjectClass> subject, Func<SubjectClass>[] data)
            {
                subject.GuaranteeByPercentageOfTotal(data, 15);
            }

            this.GuaranteeByPercentageOfTotal_ByValueFunc_Test(Call, 15);
        }

        public void GuaranteeByPercentageOfTotal_ByValueFunc_Test(
            Action<OperableList<SubjectClass>, Func<SubjectClass>[]> call, int frequencyPercentage)
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            Func<SubjectClass> subject = () => new SubjectClass();
            Func<SubjectClass>[] data = {subject};

            // Act

            call(operableList, data);

            // Assert

            Assert.AreEqual(subject, operableList.GuaranteedValues.First().Values.First());
            Assert.AreEqual(frequencyPercentage, operableList.GuaranteedValues.First().FrequencyPercentage);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_ByValue_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            var subject = new SubjectClass();
            SubjectClass[] data = {subject};

            // Act

            operableList.GuaranteeByFixedQuantity(data, 10);

            // Assert

            Assert.AreEqual(subject, operableList.GuaranteedValues.First().Values.First());
            Assert.AreEqual(10, operableList.GuaranteedValues.First().TotalFrequency);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_ByObject_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();

            var a = new SubjectClass();
            Func<SubjectClass> b = () => new SubjectClass();

            // Act

            operableList.GuaranteeByFixedQuantity(new object[] {a, b}, 10);

            // Assert

            Assert.AreEqual(a, operableList.GuaranteedValues.First().Values.ElementAt(0));
            Assert.AreEqual(b, operableList.GuaranteedValues.First().Values.ElementAt(1));
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_ByValueFunc_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            Func<SubjectClass> subject = () => new SubjectClass();
            Func<SubjectClass>[] data = {subject};

            // Act

            operableList.GuaranteeByFixedQuantity(data, 10);

            // Assert

            Assert.AreEqual(subject, operableList.GuaranteedValues.First().Values.First());
            Assert.AreEqual(10, operableList.GuaranteedValues.First().TotalFrequency);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_ByValue_DefaultQuantity_IsCollectionSize_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();

            var subject1 = new SubjectClass();
            var subject2 = new SubjectClass();
            var subject3 = new SubjectClass();
            SubjectClass[] data = {subject1, subject2, subject3};

            // Act

            operableList.GuaranteeByFixedQuantity(data);

            // Assert

            Assert.AreEqual(data.Length, operableList.GuaranteedValues.First().TotalFrequency);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_ByObject_DefaultQuantity_IsCollectionSize_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();

            var subject1 = new SubjectClass();
            Func<SubjectClass> subject2 = () => new SubjectClass();
            var subject3 = new SubjectClass();
            object[] data = {subject1, subject2, subject3};

            // Act

            operableList.GuaranteeByFixedQuantity(new object[] {subject1, subject2, subject3});

            // Assert

            Assert.AreEqual(data.Length, operableList.GuaranteedValues.First().TotalFrequency);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_ByValueFunc_DefaultQuantity_IsCollectionSize_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            Func<SubjectClass> subject1 = () => new SubjectClass();
            Func<SubjectClass> subject2 = () => new SubjectClass();
            Func<SubjectClass> subject3 = () => new SubjectClass();
            Func<SubjectClass>[] data = {subject1, subject2, subject3};

            // Act

            operableList.GuaranteeByFixedQuantity(data);

            // Assert

            Assert.AreEqual(data.Length, operableList.GuaranteedValues.First().TotalFrequency);
        }

        [TestMethod]
        public void AddToReferences_Test()
        {
            // Arrange

            this.AddSubjectClass(new SubjectClass());
            OperableList<SubjectClass> operableList = this.GetOperableList();

            var data = new List<RecordReference>();

            // Act

            operableList.AddToReferences(data);

            // Assert

            Assert.AreEqual(operableList.InternalList.Single(), data.Single());
        }

        [TestMethod]
        public void GetEnumerator_Generic_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            var reference = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.InternalList.Add(reference);

            // Act

            RecordReference<SubjectClass> actual;

            using (IEnumerator<RecordReference<SubjectClass>> enumerator = operableList.GetEnumerator())
            {
                enumerator.MoveNext();
                actual = enumerator.Current;
            }

            // Assert

            Assert.AreEqual(reference, actual);
        }

        [TestMethod]
        public void GetEnumerator_NonGeneric_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            var reference = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.InternalList.Add(reference);

            // Act

            RecordReference<SubjectClass> actual;

            IEnumerator enumerator = ((IEnumerable) operableList).GetEnumerator();
            enumerator.MoveNext();
            actual = (RecordReference<SubjectClass>) enumerator.Current;

            // Assert

            Assert.AreEqual(reference, actual);
        }

        [TestMethod]
        public void Add_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            var reference = Helpers.GetObject<RecordReference<SubjectClass>>();

            // Act

            operableList.Add(reference);

            // Assert

            Assert.AreEqual(reference, operableList.InternalList.Single());
        }

        [TestMethod]
        public void CopyTo_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            var reference = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference);
            var array = new RecordReference<SubjectClass>[1];

            // Act

            operableList.CopyTo(array, 0);

            // Assert

            Assert.AreEqual(reference, array[0]);
        }

        [TestMethod]
        public void Count_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            var reference = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference);

            // Act

            int c = operableList.Count;

            // Assert

            Assert.AreEqual(1, c);
        }

        [TestMethod]
        public void Item_Read_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            var reference = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference);

            // Act

            RecordReference<SubjectClass> result = operableList[0];

            // Assert

            Assert.AreEqual(reference, result);
        }

        [TestMethod]
        public void Item_Write_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            var reference = Helpers.GetObject<RecordReference<SubjectClass>>();
            var expected = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference);

            // Act

            operableList[0] = expected;

            // Assert

            Assert.AreEqual(expected, operableList.InternalList[0]);
        }

        [TestMethod]
        public void Clear_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();
            var reference = Helpers.GetObject<RecordReference<SubjectClass>>();

            var subject = new SubjectClass();
            SubjectClass[] data = {subject};

            operableList.GuaranteeByFixedQuantity(data);
            operableList.Add(reference);

            // Act

            operableList.Clear();

            // Assert

            Assert.AreEqual(0, operableList.GuaranteedValues.Count);
            Assert.AreEqual(0, operableList.InternalList.Count);
        }

        [TestMethod]
        public void Contains_Test()
        {
            OperableList<SubjectClass> operableList = this.GetOperableList();
            var reference = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference);

            // Act

            bool result = operableList.Contains(reference);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsReadOnly_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();

            // Act

            bool result = operableList.IsReadOnly;

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Remove_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();

            var reference1 = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference1);
            var reference2 = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference2);

            // Act

            operableList.Remove(reference2);

            // Assert

            Assert.IsTrue(operableList.Contains(reference1));
            Assert.IsFalse(operableList.Contains(reference2));
        }

        [TestMethod]
        public void IndexOf_Test()
        {
            // Arrange

            OperableList<SubjectClass> operableList = this.GetOperableList();

            var reference1 = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference1);
            var reference2 = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference2);

            // Act

            int index = operableList.IndexOf(reference2);

            // Assert

            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void Insert_Test()
        {
            // Arange

            OperableList<SubjectClass> operableList = this.GetOperableList();

            var reference1 = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference1);
            var reference2 = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference2);

            var subject = new RecordReference<SubjectClass>(null, null, null, null, null, null);

            // Act

            operableList.Insert(1, subject);

            // Assert

            Assert.AreEqual(0, operableList.IndexOf(reference1));
            Assert.AreEqual(2, operableList.IndexOf(reference2));

            Assert.AreEqual(1, operableList.IndexOf(subject));
        }

        [TestMethod]
        public void RemoveAt_Test()
        {
            // Arange

            OperableList<SubjectClass> operableList = this.GetOperableList();

            var reference1 = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference1);

            var subjectReference = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(subjectReference);

            var reference2 = Helpers.GetObject<RecordReference<SubjectClass>>();
            operableList.Add(reference2);

            // Act

            operableList.RemoveAt(1);

            // Assert

            Assert.IsFalse(operableList.Contains(subjectReference));
        }

        [TestMethod]
        public void Populate_InternalList_IsPopulated_Test()
        {
            // Arrange

            this.AddSubjectClass(null);
            this.AddSubjectClass(null);
            OperableList<SubjectClass> operableList = this.GetOperableList();

            // Act

            operableList.Populate();

            // Assert

            this.valueGuaranteePopulatorMock.Verify(
                m => m.Bind(It.IsAny<OperableList<SubjectClass>>(), It.IsAny<List<GuaranteedValues>>(),
                    It.IsAny<IValueGauranteePopulatorContextService>()), Times.Never);

            this.subjectClassRecordMocks.ForEach(m => m.Verify(n => n.Populate()));
        }

        [TestMethod]
        public void Populate_GuaranteedValues_AreProcessed_Test()
        {
            // Arrange

            var operableList =
                new OperableList<SubjectClass>(null, this.valueGuaranteePopulatorMock.Object, this.populatorMock.Object,
                    null, null, null, null);

            var subject = new SubjectClass();
            SubjectClass[] data = {subject};

            operableList.GuaranteeByPercentageOfTotal(data, 10);

            // Act

            operableList.Populate();

            // Assert

            this.valueGuaranteePopulatorMock.Verify(m => m.Bind(operableList, operableList.GuaranteedValues,
                It.IsAny<ValueSetContextService>()));
        }

        [TestMethod]
        public void Populate_NoAction_IfPopulated_Test()
        {
            var typeGeneratorMock = new Mock<ITypeGenerator>();
            var operableList = Helpers.GetObject<OperableList<SubjectClass>>();

            var subject1 = new RecordReference<SubjectClass>(typeGeneratorMock.Object, null, null, null, null, null);

            operableList.Add(subject1);

            // Act

            operableList.Populate();
            operableList.Populate();

            // Assert

            typeGeneratorMock.Verify(m => m.GetObject<SubjectClass>(It.IsAny<IEnumerable<ExplicitPropertySetter>>()),
                Times.Once);
        }

        [TestMethod]
        public void Ignore_Test()
        {
            var operableList = Helpers.GetObject<OperableList<ClassWithSideEffectProperty>>();

            var objectGraphServiceMock = new Mock<IObjectGraphService>();
            PropertyInfo propertyInfo =
                typeof(ClassWithSideEffectProperty).GetProperty(nameof(ClassWithSideEffectProperty.SideEffectProperty));

            objectGraphServiceMock
                .Setup(m => m.GetObjectGraph<ClassWithSideEffectProperty, int>(p => p.SideEffectProperty))
                .Returns(new List<PropertyInfo> {propertyInfo});

            var recordReference =
                new RecordReference<ClassWithSideEffectProperty>(null, null, null, objectGraphServiceMock.Object, null,
                    null);
            operableList.Add(recordReference);

            var subject = new ClassWithSideEffectProperty();

            // Act

            operableList.Ignore(p => p.SideEffectProperty);
            recordReference.ExplicitPropertySetters[0].Action(subject);

            // Assert

            Assert.AreEqual(0, subject.i);
        }
    }
}