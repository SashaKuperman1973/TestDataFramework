using System;
using System.CodeDom;
using System.Collections.Generic;
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

        [TestInitialize]
        public void Initialize()
        {
            this.randomizerMock = new Mock<IRandomizer>();

            this.randomizerMock.Setup(m => m.RandomizeInteger()).Returns(StandardValueGeneratorTests.IntegerResult);
            this.randomizerMock.Setup(m => m.RandomizeLongInteger()).Returns(StandardValueGeneratorTests.LongResult);
            this.randomizerMock.Setup(m => m.RandomizeShortInteger()).Returns(StandardValueGeneratorTests.ShortResult);

            this.valueGenerator = new StandardValueGenerator(this.randomizerMock.Object);
        }

        [TestMethod]
        public void AllTypeTests()
        {
            var list = new List<Tuple<Type, object>>
            {
                new Tuple<Type, object>(typeof (int), StandardValueGeneratorTests.IntegerResult),
                new Tuple<Type, object>(typeof(long), StandardValueGeneratorTests.LongResult),
                new Tuple<Type, object>(typeof(short), StandardValueGeneratorTests.ShortResult),
            };

            list.ForEach(type => this.TypeTest(type.Item1, type.Item2));
        }

        private void TypeTest(Type forType, object expectedResult)
        {
            Console.WriteLine("Executing for " + forType);

            // Act

            object result = this.valueGenerator.GetValue(forType);

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
