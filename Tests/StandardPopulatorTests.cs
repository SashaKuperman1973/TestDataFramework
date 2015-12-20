using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator;
using TestDataFramework.Randomizer;
using TestDataFramework.ValueGenerator;
using Tests.Mocks;

namespace Tests
{
    [TestClass]
    public class StandardPopulatorTests
    {
        [TestMethod]
        public void StandardPopulatorTest()
        {
            // Arrange

            const int integer = 5;
            const string text = "abcde";

            var persistence = new MockPersistence();
            var valueGeneratorMock = new Mock<IValueGenerator>();

            valueGeneratorMock.Setup(m => m.GetValue(It.Is<Type>(t => t == typeof(int)))).Returns(integer);
            valueGeneratorMock.Setup(m => m.GetValue(It.Is<Type>(t => t == typeof(string)))).Returns(text);

            var populator = new StandardPopulator(valueGeneratorMock.Object, persistence);

            // Act

            populator.Add<SubjectClass>();
            populator.Populate();

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
            var populator = new StandardPopulator(new Mock<IValueGenerator>().Object, persistence);

            // Act

            populator.Add<SubjectClass>();
            populator.Add<SecondClass>();
            populator.Populate();

            // Assert

            Assert.AreEqual(2, persistence.Storage.Count);
        }
    }
}