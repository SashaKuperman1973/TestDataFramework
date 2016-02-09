using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.ValueFormatter.Concrete;

namespace Tests.Tests
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
