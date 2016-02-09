using System;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.ValueFormatter;

namespace Tests.Tests
{
    [TestClass]
    public class DbValueFormatterTests
    {
        private ValueFormatter formatter;

        private class ValueFormatter : DbValueFormatter
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.formatter = new ValueFormatter();
        }

        [TestMethod]
        public void Int_Test()
        {
            const int input = 5;
            string result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void Short_Test()
        {
            const short input = 5;
            string result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void Long_Test()
        {
            const long input = 5;
            string result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void UInt_Test()
        {
            const uint input = 5;
            string result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void UShort_Test()
        {
            const ushort input = 5;
            string result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void ULong_Test()
        {
            const ulong input = 5;
            string result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void String_Test()
        {
            const string input = "Abcd";
            string result = this.formatter.Format(input);
            Assert.AreEqual("'Abcd'", result);
        }

        [TestMethod]
        public void Char_Test()
        {
            const char input = 'A';
            string result = this.formatter.Format(input);
            Assert.AreEqual("'A'", result);
        }

        [TestMethod]
        public void Decimal_Test()
        {
            const decimal input = 12345.678m;
            string result = this.formatter.Format(input);
            Assert.AreEqual("12345.678", result);
        }

        [TestMethod]
        public void Bool_True_Test()
        {
            const bool input = true;
            string result = this.formatter.Format(input);
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void Bool_False_Test()
        {
            const bool input = false;
            string result = this.formatter.Format(input);
            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public void DateTime_Test()
        {
            DateTime input = DateTime.Parse("January 1, 2016 1:30 PM");
            string result = this.formatter.Format(input);
            Assert.AreEqual("'2016-01-01T13:30:00'", result);
        }

        [TestMethod]
        public void Byte_Test()
        {
            const byte input = (byte) 5;
            string result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }
    }
}
