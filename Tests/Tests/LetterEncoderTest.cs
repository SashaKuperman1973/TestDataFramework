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
            ulong input = (ulong) 26*26 + 2*26 + 3;

            var generator = new LetterEncoder();

            string result = generator.Encode(input, stringLength);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Decode_Test()
        {
            const ulong expected = 5204;

            var generator = new LetterEncoder();

            ulong result = generator.Decode("HSE");

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void OverflowException_Test()
        {
            const string expected = "ABCD";
            ulong input = (ulong)Math.Pow(26, 2) + 2 * 26 + 3;

            var generator = new LetterEncoder();

            Helpers.ExceptionTest(() => generator.Encode(input, expected.Length - 1), typeof (OverflowException),
                string.Format(Messages.StringGeneratorOverflow, input, expected.Length - 1));
        }
    }
}
