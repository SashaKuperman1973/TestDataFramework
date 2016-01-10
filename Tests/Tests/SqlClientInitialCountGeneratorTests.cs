using System;
using System.Data.Common;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using Tests.Mocks;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class SqlClientInitialCountGeneratorTests
    {
        private SqlClientInitialCountGenerator handler;

        private MockDbCommand mockDbCommand;
        private Mock<DbCommand> commandMock;
        private Mock<DbDataReader> readerMock;

        [TestInitialize]
        public void Initialize()
        {
            this.commandMock = new Mock<DbCommand>();
            this.readerMock = new Mock<DbDataReader>();
            this.mockDbCommand = new MockDbCommand(this.commandMock.Object, this.readerMock.Object);

            this.handler = new SqlClientInitialCountGenerator();
        }

        [TestMethod]
        public void NumberHandler_Test()
        {
            var values = new object[] {(byte) 1, (int) 2, (short) 3, (long) 4};

            Func<PropertyInfo, string> commandTextFunc =
                propertyInfo => $"Select MAX([{propertyInfo.Name}]) From [{propertyInfo.DeclaringType.Name}]";

            foreach (object value in values)
            {
                this.Test(value, (ulong)Convert.ChangeType(value, typeof(ulong)), this.handler.NumberHandler, commandTextFunc);
            }
        }

        [TestMethod]
        public void StringHandler_Test()
        {
            Func<PropertyInfo, string> commandTextFunc =
                propertyInfo =>
                    $"Select MAX([{propertyInfo.Name}]) From [{propertyInfo.DeclaringType.Name}] Where MAX([{propertyInfo.Name}]) like '[A-Z]%'";

            this.Test("ABC", 28ul, this.handler.StringHandler, commandTextFunc);
        }

        [TestMethod]
        public void NumberHandler_UnexpectedTypeException_Test()
        {
            Helpers.ExceptionTest(() => this.Test("A", 0, this.handler.NumberHandler, null),
                typeof(UnexpectedTypeException),
                string.Format(Messages.UnexpectedHandlerType, "System.String Key", "System.String"));
        }

        [TestMethod]
        public void StringHandler_UnexpectedTypeException_Test()
        {
            Helpers.ExceptionTest(() => this.Test(1, 0, this.handler.StringHandler, null),
                typeof(UnexpectedTypeException),
                string.Format(Messages.UnexpectedHandlerType, "System.String Key", "System.Int32"));
        }

        private void Test(object value, ulong expected, HandlerDelegate<ulong> handlerDelegate, Func<PropertyInfo, string> commandTextFunc)
        {
            // Arrange

            PropertyInfo propertyInfo = typeof (ClassWithStringAutoPrimaryKey).GetProperty("Key");

            int readCount = 0;
            this.readerMock.Setup(m => m.Read()).Returns(() => new[] {true, false}[readCount++]);
            this.readerMock.Setup(m => m.GetValue(0)).Returns(value);

            // Act

            ulong result = handlerDelegate(propertyInfo, this.mockDbCommand);

            // Assert

            this.commandMock.VerifySet(m => m.CommandText = commandTextFunc(propertyInfo));
            this.readerMock.Verify(m => m.Read());
            this.readerMock.Verify(m => m.GetValue(0));

            Assert.AreEqual(expected, result);
        }
    }
}
