using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
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

            var returnValue1 = new LargeInteger(7);
            var returnValue2 = new LargeInteger(14);

            PropertyInfo propertyInfo1 = typeof(ClassWithStringAutoPrimaryKey).GetProperty("Key");
            PropertyInfo propertyInfo2 = typeof(ClassWithIntAutoPrimaryKey).GetProperty("Key");

            var data1 = new Data<LargeInteger>(null);
            var data2 = new Data<LargeInteger>(null);
            var dictionary = new Dictionary<PropertyInfo, Data<LargeInteger>>
            {
                {propertyInfo1, data1},
                {propertyInfo2, data2},
            };

            WriterDelegate writer1 = writerPi => (decoderPi, input) => returnValue1;
            WriterDelegate writer2 = writerPi => (decoderPi, input) => returnValue2;

            this.writersMock.Setup(m => m[typeof(string)]).Returns(writer1);
            this.writersMock.Setup(m => m[typeof(int)]).Returns(writer2);

            this.writersMock.Setup(m => m.Execute()).Returns(new object[2]);

            // Act

            this.generator.FillData(dictionary);

            // Assert

            Assert.AreEqual(returnValue1 + 1, data1.Item);
            Assert.AreEqual(returnValue2 + 1, data2.Item);
        }
    }
}
