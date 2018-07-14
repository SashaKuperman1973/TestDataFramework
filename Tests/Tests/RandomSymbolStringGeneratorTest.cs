/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

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
                Assert.IsTrue(character >= 'A' && character <= 'Z');
        }
    }
}