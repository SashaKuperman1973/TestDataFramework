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
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;
using Tests.Mocks;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardPopulatorTests
    {
        private IAttributeDecorator attributeDecorator;
        private Mock<IHandledTypeGenerator> handledTypeGeneratorMock;
        private Mock<IPersistence> persistenceMock;
        private StandardPopulator populator;
        private Mock<ITypeGenerator> typeGeneratorMock;
        private Mock<IValueGenerator> valueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.attributeDecorator = new StandardAttributeDecorator(null, new AssemblyWrapper(), new Schema());
            this.persistenceMock = new Mock<IPersistence>();
            this.typeGeneratorMock = new Mock<ITypeGenerator>();
            this.handledTypeGeneratorMock = new Mock<IHandledTypeGenerator>();
            this.valueGeneratorMock = new Mock<IValueGenerator>();

            this.populator = new StandardPopulator(this.typeGeneratorMock.Object, this.persistenceMock.Object,
                this.attributeDecorator, this.handledTypeGeneratorMock.Object, this.valueGeneratorMock.Object, null,
                null, null);
        }

        [TestMethod]
        public void Add_Test()
        {
            // Arrange

            var expected = new SubjectClass();
            var populator = new StandardPopulator(Helpers.GetTypeGeneratorMock(expected).Object, new MockPersistence(),
                this.attributeDecorator, null, null, null, null, null);

            // Act

            RecordReference<SubjectClass> reference = populator.Add<SubjectClass>();
            populator.Bind();

            // Assert

            Assert.IsNotNull(reference);
            Assert.AreEqual(expected.GetType(), reference.RecordType);
            Assert.AreEqual(expected, reference.RecordObject);
        }

        [TestMethod]
        public void Records_Cleared_After_Bind_Test()
        {
            // Arrange

            var expected = new SubjectClass {AnEmailAddress = "email"};
            var mockPersistence = new MockPersistence();
            var populator = new StandardPopulator(Helpers.GetTypeGeneratorMock(expected).Object, mockPersistence,
                this.attributeDecorator, null, null, null, null, null);

            // Act

            populator.Add<SubjectClass>();
            populator.Bind();

            populator.Add<SubjectClass>();
            populator.Bind();

            RecordReference<SubjectClass> reference = populator.Add<SubjectClass>();
            populator.Bind();

            // Assert

            Assert.AreEqual(3, mockPersistence.Storage.Count);
            Assert.AreEqual(reference.RecordObject.AnEmailAddress, mockPersistence.Storage[0]["AnEmailAddress"]);
        }

        [TestMethod]
        public void Bind_Test()
        {
            // Arrange

            const int integer = 5;
            const string text = "abcde";

            var inputRecord = new SubjectClass {Integer = integer, Text = text};
            Helpers.SetupTypeGeneratorMock(this.typeGeneratorMock, inputRecord);

            var mockPersistence = new MockPersistence();
            var populator = new StandardPopulator(this.typeGeneratorMock.Object, mockPersistence,
                this.attributeDecorator, null, null, null, null, null);

            // Act

            populator.Add<SubjectClass>();
            populator.Bind();

            // Assert

            IDictionary<string, object> record = mockPersistence.Storage.FirstOrDefault();

            Assert.IsNotNull(record);

            Assert.AreEqual(record["Integer"], integer);
            Assert.AreEqual(record["Text"], text);
        }

        [TestMethod]
        public void Bind_SingleResult_Test()
        {
            // Arrange

            var referenceMock = new Mock<RecordReference<SubjectClass>>(null, null, null, null, null, null);

            // Act

            this.populator.Bind(referenceMock.Object);

            // Assert

            referenceMock.VerifyGet(m => m.IsPopulated);
            referenceMock.Verify(m => m.Populate());
            this.persistenceMock.Verify(m => m.Persist(
                It.Is<IEnumerable<RecordReference>>(referenceSet => referenceSet.Single() == referenceMock.Object)));
        }

        [TestMethod]
        public void Bind_ResultSet_Test()
        {
            // Arrange

            var referenceMock1 = new Mock<RecordReference<SubjectClass>>(null, null, null, null, null, null);
            var referenceMock2 = new Mock<RecordReference<SubjectClass>>(null, null, null, null, null, null);
            var set = new ListParentOperableList<SubjectClass>(
                null,
                new List<RecordReference<SubjectClass>> {referenceMock1.Object, referenceMock2.Object},
                null, null);

            // Act

            this.populator.Bind(set);

            // Assert

            referenceMock1.VerifyGet(m => m.IsPopulated);
            referenceMock1.Verify(m => m.Populate());

            referenceMock2.VerifyGet(m => m.IsPopulated);
            referenceMock2.Verify(m => m.Populate());

            Func<IEnumerable<RecordReference>, bool> verifyPersistence = referenceSet =>
            {
                referenceSet = referenceSet.ToList();

                return referenceSet.Count() == 2 &&
                       referenceSet.Contains(referenceMock1.Object) &&
                       referenceSet.Contains(referenceMock2.Object);
            };

            this.persistenceMock.Verify(m => m.Persist(
                It.Is<IEnumerable<RecordReference>>(referenceSet => verifyPersistence(referenceSet))));
        }

        [TestMethod]
        public void Bind_RecordReference_Test()
        {
            // Arrange

            var referenceMock = new Mock<RecordReference<SubjectClass>>(null, null, null, null, null, null);

            // Act

            this.populator.Bind(referenceMock.Object);

            // Assert

            referenceMock.Verify(m => m.Populate(), Times.Once);
        }

        [TestMethod]
        public void MutliRecord_Test()
        {
            // Arrange

            var mockPersistence = new MockPersistence();
            var populator = new StandardPopulator(this.typeGeneratorMock.Object, mockPersistence,
                this.attributeDecorator, null, null, null, null, null);

            Helpers.SetupTypeGeneratorMock(this.typeGeneratorMock, new SubjectClass());
            Helpers.SetupTypeGeneratorMock(this.typeGeneratorMock, new SecondClass());

            // Act

            populator.Add<SubjectClass>();
            populator.Add<SecondClass>();
            populator.Bind();

            // Assert

            Assert.AreEqual(2, mockPersistence.Storage.Count);
        }

        [TestMethod]
        public void Extend_Test()
        {
            // Arrange

            var valueGetterDictionary = new Dictionary<Type, HandledTypeValueGetter>();
            this.handledTypeGeneratorMock.SetupGet(m => m.HandledTypeValueGetterDictionary)
                .Returns(valueGetterDictionary);

            var subject = new SubjectClass();

            // Act

            this.populator.Extend(typeof(SubjectClass), type => subject);

            // Assert

            Assert.AreEqual(1, valueGetterDictionary.Count);
            Assert.AreEqual(subject, valueGetterDictionary[typeof(SubjectClass)](typeof(SubjectClass)));
        }

        [TestMethod]
        public void Bind_RecordReference_Is_Populated_Test()
        {
            // Arrange

            var recordReference =
                new RecordReference<SubjectClass>(null, null, null, null, null, null) {IsPopulated = true};

            // Act

            this.populator.Bind(recordReference);

            // Assert

            this.persistenceMock.Verify(m => m.Persist(It.IsAny<RecordReference[]>()), Times.Never);
            this.typeGeneratorMock.Verify(m => m.GetObject<SubjectClass>(It.IsAny<List<ExplicitPropertySetter>>()), Times.Never);
        }

        [TestMethod]
        public void Add_Gets_Collection_Test()
        {
            // Arrange

            var primaryKeys = new[]
            {
                new RecordReference<SecondClass>(null, null, null, null, null, null),
                new RecordReference<SecondClass>(null, null, null, null, null, null)
            };

            // Act

            ListParentOperableList<SubjectClass> result =
                this.populator.Add<SubjectClass>(5, primaryKeys);

            // Assert

            Assert.AreEqual(result, this.populator.Populatables.Single(x => x is ListParentOperableList<SubjectClass>));
        }

        [TestMethod]
        public void ValueGenerator_Get()
        {
            IValueGenerator result = this.populator.ValueGenerator;

            Assert.AreEqual(this.valueGeneratorMock.Object, result);
        }

        [TestMethod]
        public void Clear_Test()
        {
            this.populator.Populatables.Add(new RecordReference<SubjectClass>(null, null, null, null, null, null));

            // Act

            this.populator.Clear();

            // Assert

            Assert.AreEqual(0, this.populator.Populatables.Count);
        }
    }
}