using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.Randomizer;
using TestDataFramework.ValueGenerator;

namespace Tests
{
    [TestClass]
    public class StandardValueGeneratorTests
    {
        private Mock<IRandomizer> randomizerMock;
        private StandardValueGenerator valueGenerator;

        private const int IntegerResult = 5;
        private const long LongResult = 6;
        private const short ShortResult = 7;
        private static readonly string StringResult = Guid.NewGuid().ToString("N");
        private const char CharacterResult = 'A';
        private const decimal DecimalResult = 48576.412587m;

        [TestInitialize]
        public void Initialize()
        {
            this.randomizerMock = new Mock<IRandomizer>();

            this.randomizerMock.Setup(m => m.RandomizeInteger()).Returns(StandardValueGeneratorTests.IntegerResult);
            this.randomizerMock.Setup(m => m.RandomizeLongInteger()).Returns(StandardValueGeneratorTests.LongResult);
            this.randomizerMock.Setup(m => m.RandomizeShortInteger()).Returns(StandardValueGeneratorTests.ShortResult);
            this.randomizerMock.Setup(m => m.RandomizeString(It.Is<int?>(length => length == null))).Returns(StandardValueGeneratorTests.StringResult);
            this.randomizerMock.Setup(m => m.RandomizeCharacter()).Returns(StandardValueGeneratorTests.CharacterResult);
            this.randomizerMock.Setup(m => m.RandomizeDecimal()).Returns(StandardValueGeneratorTests.DecimalResult);

            this.valueGenerator = new StandardValueGenerator(this.randomizerMock.Object);
        }

        [TestMethod]
        public void AllTypeTests()
        {
            var list = new List<Tuple<string, object>>
            {
                new Tuple<string, object>("Integer", StandardValueGeneratorTests.IntegerResult),
                new Tuple<string, object>("LongInteger", StandardValueGeneratorTests.LongResult),
                new Tuple<string, object>("ShortInteger", StandardValueGeneratorTests.ShortResult),
                new Tuple<string, object>("Text", StandardValueGeneratorTests.StringResult),
                new Tuple<string, object>("Character", StandardValueGeneratorTests.CharacterResult),
                new Tuple<string, object>("Decimal", StandardValueGeneratorTests.DecimalResult),
            };

            list.ForEach(type => this.TypeTest(type.Item1, type.Item2));
        }

        private void TypeTest(string propertyName, object expectedResult)
        {
            Console.WriteLine("Executing for " + propertyName);

            // Arrange

            PropertyInfo propertyInfo = typeof (SubjectClass).GetProperty(propertyName);

            // Act

            object result = this.valueGenerator.GetValue(propertyInfo);

            // Assert

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetValue_StringLengthAttribute_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("TextWithLength");

            string expected = Guid.NewGuid().ToString("N");

            this.randomizerMock.Setup(m => m.RandomizeString(It.Is<int?>(length => length == 10))).Returns(expected).Verifiable();

            // Act

            object result = this.valueGenerator.GetValue(propertyInfo);

            // Assert

            this.randomizerMock.Verify();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetValue_Generates_UnknownValueGeneratorTypeException()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("UnresolvableTypeMember");

            // Act

            UnknownValueGeneratorTypeException exception = null;

            try
            {
                this.valueGenerator.GetValue(propertyInfo);
            }
            catch (UnknownValueGeneratorTypeException ex)
            {
                exception = ex;
            }
            
            Assert.IsNotNull(exception);
            Assert.AreEqual("Cannot resolve a value generator for type: " + typeof (UnresolvableType), exception.Message);
        }
    }
}
