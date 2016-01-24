using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator;
using TestDataFramework.TypeGenerator;
using Tests.Mocks;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardPopulatorTests
    {
        private Mock<ITypeGenerator> typeGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.typeGeneratorMock = new Mock<ITypeGenerator>();
        }

        [TestMethod]
        public void Add_Test(string testValue)
        {
            // Arrange

            var populator = new StandardPopulator(this.typeGeneratorMock.Object, null);

            // Act

            RecordReference<SubjectClass> reference = populator.Add<SubjectClass>();

            // Assert

            Assert.IsNotNull(reference);
            Assert.IsNull(reference.RecordType);
            Assert.IsNull(reference.RecordObject);
        }

        [TestMethod]
        public void StandardPopulatorTest_Bind()
        {
            // Arrange

            const int integer = 5;
            const string text = "abcde";

            var persistence = new MockPersistence();

            this.typeGeneratorMock.Setup(m => m.GetObject(It.Is<Type>(t => t == typeof(SubjectClass)))).Returns(new SubjectClass { Integer = integer, Text = text,});

            var populator = new StandardPopulator(this.typeGeneratorMock.Object, persistence);

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

            this.typeGeneratorMock.Setup(m => m.GetObject(It.Is<Type>(t => t == typeof(SubjectClass)))).Returns(new SubjectClass());
            this.typeGeneratorMock.Setup(m => m.GetObject(It.Is<Type>(t => t == typeof(SecondClass)))).Returns(new SecondClass());

            var populator = new StandardPopulator(this.typeGeneratorMock.Object, persistence);

            // Act

            populator.Add<SubjectClass>();
            populator.Add<SecondClass>();
            populator.Bind();

            // Assert

            Assert.AreEqual(2, persistence.Storage.Count);
        }
    }
}