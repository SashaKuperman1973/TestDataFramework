using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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

        [TestInitialize]
        public void Initialize()
        {
            this.randomizerMock = new Mock<IRandomizer>();

            this.randomizerMock.Setup(m => m.RandomizeInteger()).Returns(StandardValueGeneratorTests.IntegerResult);
            this.randomizerMock.Setup(m => m.RandomizeLongInteger()).Returns(StandardValueGeneratorTests.LongResult);
            this.randomizerMock.Setup(m => m.RandomizeShortInteger()).Returns(StandardValueGeneratorTests.ShortResult);
            this.randomizerMock.Setup(m => m.RandomizeString(It.Is<int?>(length => length == null))).Returns(StandardValueGeneratorTests.StringResult);

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
        public void GetValue_Generates_UnknownValueGeneratorTypeException()
        {
            throw new NotImplementedException();
        }
    }
}
