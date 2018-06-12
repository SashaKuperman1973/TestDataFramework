/*
    Copyright 2016, 2017 Alexander Kuperman

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
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class LetterEncoderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();
        }

        [TestMethod]
        public void Encode_Test()
        {
            const int stringLength = 10;

            const string expected = "BCD";
            const ulong input = 26 * 26 + 2 * 26 + 3;

            var generator = new LetterEncoder();

            var result = generator.Encode(input, stringLength);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Decode_Test()
        {
            var expected = new LargeInteger(5204);

            var generator = new LetterEncoder();

            LargeInteger result = generator.Decode("HSE");

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void OverflowException_Test()
        {
            var input = (ulong) Math.Pow(26, 2) + 2 * 26 + 3;
            const string stringValueOfInput = "ABCD";

            var generator = new LetterEncoder();

            Helpers.ExceptionTest(() => generator.Encode(input, stringValueOfInput.Length - 1),
                typeof(OverflowException),
                string.Format(Messages.StringGeneratorOverflow, input, stringValueOfInput.Length - 1));
        }
    }
}