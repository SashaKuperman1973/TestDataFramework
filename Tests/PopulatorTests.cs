using System;
using System.CodeDom;
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
    public class PopulatorTests
    {
        [TestMethod]
        public void StandardPopulatorTest()
        {
            // Arrange

            const int integer = 5;

            var persistence = new MockPersistence();

            var valueGeneratorMock = new Mock<IValueGenerator>();
            valueGeneratorMock.Setup(m => m.GetValue(It.Is<Type>(t => t == typeof(int)))).Returns(integer);

            var populator = new StandardPopulator(valueGeneratorMock.Object, persistence);

            // Act

            populator.Add<SubjectClass>();

            populator.Populate();

            // Assert

            Assert.AreEqual(integer, persistence.Persisted);
        }
    }
}