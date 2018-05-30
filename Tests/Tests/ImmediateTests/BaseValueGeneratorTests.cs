/*
    Copyright 2016, 2017 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting;
using TestDataFramework.Exceptions;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.UniqueValueGenerator.Interfaces;
using TestDataFramework.ValueGenerator;
using TestDataFramework.ValueProvider.Interfaces;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class BaseValueGeneratorTests
    {
        private Mock<IValueProvider> randomizerMock;
        private Mock<ITypeGenerator> typeGeneratorMock;
        private Mock<IArrayRandomizer> arrayRandomizerMock;
        private BaseValueGenerator valueGenerator;
        private Mock<IUniqueValueGenerator> uniqueValueGeneratorMock;

        private const int IntegerResult = 5;
        private const long LongResult = 6;
        private const short ShortResult = 7;
        private const uint UnsignedIntegerResult = BaseValueGeneratorTests.IntegerResult;
        private const ulong UnsignedLongResult = BaseValueGeneratorTests.LongResult;
        private const ushort UnsignedShortResult = (ushort)BaseValueGeneratorTests.ShortResult;
        private static readonly string StringResult = Guid.NewGuid().ToString("N");
        private const char CharacterResult = 'A';
        private const decimal DecimalResult = 48576.412587m;
        private const bool BooleanResult = true;
        private static readonly DateTime DateTimeResult = DateTime.Now;
        private const byte ByteResult = 8;
        private const double DoubleResult = 574.1575d;
        private const float FloatResult = 4646.158f;
        private const string EmailAddress = "address@domain.com";

        private class ValueGenerator : BaseValueGenerator
        {
            public ValueGenerator(IValueProvider valueProvider, Func<ITypeGenerator> getTypeGenerator,
                Func<IArrayRandomizer> getArrayRandomizer, IUniqueValueGenerator uniqueValueGenerator, IAttributeDecorator attributeDecorator)
                : base(valueProvider, getTypeGenerator, getArrayRandomizer, uniqueValueGenerator, attributeDecorator)
            {
            }

            protected override object GetGuid(PropertyInfo propertyInfo)
            {
                return default(Guid);
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.randomizerMock = new Mock<IValueProvider>();
            this.typeGeneratorMock = new Mock<ITypeGenerator>();
            this.arrayRandomizerMock = new Mock<IArrayRandomizer>();
            this.uniqueValueGeneratorMock = new Mock<IUniqueValueGenerator>();

            this.randomizerMock.Setup(m => m.GetInteger(It.Is<int?>(max => max == null))).Returns(BaseValueGeneratorTests.IntegerResult);
            this.randomizerMock.Setup(m => m.GetLongInteger(It.Is<long?>(max => max == null))).Returns(BaseValueGeneratorTests.LongResult);
            this.randomizerMock.Setup(m => m.GetShortInteger(It.Is<short?>(max => max == null))).Returns(BaseValueGeneratorTests.ShortResult);
            this.randomizerMock.Setup(m => m.GetString(It.Is<int?>(length => length == null))).Returns(BaseValueGeneratorTests.StringResult);
            this.randomizerMock.Setup(m => m.GetCharacter()).Returns(BaseValueGeneratorTests.CharacterResult);
            this.randomizerMock.Setup(m => m.GetDecimal(It.Is<int?>(precision => precision == null))).Returns(BaseValueGeneratorTests.DecimalResult);
            this.randomizerMock.Setup(m => m.GetBoolean()).Returns(BaseValueGeneratorTests.BooleanResult);
            this.randomizerMock.Setup(
                m =>
                    m.GetDateTime(It.Is<PastOrFuture?>(pastOrFuture => pastOrFuture == null),
                        It.Is<Func<long?, long>>(lir => lir == this.randomizerMock.Object.GetLongInteger), null, null))
                .Returns(BaseValueGeneratorTests.DateTimeResult);

            this.randomizerMock.Setup(m => m.GetByte()).Returns(BaseValueGeneratorTests.ByteResult);
            this.randomizerMock.Setup(m => m.GetDouble(It.Is<int?>(precision => precision == null))).Returns(BaseValueGeneratorTests.DoubleResult);
            this.randomizerMock.Setup(m => m.GetFloat(It.Is<int?>(precision => precision == null))).Returns(BaseValueGeneratorTests.FloatResult);
            this.randomizerMock.Setup(m => m.GetEmailAddress()).Returns(BaseValueGeneratorTests.EmailAddress);

            this.valueGenerator = new ValueGenerator(this.randomizerMock.Object, () => this.typeGeneratorMock.Object,
                () => this.arrayRandomizerMock.Object, this.uniqueValueGeneratorMock.Object, new StandardAttributeDecorator(null, null));
        }

        [TestMethod]
        public void AllTypeTests()
        {
            var list = new List<Tuple<string, object>>
            {
                new Tuple<string, object>("Integer", BaseValueGeneratorTests.IntegerResult),
                new Tuple<string, object>("UnsignedInteger", BaseValueGeneratorTests.UnsignedIntegerResult),
                new Tuple<string, object>("LongInteger", BaseValueGeneratorTests.LongResult),
                new Tuple<string, object>("UnsignedLongInteger", BaseValueGeneratorTests.UnsignedLongResult),
                new Tuple<string, object>("ShortInteger", BaseValueGeneratorTests.ShortResult),
                new Tuple<string, object>("UnsignedShortInteger", BaseValueGeneratorTests.UnsignedShortResult),
                new Tuple<string, object>("Text", BaseValueGeneratorTests.StringResult),
                new Tuple<string, object>("Character", BaseValueGeneratorTests.CharacterResult),
                new Tuple<string, object>("Decimal", BaseValueGeneratorTests.DecimalResult),
                new Tuple<string, object>("Boolean", BaseValueGeneratorTests.BooleanResult),
                new Tuple<string, object>("DateTime", BaseValueGeneratorTests.DateTimeResult),
                new Tuple<string, object>("Byte", BaseValueGeneratorTests.ByteResult),
                new Tuple<string, object>("Double", BaseValueGeneratorTests.DoubleResult),
                new Tuple<string, object>("Float", BaseValueGeneratorTests.FloatResult),
                new Tuple<string, object>("NullableInteger", BaseValueGeneratorTests.IntegerResult),
                new Tuple<string, object>("UnsignedNullableInteger", BaseValueGeneratorTests.UnsignedIntegerResult),
                new Tuple<string, object>("NullableLong", BaseValueGeneratorTests.LongResult),
                new Tuple<string, object>("UnsignedNullableLong", BaseValueGeneratorTests.UnsignedLongResult),
                new Tuple<string, object>("NullableShort", BaseValueGeneratorTests.ShortResult),
                new Tuple<string, object>("UnsignedNullableShort", BaseValueGeneratorTests.UnsignedShortResult),
                new Tuple<string, object>("AnEmailAddress", BaseValueGeneratorTests.EmailAddress),
            };

            list.ForEach(type => this.TypeTest(type.Item1, type.Item2));
        }

        private void TypeTest(string propertyName, object expectedResult)
        {
            Console.WriteLine("Executing for " + propertyName);

            // Arrange

            PropertyInfo propertyInfo = typeof (SubjectClass).GetProperty(propertyName);

            // Act

            object result = this.valueGenerator.GetValue(propertyInfo, (ObjectGraphNode) null);

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

            this.valueGenerator.GetValue(propertyInfo, (ObjectGraphNode)null);

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
                        this.randomizerMock.Verify(
                            m => m.GetLongInteger(It.Is<long?>(max => max == SubjectClass.Max)),
                            Times.Once())
                    ),
                new Tuple<string, Action>(
                    "ShortIntegerWithMax",
                    () =>
                        this.randomizerMock.Verify(
                            m => m.GetShortInteger(It.Is<short?>(max => max == SubjectClass.Max)),
                            Times.Once())
                    ),
                new Tuple<string, Action>(
                    "DateTimeWithTense",
                    () =>
                        this.randomizerMock.Verify(
                            m => m.GetDateTime(It.Is<PastOrFuture?>(pastOrFuture => pastOrFuture == PastOrFuture.Future), It.IsAny<Func<long?, long>>(), null, null),
                            Times.Once())
                    ),
            };

            // Act and Assert

            propertyNameAnVerifierList.ForEach(propertyNameVerifier =>
            {
                PropertyInfo propertyInfo = typeof (SubjectClass).GetProperty(propertyNameVerifier.Item1);
                this.valueGenerator.GetValue(propertyInfo, (ObjectGraphNode)null);
                propertyNameVerifier.Item2();
            });            
        }

        [TestMethod]
        public void GetValue_ComplexObject_Test()
        {
            // Arrange

            var secondClass = new SecondClass();

            this.typeGeneratorMock.Setup(m => m.GetObject(It.Is<Type>(t => t == typeof (SecondClass)), null)).Returns(secondClass);
            PropertyInfo propertyInfo = typeof (SubjectClass).GetProperty("SecondObject");

            // Act

            object result = this.valueGenerator.GetValue(propertyInfo, (ObjectGraphNode)null);
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
                Action action =
                    () => this.valueGenerator.GetValue(typeof(ClassWithMaxInvalidMaxRanges).GetProperty(input.Property),
                        (ObjectGraphNode) null);

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

            object result = this.valueGenerator.GetValue(simpleArrayPropertyInfo, (ObjectGraphNode)null);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ManualPrimaryKey_ReturnsDefaultIntegral_Test()
        {
            PropertyInfo primaryKeyPropertyInfo = typeof(ManualKeyPrimaryTable).GetProperty("Key2");
            const int expected = default(int);

            this.uniqueValueGeneratorMock.Setup(m => m.GetValue(primaryKeyPropertyInfo)).Returns(default(int));

            object result1 = this.valueGenerator.GetValue(primaryKeyPropertyInfo, (ObjectGraphNode)null);
            object result2 = this.valueGenerator.GetValue(primaryKeyPropertyInfo, (ObjectGraphNode)null);

            this.uniqueValueGeneratorMock.Verify();
            Assert.AreEqual(expected, result1);
            Assert.AreEqual(expected, result2);
        }

        [TestMethod]
        public void AutoPrimaryKey_Test()
        {
            PropertyInfo primaryKeyPropertyInfo = typeof(PrimaryTable).GetProperty("Key");

            this.uniqueValueGeneratorMock.Setup(m => m.GetValue(primaryKeyPropertyInfo)).Returns(1).Verifiable();

            object result = this.valueGenerator.GetValue(primaryKeyPropertyInfo, (ObjectGraphNode)null);

            this.uniqueValueGeneratorMock.Verify();
            Assert.AreEqual(1, result);
        }
    }
}
