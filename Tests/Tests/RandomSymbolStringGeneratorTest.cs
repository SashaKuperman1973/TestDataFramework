using System;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Helpers.Concrete;

namespace Tests.Tests
{
    [TestClass]
    public class RandomSymbolStringGeneratorTest
    {
        [TestMethod]
        public void RandomSymbolStringGenerator_Test()
        {
            XmlConfigurator.Configure();

            // Arrange

            var randomSymbolStringGenerator = new RandomSymbolStringGenerator(new Random());
            const int stringLength = 5;

            // Act

            string result = randomSymbolStringGenerator.GetRandomString(stringLength);

            // Assert

            Assert.AreEqual(stringLength, result.Length);

            foreach (char character in result)
            {
                Assert.IsTrue(character >= 'A' && character <= 'Z');
            }
        }
    }
}
