using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Randomizer;

namespace Tests
{
    [TestClass]
    public class StandardRandomizerTests
    {
        private StandardRandomizer randomizer;
        private Mock<Random> randomMock;

        private const int Integer = 5;

        [TestInitialize]
        public void Initialize()
        {
            this.randomMock = new Mock<Random>();
            this.randomizer = new StandardRandomizer(this.randomMock.Object);
        }

        [TestMethod]
        public void RandomizeInteger_Test()
        {

            // Arrange

            this.randomMock.Setup(m => m.Next()).Returns(StandardRandomizerTests.Integer);

            // Act

            int result = this.randomizer.RandomizeInteger(null);

            // Assert

            Assert.AreEqual(StandardRandomizerTests.Integer, result);
            this.randomMock.Verify(m => m.Next(), Times.Once());
            this.randomMock.Verify(m => m.Next(It.IsAny<int>()), Times.Never());
        }

        [TestMethod]
        public void RandomizeInteger_WithMax_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(StandardRandomizerTests.Integer);

            // Act

            int result = this.randomizer.RandomizeInteger(7);

            // Assert

            Assert.AreEqual(StandardRandomizerTests.Integer, result);
            this.randomMock.Verify(m => m.Next(), Times.Never());
            this.randomMock.Verify(m => m.Next(It.Is<int>(max => max == 7)), Times.Once());
        }
    }
}
