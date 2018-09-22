/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class UniqueValueTypeGeneratorTests
    {
        private Mock<IValueGenerator> accumulatorValueGeneratorMock;
        private Mock<IHandledTypeGenerator> handledTypeGeneratorMock;
        private Mock<ITypeGeneratorService> typeGeneratorServiceMock;
        private UniqueValueTypeGenerator uniqueValueTypeGenerator;
        private Mock<IValueGenerator> valueGeneratorMock;
        private Mock<RecursionGuard> recursionGuardMock;

        [TestInitialize]
        public void Initialize()
        {
            this.accumulatorValueGeneratorMock = new Mock<IValueGenerator>();
            this.valueGeneratorMock = new Mock<IValueGenerator>();
            this.handledTypeGeneratorMock = new Mock<IHandledTypeGenerator>();
            this.typeGeneratorServiceMock = new Mock<ITypeGeneratorService>();
            this.recursionGuardMock = Helpers.GetMock<RecursionGuard>();

            this.recursionGuardMock.Setup(m => m.Push(It.IsAny<Type>(), It.IsAny<IEnumerable<ExplicitPropertySetter>>(),
                It.IsAny<ObjectGraphNode>())).Returns(true);

            this.uniqueValueTypeGenerator = new UniqueValueTypeGenerator(
                typeGenerator => this.accumulatorValueGeneratorMock.Object,
                this.valueGeneratorMock.Object,
                this.handledTypeGeneratorMock.Object,
                this.typeGeneratorServiceMock.Object);
        }

        [TestMethod]
        public void Unique_SetProperties_Test()
        {
            // Arrange

            this.typeGeneratorServiceMock.Setup(
                    m => m.GetExplicitlySetPropertySetters(It.IsAny<IEnumerable<ExplicitPropertySetter>>(),
                        It.IsAny<ObjectGraphNode>()))
                .Returns(Enumerable.Empty<ExplicitPropertySetter>());

            int uniqueReturnValue = 5;

            this.accumulatorValueGeneratorMock.Setup(
                m => m.GetValue(
                    It.Is<PropertyInfo>(propertyInfo => propertyInfo.Name ==
                                                        nameof(ClassWithValueAndRefernceTypeProperties.AnInteger)),
                    It.IsAny<ObjectGraphNode>(), It.IsAny<TypeGeneratorContext>())).Returns(uniqueReturnValue);

            // Act

            object result =
                this.uniqueValueTypeGenerator.GetObject<ClassWithValueAndRefernceTypeProperties>(
                    new TypeGeneratorContext(this.recursionGuardMock.Object, null));

            // Assert

            var subject = result as ClassWithValueAndRefernceTypeProperties;
            Assert.IsNotNull(subject);
            Assert.AreEqual(uniqueReturnValue, subject.AnInteger);
        }

        [TestMethod]
        public void Base_SetProperties_Test()
        {
            // Arrange

            var referenceReturnValue = new object();

            this.typeGeneratorServiceMock.Setup(
                    m => m.GetExplicitlySetPropertySetters(It.IsAny<IEnumerable<ExplicitPropertySetter>>(),
                        It.IsAny<ObjectGraphNode>()))
                .Returns(Enumerable.Empty<ExplicitPropertySetter>());

            this.valueGeneratorMock.Setup(
                m => m.GetValue(
                    It.Is<PropertyInfo>(propertyInfo => propertyInfo.Name ==
                                                        nameof(ClassWithValueAndRefernceTypeProperties.ARefernce)),
                    It.IsAny<ObjectGraphNode>(), It.IsAny<TypeGeneratorContext>())).Returns(referenceReturnValue);

            // Act

            object result =
                this.uniqueValueTypeGenerator.GetObject<ClassWithValueAndRefernceTypeProperties>(
                    new TypeGeneratorContext(this.recursionGuardMock.Object, null));

            // Assert

            var subject = result as ClassWithValueAndRefernceTypeProperties;
            Assert.IsNotNull(subject);
            Assert.AreEqual(referenceReturnValue, subject.ARefernce);
        }
    }
}