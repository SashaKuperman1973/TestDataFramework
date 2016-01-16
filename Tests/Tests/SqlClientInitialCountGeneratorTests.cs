using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.WritePrimitives;
using Tests.Mocks;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class SqlClientInitialCountGeneratorTests
    {
        private SqlClientInitialCountGenerator generator;
        private Mock<IWriterDictinary> writersMock;
        private Mock<LetterEncoder> encoderMock;

        [TestInitialize]
        public void Initialize()
        {
            this.writersMock = new Mock<IWriterDictinary>();
            this.encoderMock = new Mock<LetterEncoder>();
            this.generator = new SqlClientInitialCountGenerator(this.writersMock.Object);
        }

        [TestMethod]
        public void FillData_Test()
        {
            // Arrange

            const ulong expectedValue1 = 7;
            const ulong expectedValue2 = 14;

            PropertyInfo propertyInfo1 = typeof(ClassWithStringAutoPrimaryKey).GetProperty("Key");
            PropertyInfo propertyInfo2 = typeof(ClassWithIntAutoPrimaryKey).GetProperty("Key");

            var data1 = new Data<ulong>(null);
            var data2 = new Data<ulong>(null);
            var dictionary = new Dictionary<PropertyInfo, Data<ulong>>
            {
                {propertyInfo1, data1},
                {propertyInfo2, data2},
            };

            WriterDelegate writer1 = writerPi => (decoderPi, input) => expectedValue1;
            WriterDelegate writer2 = writerPi => (decoderPi, input) => expectedValue2;

            this.writersMock.Setup(m => m[typeof(string)]).Returns(writer1);
            this.writersMock.Setup(m => m[typeof(int)]).Returns(writer2);

            this.writersMock.Setup(m => m.Execute()).Returns(new object[2]);

            // Act

            this.generator.FillData(dictionary);

            // Assert

            Assert.AreEqual(expectedValue1, data1.Item);
            Assert.AreEqual(expectedValue2, data2.Item);
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

                [TestMethod]
                public void NumberHandler_Test()
                {
                    var values = new object[] {(byte) 1, (int) 2, (short) 3, (long) 4};

                    foreach (object value in values)
                    {
                        this.Test(value, (ulong)Convert.ChangeType(value, typeof(ulong)));
                    }
                }

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
