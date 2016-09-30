/*
    Copyright 2016 Alexander Kuperman

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.ValueFormatter.Concrete;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class SqlClientValueFormatterTests
    {
        private SqlClientValueFormatter formatter;

        [TestInitialize]
        public void Initialize()
        {
            this.formatter = new SqlClientValueFormatter();
        }

        [TestMethod]
        public void Format_Variable_Test()
        {
            // Arrange

            var input = new Variable("ABCD");

            // Act

            object result = this.formatter.Format(input);

            // Assert

            Assert.AreEqual("@ABCD", result);
        }

        private class NotHandledType
        {
        }

        [TestMethod]
        public void Format_NotHanldedType_Test()
        {
            // Arrange

            var unhandled = new NotHandledType();

            // Act. Assert.

            Helpers.ExceptionTest(() => this.formatter.Format(unhandled), typeof (NotSupportedException),
                string.Format(TestDataFramework.Exceptions.Messages.InsertionDoesNotSupportType, unhandled.GetType(), unhandled));
        }

        [TestMethod]
        public void Format_HanldedType_Test()
        {
            // Act

            object result = this.formatter.Format(5);

            // Assert

            Assert.AreEqual("5", result);
        }
    }
}
