using System;
using System.Collections.Generic;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.Exceptions;
using TestDataFramework.TypeGenerator;
using TestDataFramework.UniqueValueGenerator;
using TestDataFramework.UniqueValueGenerator.Interface;
using TestDataFramework.ValueGenerator;
using TestDataFramework.ValueProvider;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardValueGeneratorTests
    {
        private Mock<IValueProvider> randomizerMock;
        private Mock<ITypeGenerator> typeGeneratorMock;
        private Mock<IArrayRandomizer> arrayRandomizerMock;
        private StandardValueGenerator valueGenerator;
        private Mock<IUniqueValueGenerator> uniqueValueGeneratorMock = new Mock<IUniqueValueGenerator>();

        private const int IntegerResult = 5;
        private const long LongResult = 6;
        private const short ShortResult = 7;
        private static readonly string StringResult = Guid.NewGuid().ToString("N");
        private const char CharacterResult = 'A';
        private const decimal DecimalResult = 48576.412587m;
        private const bool BooleanResult = true;
        private static readonly DateTime DateTimeResult = DateTime.Now;
        private const byte ByteResult = 8;
        private const double DoubleResult = 574.1575d;
        private const string EmailAddress = "address@domain.com";

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.randomizerMock = new Mock<IValueProvider>();
            this.typeGeneratorMock = new Mock<ITypeGenerator>();
            this.arrayRandomizerMock = new Mock<IArrayRandomizer>();
            this.uniqueValueGeneratorMock = new Mock<IUniqueValueGenerator>();

            this.randomizerMock.Setup(m => m.GetInteger(It.Is<int?>(max => max == null))).Returns(StandardValueGeneratorTests.IntegerResult);
            this.randomizerMock.Setup(m => m.GetLongInteger(It.Is<long?>(max => max == null))).Returns(StandardValueGeneratorTests.LongResult);
            this.randomizerMock.Setup(m => m.GetShortInteger(It.Is<short?>(max => max == null))).Returns(StandardValueGeneratorTests.ShortResult);
            this.randomizerMock.Setup(m => m.GetString(It.Is<int?>(length => length == null))).Returns(StandardValueGeneratorTests.StringResult);
            this.randomizerMock.Setup(m => m.GetCharacter()).Returns(StandardValueGeneratorTests.CharacterResult);
            this.randomizerMock.Setup(m => m.GetDecimal(It.Is<int?>(precision => precision == null))).Returns(StandardValueGeneratorTests.DecimalResult);
            this.randomizerMock.Setup(m => m.GetBoolean()).Returns(StandardValueGeneratorTests.BooleanResult);
            this.randomizerMock.Setup(
                m =>
                    m.GetDateTime(It.Is<PastOrFuture?>(pastOrFuture => pastOrFuture == null),
                        It.Is<Func<long?, long>>(lir => lir == this.randomizerMock.Object.GetLongInteger)))
                .Returns(StandardValueGeneratorTests.DateTimeResult);

            this.randomizerMock.Setup(m => m.GetByte()).Returns(StandardValueGeneratorTests.ByteResult);
            this.randomizerMock.Setup(m => m.GetDouble(It.Is<int?>(precision => precision == null))).Returns(StandardValueGeneratorTests.DoubleResult);
            this.randomizerMock.Setup(m => m.GetEmailAddress()).Returns(StandardValueGeneratorTests.EmailAddress);

            this.valueGenerator = new StandardValueGenerator(this.randomizerMock.Object, () => this.typeGeneratorMock.Object, () => this.arrayRandomizerMock.Object, this.uniqueValueGeneratorMock.Object);
        }

        [TestMethod]
        public void AllTypeTests()
        {
            var list = new List<Tuple<string, object>>
            {
                new Tuple<string, object>("Integer", StandardValueGeneratorTests.IntegerResult),
                new Tuple<string, object>("UnsignedInteger", StandardValueGeneratorTests.IntegerResult),
                new Tuple<string, object>("LongInteger", StandardValueGeneratorTests.LongResult),
                new Tuple<string, object>("UnsignedLongInteger", StandardValueGeneratorTests.LongResult),
                new Tuple<string, object>("ShortInteger", StandardValueGeneratorTests.ShortResult),
                new Tuple<string, object>("UnsignedShortInteger", StandardValueGeneratorTests.ShortResult),
                new Tuple<string, object>("Text", StandardValueGeneratorTests.StringResult),
                new Tuple<string, object>("Character", StandardValueGeneratorTests.CharacterResult),
                new Tuple<string, object>("Decimal", StandardValueGeneratorTests.DecimalResult),
                new Tuple<string, object>("Boolean", StandardValueGeneratorTests.BooleanResult),
                new Tuple<string, object>("DateTime", StandardValueGeneratorTests.DateTimeResult),
                new Tuple<string, object>("Byte", StandardValueGeneratorTests.ByteResult),
                new Tuple<string, object>("Double", StandardValueGeneratorTests.DoubleResult),
                new Tuple<string, object>("NullableInteger", StandardValueGeneratorTests.IntegerResult),
                new Tuple<string, object>("UnsignedNullableInteger", StandardValueGeneratorTests.IntegerResult),
                new Tuple<string, object>("NullableLong", StandardValueGeneratorTests.LongResult),
                new Tuple<string, object>("UnsignedNullableLong", StandardValueGeneratorTests.LongResult),
                new Tuple<string, object>("NullableShort", StandardValueGeneratorTests.ShortResult),
                new Tuple<string, object>("UnsignedNullableShort", StandardValueGeneratorTests.ShortResult),
                new Tuple<string, object>("AnEmailAddress", StandardValueGeneratorTests.EmailAddress),
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

            this.randomizerMock.Setup(m => m.GetString(It.Is<int?>(length => length == SubjectClass.StringLength))).Verifiable();

            // Act

            object result = this.valueGenerator.GetValue(propertyInfo);

            // Assert

            this.randomizerMock.Verify();
        }
  
        [TestMethod]
        public void GetValue_Attribute_Tests()
        {
            // Arrange

            var propertyNameAnVerifierList = new List<Tuple<string, Action>>
            {
                new Tuple<string, Action>(
                    "DecimalWithPrecision",
                    () =>
                        this.randomizerMock.Verify(
                            m => m.GetDecimal(It.Is<int?>(precision => precision == SubjectClass.Precision)),
                            Times.Once())
                ),
                new Tuple<string, Action>(
                    "DoubleWithPrecision",
                    () =>
                        this.randomizerMock.Verify(
                            m => m.GetDouble(It.Is<int?>(precision => precision == SubjectClass.Precision)),
                            Times.Once())
                ),
                new Tuple<string, Action>(
                    "IntegerWithMax",
                    () =>
                        this.randomizerMock.Verify((
                            m => m.GetInteger(It.Is<int?>(max => max == SubjectClass.Max))),
                            Times.Once())
                    ),
                new Tuple<string, Action>(
                    "LongIntegerWithMax",
                    () =>
                        this.randomizerMock.Verify((
                            m => m.GetLongInteger(It.Is<long?>(max => max == SubjectClass.Max))),
                            Times.Once())
                    ),
                new Tuple<string, Action>(
                    "ShortIntegerWithMax",
                    () =>
                        this.randomizerMock.Verify((
                            m => m.GetShortInteger(It.Is<short?>(max => max == SubjectClass.Max))),
                            Times.Once())
                    ),
                new Tuple<string, Action>(
                    "DateTimeWithTense",
                    () =>
                        this.randomizerMock.Verify((
                            m => m.GetDateTime(It.Is<PastOrFuture?>(pastOrFuture => pastOrFuture == PastOrFuture.Future), It.IsAny<Func<long?, long>>())),
                            Times.Once())
                    ),
            };

            // Act and Assert

            propertyNameAnVerifierList.ForEach(propertyNameVerifier =>
            {
                PropertyInfo propertyInfo = typeof (SubjectClass).GetProperty(propertyNameVerifier.Item1);
                this.valueGenerator.GetValue(propertyInfo);
                propertyNameVerifier.Item2();
            });            
        }

        [TestMethod]
        public void GetValue_ComplexObject_Test()
        {
            // Arrange

            var secondClass = new SecondClass();

            this.typeGeneratorMock.Setup(m => m.GetObject(It.Is<Type>(t => t == typeof (SecondClass)))).Returns(secondClass);
            PropertyInfo propertyInfo = typeof (SubjectClass).GetProperty("SecondObject");

            // Act

            object result = this.valueGenerator.GetValue(propertyInfo);
            this.typeGeneratorMock.Verify();

            // Assert

            var secondObject = result as SecondClass;
            Assert.IsNotNull(secondObject);
        }

        [TestMethod]
        public void GetValue_MaxAttributeOutOfRange_Test()
        {
            // Arrange

            var inputs = new[]
            {
                new
                {
                    Property = "IntegerMaxLessThanZero",
                    ExceptionType = typeof (ArgumentOutOfRangeException),
                    Message = Messages.MaxAttributeLessThanZero
                },
                new
                {
                    Property = "IntegerMaxOutOfRange",
                    ExceptionType = typeof (ArgumentOutOfRangeException),
                    Message = string.Format(Messages.MaxAttributeOutOfRange, "int")
                },
                new
                {
                    Property = "LongMaxLessThanZero",
                    ExceptionType = typeof (ArgumentOutOfRangeException),
                    Message = Messages.MaxAttributeLessThanZero
                },
                new
                {
                    Property = "ShortMaxLessThanZero",
                    ExceptionType = typeof (ArgumentOutOfRangeException),
                    Message = Messages.MaxAttributeLessThanZero
                },
                new
                {
                    Property = "ShortMaxOutOfRange",
                    ExceptionType = typeof (ArgumentOutOfRangeException),
                    Message = string.Format(Messages.MaxAttributeOutOfRange, "short")
                },
            };

            foreach (var input in inputs)
            {
                Action action = () => this.valueGenerator.GetValue(typeof(ClassWithMaxInvalidMaxRanges).GetProperty(input.Property));

                Helpers.ExceptionTest(action, input.ExceptionType, input.Message);
            }
        }

        [TestMethod]
        public void GetValue_ArrayTest()
        {
            // Arrange

            var expected = new int[0];

            PropertyInfo simpleArrayPropertyInfo = typeof (SubjectClass).GetProperty("SimpleArray");

            this.arrayRandomizerMock.Setup(
                m =>
                    m.GetArray(It.Is<PropertyInfo>(p => p == simpleArrayPropertyInfo),
                        It.Is<Type>(t => t == simpleArrayPropertyInfo.PropertyType)))
                .Returns(expected);
            
            // Act

            object result = this.valueGenerator.GetValue(simpleArrayPropertyInfo);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ManualPrimaryKey_ReturnsDefaultIntegral_Test()
        {
            PropertyInfo primaryKeyPropertyInfo = typeof(ManualKeyPrimaryTable).GetProperty("Key2");
            const int expected = default(int);

            this.uniqueValueGeneratorMock.Setup(m => m.GetValue(primaryKeyPropertyInfo)).Returns(default(int));

            object result1 = this.valueGenerator.GetValue(primaryKeyPropertyInfo);
            object result2 = this.valueGenerator.GetValue(primaryKeyPropertyInfo);

            this.uniqueValueGeneratorMock.Verify();
            Assert.AreEqual(expected, result1);
            Assert.AreEqual(expected, result2);
        }

        [TestMethod]
        public void AutoPrimaryKey_Test()
        {
            PropertyInfo primaryKeyPropertyInfo = typeof(PrimaryTable).GetProperty("Key");

            this.uniqueValueGeneratorMock.Setup(m => m.GetValue(primaryKeyPropertyInfo)).Returns(1).Verifiable();

            object result = this.valueGenerator.GetValue(primaryKeyPropertyInfo);

            this.uniqueValueGeneratorMock.Verify();
            Assert.AreEqual(1, result);
        }
    }
}
