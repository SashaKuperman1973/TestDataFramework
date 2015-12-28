using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator;
using TestDataFramework.TypeGenerator;
using TestDataFramework.ValueGenerator;
using Tests.Mocks;

namespace Tests
{
    [TestClass]
    public class StandardPopulatorTests
    {
        [TestMethod]
        public void StandardPopulatorTest_Add()
        {
            // Arrange

            const string email = "name@domain.com";

            var typeGeneratorMock = new Mock<ITypeGenerator>();
            typeGeneratorMock.Setup(m => m.GetObject(It.Is<Type>(t => t == typeof(SubjectClass)))).Returns(new SubjectClass { AnEmailAddress = email});

            var populator = new StandardPopulator(typeGeneratorMock.Object, null);

            // Act

            RecordReference<SubjectClass> reference = populator.Add<SubjectClass>();

            // Assert

            Assert.IsNotNull(reference);

            Assert.AreEqual(email, reference.RecordObject.AnEmailAddress);
        }

        [TestMethod]
        public void StandardPopulatorTest_Bind()
        {
            // Arrange

            const int integer = 5;
            const string text = "abcde";

            var persistence = new MockPersistence();
            var typeGeneratorMock = new Mock<ITypeGenerator>();

            typeGeneratorMock.Setup(m => m.GetObject(It.Is<Type>(t => t == typeof(SubjectClass)))).Returns(new SubjectClass { Integer = integer, Text = text,});

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
            var typeGeneratorMock = new Mock<ITypeGenerator>();

            typeGeneratorMock.Setup(m => m.GetObject(It.Is<Type>(t => t == typeof(SubjectClass)))).Returns(new SubjectClass());
            typeGeneratorMock.Setup(m => m.GetObject(It.Is<Type>(t => t == typeof(SecondClass)))).Returns(new SecondClass());

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