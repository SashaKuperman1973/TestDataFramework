using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.UniqueValueGenerator;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardUniqueValueGeneratorTest
    {
        [TestMethod]
        public void String_Test()
        {
            // Arrange

            var stringGeneratorMock = new Mock<StringGenerator>();
            var generator = new StandardUniqueValueGenerator(stringGeneratorMock.Object);

            const string expected = "ABCD";

            stringGeneratorMock.Setup(m => m.GetValue(0, ClassWithStringAutoPrimaryKey.StringLength))
                .Returns(expected)
                .Verifiable();

            // Act

            object value = generator.GetValue(typeof (ClassWithStringAutoPrimaryKey).GetProperty("Key"));

            // Assert

            stringGeneratorMock.Verify();
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        public void Integer_Test()
        {
            StandardUniqueValueGeneratorTest.IntegerTest(typeof(ClassWithIntAutoPrimaryKey));
            StandardUniqueValueGeneratorTest.IntegerTest(typeof(ClassWithShortAutoPrimaryKey));
            StandardUniqueValueGeneratorTest.IntegerTest(typeof(ClassWithLongAutoPrimaryKey));
        }

        private static void IntegerTest(Type inputType)
        {
            var generator = new StandardUniqueValueGenerator(null);

            PropertyInfo keyPropertyInfo = inputType.GetProperty("Key");

            // Act. Assert.

            object value;

            value = generator.GetValue(keyPropertyInfo);
            StandardUniqueValueGeneratorTest.AreEqual(0, value);

            value = generator.GetValue(keyPropertyInfo);
            StandardUniqueValueGeneratorTest.AreEqual(1, value);

            value = generator.GetValue(keyPropertyInfo);
            StandardUniqueValueGeneratorTest.AreEqual(2, value);
        }

        private static void AreEqual(object expected, object actual)
        {
            Assert.AreEqual(Convert.ChangeType(expected, actual.GetType()), actual);
        }
    }
}
