using System;
using System.Runtime.CompilerServices;
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

        [TestMethod]
        public void RandomizeLongInteger_Word0Max_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(0x10000)).Returns(StandardRandomizerTests.Integer).Verifiable();

            // Act

            long result = this.randomizer.RandomizeLongInteger(0x10000);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual((long)StandardRandomizerTests.Integer, result);
        }


        [TestMethod]
        public void RandomizeLongInteger_Test()
        {
            // Arrange

            long maxValue = long.MaxValue - new Random().Next() - 1;
            long expected = maxValue - new Random().Next();

            if ((maxValue & 0xffff) == 0)
            {
                maxValue--;
            }

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 0)) & 0xffff)))
                .Returns((int)((expected >> (16 * 0)) & 0xffff)).Verifiable();

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 1)) & 0xffff) + 1))
                .Returns((int)((expected >> (16 * 1)) & 0xffff)).Verifiable();

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 2)) & 0xffff) + 1))
                .Returns((int)((expected >> (16 * 2)) & 0xffff)).Verifiable();

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 3)) & 0xffff) + 1))
                .Returns((int)((expected >> (16 * 3)) & 0xffff)).Verifiable();

            // Act

            long result = this.randomizer.RandomizeLongInteger(maxValue);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RandomizeLongInteger_DefaultMax_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(1);

            // Act

            long result = this.randomizer.RandomizeLongInteger(null);

            // Assert

            this.randomMock.Verify(m => m.Next(0xffff), Times.Once);
            this.randomMock.Verify(m => m.Next(0x10000), Times.Exactly(2));
            this.randomMock.Verify(m => m.Next(0x8000), Times.Once);

            const long expected = 0x0001000100010001;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RandomizeShortInteger_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.Is<int>(i => i == 7))).Returns(StandardRandomizerTests.Integer).Verifiable();

            // Act

            short result = this.randomizer.RandomizeShortInteger(7);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual((short)StandardRandomizerTests.Integer, result);
        }

        [TestMethod]
        public void RandomizeShortInteger_DefaultMax_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.Is<int>(i => i == short.MaxValue))).Returns(StandardRandomizerTests.Integer).Verifiable();

            // Act

            short result = this.randomizer.RandomizeShortInteger(null);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual((short)StandardRandomizerTests.Integer, result);
        }
    }
}
