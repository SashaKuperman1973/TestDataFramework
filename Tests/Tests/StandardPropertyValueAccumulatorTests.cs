using System;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardPropertyValueAccumulatorTests
    {
        private StandardPropertyValueAccumulator accumulator;
        private Mock<LetterEncoder> stringGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.stringGeneratorMock = new Mock<LetterEncoder>();
            this.accumulator = new StandardPropertyValueAccumulator(this.stringGeneratorMock.Object);
        }

        [TestMethod]
        public void IntegerTest()
        {
            this.IntegerTest(typeof(ClassWithIntAutoPrimaryKey));
            this.IntegerTest(typeof(ClassWithShortAutoPrimaryKey));
            this.IntegerTest(typeof(ClassWithLongAutoPrimaryKey));
            this.IntegerTest(typeof(ClassWithByteAutoPrimaryKey));
        }

        private void IntegerTest(Type inputClass)
        {
            object[] resultArray = this.Test(inputClass);

            StandardPropertyValueAccumulatorTests.AreEqual(5, resultArray[0]);
            StandardPropertyValueAccumulatorTests.AreEqual(6, resultArray[1]);
        }

        [TestMethod]
        public void StringTest()
        {
            this.stringGeneratorMock.Setup(m => m.Encode(5, It.IsAny<int>())).Returns("A");
            this.stringGeneratorMock.Setup(m => m.Encode(6, It.IsAny<int>())).Returns("B");

            object[] resultArray = this.Test(typeof(ClassWithStringAutoPrimaryKey));

            StandardPropertyValueAccumulatorTests.AreEqual("A", resultArray[0]);
            StandardPropertyValueAccumulatorTests.AreEqual("B", resultArray[1]);
        }

        private object[] Test(Type inputClass)
        {
            PropertyInfo keyPropertyInfo = inputClass.GetProperty("Key");

            var resultArray = new object[2];

            // Act

            resultArray[0] = this.accumulator.GetValue(keyPropertyInfo, 5);
            resultArray[1] = this.accumulator.GetValue(keyPropertyInfo, 5);

            return resultArray;
        }

        private static void AreEqual(object expected, object actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.AreEqual(Convert.ChangeType(expected, actual.GetType()), actual);
            }

        }
    }
}
