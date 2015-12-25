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

            this.randomMock.Setup(m => m.Next()).Returns(StandardRandomizerTests.Integer);
        }

        [TestMethod]
        public void RandomizeInteger_Test()
        {
            // Act

            int result = this.randomizer.RandomizeInteger();

            // Assert

            Assert.AreEqual(StandardRandomizerTests.Integer, result);
        }
    }
}
