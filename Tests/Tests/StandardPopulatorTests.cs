/*
    Copyright 2016 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;
using System.Linq;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using Tests.Mocks;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardPopulatorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();
        }

        [TestMethod]
        public void Add_Test()
        {
            // Arrange
            var expected = new SubjectClass();
            var populator = new StandardPopulator(Helpers.GetTypeGeneratorMock(expected).Object, new MockPersistence());

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
            var populator = new StandardPopulator(Helpers.GetTypeGeneratorMock(expected).Object, mockPersistence);

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
        public void StandardPopulatorTest_Bind()
        {
            // Arrange

            const int integer = 5;
            const string text = "abcde";

            var persistence = new MockPersistence();

            var inputRecord = new SubjectClass {Integer = integer, Text = text,};

            Mock<ITypeGenerator> typeGeneratorMock = Helpers.GetTypeGeneratorMock(inputRecord);

            var populator = new StandardPopulator(typeGeneratorMock.Object, persistence);

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
        public void StandardPopulator_MutliRecord_Test()
        {
            // Arrange

            var persistence = new MockPersistence();

            Mock<ITypeGenerator> typeGeneratorMock = Helpers.GetTypeGeneratorMock(new SubjectClass());
            Helpers.SetupTypeGeneratorMock(typeGeneratorMock, new SecondClass());

            var populator = new StandardPopulator(typeGeneratorMock.Object, persistence);

            // Act

            populator.Add<SubjectClass>();
            populator.Add<SecondClass>();
            populator.Bind();

            // Assert

            Assert.AreEqual(2, persistence.Storage.Count);
        }
    }
}