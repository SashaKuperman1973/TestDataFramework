using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.UniqueValueGenerator;

namespace Tests.Tests
{
    [TestClass]
    public class StringGeneratorTest
    {
        [TestMethod]
        public void StringGenerator_Test()
        {
            const int stringLength = 10;

            string expected = "ABCD".PadLeft(stringLength, 'A');
            int input = (int)Math.Pow(26, 3) + (int)Math.Pow(26, 2) + 2*26 + 3;

            var generator = new StringGenerator();

            string result = generator.GetValue(input, stringLength);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void OverflowException_Test()
        {
            const string expected = "ABCD";
            int input = (int)Math.Pow(26, 3) + (int)Math.Pow(26, 2) + 2 * 26 + 3;

            var generator = new StringGenerator();

            Helpers.ExceptionTest(() => generator.GetValue(input, expected.Length - 1), typeof (OverflowException),
                string.Format(Messages.StringGeneratorOverflow, input, expected.Length - 1));
        }
    }
}
