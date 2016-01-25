using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.WritePrimitives;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class SqlWriterDictionaryTests
    {
        private SqlWriterDictionary writerDictionary;
        private Mock<LetterEncoder> encoderMock;
        private Mock<IWritePrimitives> primitivesMock;
        private Mock<SqlWriterCommandTextGenerator> commandGenerator;

        [TestInitialize]
        public void Initialize()
        {
            this.encoderMock = new Mock<LetterEncoder>();
            this.primitivesMock = new Mock<IWritePrimitives>();
            this.commandGenerator = new Mock<SqlWriterCommandTextGenerator>();

            this.writerDictionary = new SqlWriterDictionary(this.encoderMock.Object, this.primitivesMock.Object,
                this.commandGenerator.Object);
        }
        /*
        private void Test(object value, ulong expected, Func<PropertyInfo, string> commandTextFunc)
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(ClassWithStringAutoPrimaryKey).GetProperty("Key");

            int readCount = 0;

            // Act

            ulong result = this.generator.FillData(propertyInfo)

                    // Assert

            this.commandMock.VerifySet(m => m.CommandText = commandTextFunc(propertyInfo));
            this.readerMock.Verify(m => m.Read());
            this.readerMock.Verify(m => m.GetValue(0));

            Assert.AreEqual(expected, result);
        }
        */
        [TestMethod]
        public void Number_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(ClassWithIntAutoPrimaryKey).GetProperty("Key");

            const string command = "XXXX";
            this.commandGenerator.Setup(m => m.WriteNumber(propertyInfo)).Returns(command);

            var values = new object[] { (byte)1, (int)2, (short)3, (long)4 };

            foreach (object value in values)
            {
                // Act

                WriterDelegate numberWriter = this.writerDictionary[value.GetType()];
                DecoderDelegate numberDecoder = numberWriter(propertyInfo);
                ulong result = numberDecoder(propertyInfo, value);

                // Assert

                Assert.AreEqual((ulong)Convert.ChangeType(value, typeof(ulong)), result);
                this.primitivesMock.Verify(m => m.AddSqlCommand(command));
            }
        }

        [TestMethod]
        public void String_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(ClassWithStringAutoPrimaryKey).GetProperty("Key");
            const string value = "X";
            const ulong expected = 7;

            const string command = "XXXX";
            this.commandGenerator.Setup(m => m.WriteString(propertyInfo)).Returns(command);

            this.encoderMock.Setup(m => m.Decode(value)).Returns(expected);

            // Act

            WriterDelegate stringWriter = this.writerDictionary[value.GetType()];
            DecoderDelegate stringDecoder = stringWriter(propertyInfo);
            ulong result = stringDecoder(propertyInfo, value);

            // Assert

            Assert.AreEqual(expected, result);
            this.commandGenerator.Verify(m => m.WriteString(propertyInfo), Times.Once);
            this.primitivesMock.Verify(m => m.AddSqlCommand(command));
        }

        [TestMethod]
        public void Execute_Test()
        {
            // Act

            this.writerDictionary.Execute();

            // Assert

            this.primitivesMock.Verify(m => m.Execute(), Times.Once);
        }

        [TestMethod]
        public void NotANumberException_Test()
        {
            PropertyInfo propertyInfo = typeof(ClassWithIntAutoPrimaryKey).GetProperty("Key");
            const string input = "ABC";

            WriterDelegate stringWriter = this.writerDictionary[typeof(int)];
            DecoderDelegate decoder = stringWriter(propertyInfo);

            Helpers.ExceptionTest(() => decoder(propertyInfo, input), typeof(UnexpectedTypeException),
                string.Format(Messages.UnexpectedHandlerType, propertyInfo, input));
        }

        [TestMethod]
        public void NotAStringException_Test()
        {
            PropertyInfo propertyInfo = typeof(ClassWithStringAutoPrimaryKey).GetProperty("Key");
            const int input = 5;

            WriterDelegate stringWriter = this.writerDictionary[typeof(string)];
            DecoderDelegate decoder = stringWriter(propertyInfo);

            Helpers.ExceptionTest(() => decoder(propertyInfo, input), typeof (UnexpectedTypeException),
                string.Format(Messages.UnexpectedHandlerType, propertyInfo, input));
        }

        /*
                        [TestMethod]
                        public void StringHandler_Test()
                        {
                            Func<PropertyInfo, string> commandTextFunc =
                                propertyInfo =>
                                    $"Select MAX([{propertyInfo.Name}]) From [{propertyInfo.DeclaringType.Name}] Where [{propertyInfo.Name}] like '[A-Z]%'";

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

                        [TestMethod]
                        public void NumberHandler_DbNull_Test()
                        {
                            Func<PropertyInfo, string> commandTextFunc =
                                propertyInfo => $"Select MAX([{propertyInfo.Name}]) From [{propertyInfo.DeclaringType.Name}]";

                            this.Test(DBNull.Value, Helper.DefaultInitalCount, this.handler.NumberHandler, commandTextFunc);
                        }

                        [TestMethod]
                        public void StringHandler_DbNull_Test()
                        {
                            Func<PropertyInfo, string> commandTextFunc =
                                propertyInfo =>
                                    $"Select MAX([{propertyInfo.Name}]) From [{propertyInfo.DeclaringType.Name}] Where [{propertyInfo.Name}] like '[A-Z]%'";

                            this.Test(DBNull.Value, Helper.DefaultInitalCount, this.handler.StringHandler, commandTextFunc);
                        }
                */
    }
}
