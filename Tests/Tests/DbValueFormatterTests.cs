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
using TestDataFramework.ValueFormatter;

namespace Tests.Tests
{
    [TestClass]
    public class DbValueFormatterTests
    {
        private ValueFormatter formatter;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.formatter = new ValueFormatter();
        }

        [TestMethod]
        public void Null_Input_Test()
        {
            var result = this.formatter.Format(null);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Int_Test()
        {
            const int input = 5;
            var result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void Short_Test()
        {
            const short input = 5;
            var result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void Long_Test()
        {
            const long input = 5;
            var result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void UInt_Test()
        {
            const uint input = 5;
            var result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void UShort_Test()
        {
            const ushort input = 5;
            var result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void ULong_Test()
        {
            const ulong input = 5;
            var result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void String_Test()
        {
            const string input = "Abcd";
            var result = this.formatter.Format(input);
            Assert.AreEqual("'Abcd'", result);
        }

        [TestMethod]
        public void Char_Test()
        {
            const char input = 'A';
            var result = this.formatter.Format(input);
            Assert.AreEqual("'A'", result);
        }

        [TestMethod]
        public void Decimal_Test()
        {
            const decimal input = 12345.678m;
            var result = this.formatter.Format(input);
            Assert.AreEqual("12345.678", result);
        }

        [TestMethod]
        public void Bool_True_Test()
        {
            const bool input = true;
            var result = this.formatter.Format(input);
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void Bool_False_Test()
        {
            const bool input = false;
            var result = this.formatter.Format(input);
            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public void DateTime_Test()
        {
            DateTime input = DateTime.Parse("January 1, 2016 1:30 PM");
            var result = this.formatter.Format(input);
            Assert.AreEqual("'2016-01-01T13:30:00.000'", result);
        }

        [TestMethod]
        public void Byte_Test()
        {
            const byte input = (byte) 5;
            var result = this.formatter.Format(input);
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void Double_Test()
        {
            const double input = 5.489;
            var result = this.formatter.Format(input);
            Assert.AreEqual("5.489", result);
        }

        [TestMethod]
        public void Float_Test()
        {
            const float input = 5.587f;
            var result = this.formatter.Format(input);
            Assert.AreEqual("5.587", result);
        }

        [TestMethod]
        public void Guid_Test()
        {
            var input = new Guid("fe3fbecb-f023-48df-8b18-88b7c8282553");
            var result = this.formatter.Format(input);
            Assert.AreEqual("'fe3fbecb-f023-48df-8b18-88b7c8282553'", result);
        }

        private class ValueFormatter : DbValueFormatter
        {
        }
    }
}