using System;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;

namespace Tests.Tests
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
            const ulong input = 26*26 + 2*26 + 3;

            var generator = new LetterEncoder();

            string result = generator.Encode(input, stringLength);

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
            ulong input = (ulong)Math.Pow(26, 2) + 2 * 26 + 3;
            const string stringValueOfInput = "ABCD";

            var generator = new LetterEncoder();

            Helpers.ExceptionTest(() => generator.Encode(input, maxStringLength: stringValueOfInput.Length - 1), typeof (OverflowException),
                string.Format(Messages.StringGeneratorOverflow, input, stringValueOfInput.Length - 1));
        }
    }
}
