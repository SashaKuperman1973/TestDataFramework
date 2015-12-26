using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Helpers;
using TestDataFramework.Helpers.Concrete;

namespace Tests
{
    [TestClass]
    public class RandomSymbolStringGeneratorTest
    {
        [TestMethod]
        public void RandomSymbolStringGenerator_Test()
        {
            // Arrange

            var randomSymbolStringGenerator = new RandomSymbolStringGenerator(new Random());

            // Act

            string result = randomSymbolStringGenerator.GetRandomString(5);

            // Assert

            Assert.AreEqual(5, result.Length);

            foreach (char character in result)
            {
                Assert.IsTrue(character >= 'A' && character <= 'Z');
            }
        }
    }
}
