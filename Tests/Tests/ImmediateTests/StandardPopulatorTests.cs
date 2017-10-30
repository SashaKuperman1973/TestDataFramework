/*
    Copyright 2016, 2017 Alexander Kuperman

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
using System.Net;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.ListOperations;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;
using Tests.Mocks;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class StandardPopulatorTests
    {
        private IAttributeDecorator attributeDecorator;
        private Mock<IPersistence> persistenceMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.attributeDecorator = new StandardAttributeDecorator(attributeDecorator => null, null);
            this.persistenceMock = new Mock<IPersistence>();
        }

        [TestMethod]
        public void Add_Test()
        {
            // Arrange

            var expected = new SubjectClass();
            var populator = new StandardPopulator(Helpers.GetTypeGeneratorMock(expected).Object, new MockPersistence(),
                this.attributeDecorator, null, null, null);

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

            var expected = new SubjectClass { AnEmailAddress = "email"};
            var mockPersistence = new MockPersistence();
            var populator = new StandardPopulator(Helpers.GetTypeGeneratorMock(expected).Object, mockPersistence,
                this.attributeDecorator, null, null, null);

            // Act

            populator.Add<SubjectClass>();
            populator.Bind();

            populator.Add<SubjectClass>();
            populator.Bind();

            RecordReference<SubjectClass> reference = populator.Add<SubjectClass>();
            populator.Bind();

            // Assert

            Assert.AreEqual(1, mockPersistence.Storage.Count);
            Assert.AreEqual(reference.RecordObject.AnEmailAddress, mockPersistence.Storage[0]["AnEmailAddress"]);
        }

        [TestMethod]
        public void Bind_Test()
        {
            // Arrange

            const int integer = 5;
            const string text = "abcde";

            var persistence = new MockPersistence();

            var inputRecord = new SubjectClass {Integer = integer, Text = text,};

            Mock<ITypeGenerator> typeGeneratorMock = Helpers.GetTypeGeneratorMock(inputRecord);

            var populator = new StandardPopulator(typeGeneratorMock.Object, persistence, this.attributeDecorator, null, null, null);

            // Act

            populator.Add<SubjectClass>();
            populator.Bind();

            // Assert

            IDictionary<string, object> record = persistence.Storage.FirstOrDefault();

            Assert.IsNotNull(record);

            Assert.AreEqual(record["Integer"], integer);
            Assert.AreEqual(record["Text"], text);
        }

        [TestMethod]
        public void Bind_SingleResult_Test()
        {
            // Arrange

            var populator = new StandardPopulator(null, this.persistenceMock.Object, null, null, null, null);

            var referenceMock = new Mock<RecordReference<SubjectClass>>(null, null, null);

            // Act

            populator.Bind(referenceMock.Object);

            // Assert

            referenceMock.VerifyGet(m => m.IsProcessed);
            referenceMock.VerifyGet(m => m.PreBoundObject);
            referenceMock.Verify(m => m.Populate());
            this.persistenceMock.Verify(m => m.Persist(
                It.Is<IEnumerable<RecordReference>>(referenceSet => referenceSet.Single() == referenceMock.Object)));
        }

        [TestMethod]
        public void Bind_ResultSet_Test()
        {
            // Arrange

            var populator = new StandardPopulator(null, this.persistenceMock.Object, null, null, null, null);

            var referenceMock1 = new Mock<RecordReference<SubjectClass>>(null, null, null);
            var referenceMock2 = new Mock<RecordReference<SubjectClass>>(null, null, null);
            var set = new OperableList<SubjectClass>(new List<RecordReference<SubjectClass>> { referenceMock1.Object, referenceMock2.Object },
                null, null) {IsProcessed = true};

            // Act

            populator.Bind(set);

            // Assert

            referenceMock1.VerifyGet(m => m.IsProcessed);
            referenceMock1.VerifyGet(m => m.PreBoundObject);
            referenceMock1.Verify(m => m.Populate());

            referenceMock2.VerifyGet(m => m.IsProcessed);
            referenceMock2.VerifyGet(m => m.PreBoundObject);
            referenceMock2.Verify(m => m.Populate());

            this.persistenceMock.Verify(m => m.Persist(
                It.Is<IEnumerable<RecordReference>>(referenceSet => referenceSet.Single() == referenceMock1.Object)));

            this.persistenceMock.Verify(m => m.Persist(
                It.Is<IEnumerable<RecordReference>>(referenceSet => referenceSet.Single() == referenceMock2.Object)));
        }

        [TestMethod]
        public void MutliRecord_Test()
        {
            // Arrange

            var persistence = new MockPersistence();

            Mock<ITypeGenerator> typeGeneratorMock = Helpers.GetTypeGeneratorMock(new SubjectClass());
            Helpers.SetupTypeGeneratorMock(typeGeneratorMock, new SecondClass());

            var populator = new StandardPopulator(typeGeneratorMock.Object, persistence, this.attributeDecorator, null, null, null);

            // Act

            populator.Add<SubjectClass>();
            populator.Add<SecondClass>();
            populator.Bind();

            // Assert

            Assert.AreEqual(2, persistence.Storage.Count);
        }

        [TestMethod]
        public void Extend_Test()
        {
            // Arrange

            var handledTypeGeneratorMock = new Mock<IHandledTypeGenerator>();
            var valueGetterDictionary = new Dictionary<Type, HandledTypeValueGetter>();
            handledTypeGeneratorMock.SetupGet(m => m.HandledTypeValueGetterDictionary).Returns(valueGetterDictionary);
            var populator = new StandardPopulator(null, null, null, handledTypeGeneratorMock.Object, null, null);

            var subject = new SubjectClass();

            // Act

            populator.Extend(typeof(SubjectClass), type => subject);

            // Assert

            Assert.AreEqual(1, valueGetterDictionary.Count);
            Assert.AreEqual(subject, valueGetterDictionary[typeof(SubjectClass)](typeof(SubjectClass)));
        }
    }
}