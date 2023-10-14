/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Concrete;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardTypeGeneratorTests
    {
        private Mock<IHandledTypeGenerator> handledTypeGeneratorMock;
        private StandardTypeGenerator typeGenerator;
        private Mock<ITypeGeneratorService> typeGeneratorServiceMock;
        private Mock<IValueGenerator> valueGeneratorMock;
        private Mock<RecursionGuard> recursionGuardMock;

        [TestInitialize]
        public void Initialize()
        {
            Helpers.ConfigureLogger();

            this.valueGeneratorMock = new Mock<IValueGenerator>();
            this.handledTypeGeneratorMock = new Mock<IHandledTypeGenerator>();
            this.typeGeneratorServiceMock = new Mock<ITypeGeneratorService>();
            this.recursionGuardMock = Helpers.GetMock<RecursionGuard>();

            this.typeGenerator = new StandardTypeGenerator(this.valueGeneratorMock.Object,
                this.handledTypeGeneratorMock.Object, this.typeGeneratorServiceMock.Object,
                this.recursionGuardMock.Object);

            this.recursionGuardMock.Setup(m => m.Push(It.IsAny<Type>(),
                    It.IsAny<IEnumerable<ExplicitPropertySetter>>(), It.IsAny<ObjectGraphNode>()))
                .Returns(true).Verifiable();
        }

        [TestMethod]
        public void GetObject_Test()
        {
            // Arrange

            const int expected = 5;

            this.valueGeneratorMock.Setup(m => m.GetValue(
                    It.Is<PropertyInfoProxy>(p => p.PropertyType == typeof(int)),
                    It.IsAny<ObjectGraphNode>(),
                    It.IsAny<TypeGeneratorContext>()))
                .Returns(expected);

            // Act

            object result = this.typeGenerator.GetObject<SecondClass>(new TypeGeneratorContext(null));

            // Assert

            var secondClassObject = result as SecondClass;
            Assert.IsNotNull(secondClassObject);

            Assert.AreEqual(expected, secondClassObject.SecondInteger);

            this.valueGeneratorMock.Verify(
                m => m.GetValue(It.Is<PropertyInfoProxy>(p => p.PropertyType == typeof(int)),
                    It.IsAny<ObjectGraphNode>(),
                    It.IsAny<TypeGeneratorContext>()),
                Times.Once);

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Once);
            this.recursionGuardMock.Verify();
        }

        [TestMethod]
        public void GetObject_RecursionGuard_Test()
        {
            // Arrange

            this.recursionGuardMock.Setup(m => m.Push(typeof(InfiniteRecursiveClass1),
                    It.IsAny<IEnumerable<ExplicitPropertySetter>>(), It.IsAny<ObjectGraphNode>()))
                .Returns(false).Verifiable();            

            // Act

            var result =
                this.typeGenerator.GetObject<InfiniteRecursiveClass1>(new TypeGeneratorContext(null)) as RecursionRootClass;

            // Assert

            this.recursionGuardMock.Verify(m => m.Push(typeof(InfiniteRecursiveClass1),
                It.IsAny<IEnumerable<ExplicitPropertySetter>>(), It.IsAny<ObjectGraphNode>()));

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Never);
        }

        [TestMethod]
        public void GetObject_WithExplicitPropertySetters_Test()
        {
            // Arrange

            const int expected = 7;

            PropertyInfoProxy propertyInfo = typeof(SecondClass).GetPropertyInfoProxy(nameof(SecondClass.SecondInteger));
            Action<object> setter = @object => propertyInfo.SetValue(@object, expected);
            TypeGeneratorContext typeGeneratorContext = this.SetupExplicitPropertySetter(propertyInfo, setter);

            // Act

            var result =
                this.typeGenerator.GetObject<SecondClass>(typeGeneratorContext) as SecondClass;

            // Assert

            Assert.AreEqual(expected, result.SecondInteger);

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Once);
            this.recursionGuardMock.Verify();
        }

        [TestMethod]
        public void GetObject_With_ExplicitPropertySetter_ThatThrows_Test()
        {
            // Act

            var result =
                this.typeGenerator.GetObject<ClassWithPropertySetterThatThrows>(new TypeGeneratorContext(null))
                    as ClassWithPropertySetterThatThrows;

            // Assert

            this.valueGeneratorMock.Verify(m => m.GetValue(It.IsAny<PropertyInfoProxy>(), It.IsAny<ObjectGraphNode>(),
                It.IsAny<TypeGeneratorContext>()), Times.Once);

            Assert.IsNotNull(result);
        }

        private TypeGeneratorContext SetupExplicitPropertySetter(PropertyInfoProxy propertyInfo, Action<object> setter)
        {
            var typeGeneratorContext = new TypeGeneratorContext(new List<ExplicitPropertySetter>());
            List<ExplicitPropertySetter> explicitProperySetters = typeGeneratorContext.ExplicitPropertySetters;

            explicitProperySetters.Add(new ExplicitPropertySetter
            {
                Action = setter,
                PropertyChain = new List<PropertyInfoProxy> { propertyInfo }
            });

            this.typeGeneratorServiceMock.Setup(m => m.GetExplicitlySetPropertySetters(explicitProperySetters,
                    It.Is<ObjectGraphNode>(node => node.PropertyInfoProxy.Name == propertyInfo.Name)))
                .Returns(explicitProperySetters);

            this.typeGeneratorServiceMock.Setup(m => m.GetExplicitlySetPropertySetters(explicitProperySetters,
                    It.Is<ObjectGraphNode>(node => node.PropertyInfoProxy.Name != propertyInfo.Name)))
                .Returns(Enumerable.Empty<ExplicitPropertySetter>());

            return typeGeneratorContext;
        }

        [TestMethod]
        public void GetObject_ValueType_Test()
        {
            // Act

            object result =
                this.typeGenerator.GetObject<AStruct>(new TypeGeneratorContext(null));

            // Assert

            Assert.IsTrue(result is AStruct);

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Once);
            this.recursionGuardMock.Verify();
        }

        [TestMethod]
        public void GetObject_NoDefaultConstructor_ReturnsNull_Test()
        {
            object result = this.typeGenerator.GetObject(typeof(ClassWithoutADefaultConstructor), null,
                new TypeGeneratorContext(null));

            Assert.IsNull(result);

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Once);
            this.recursionGuardMock.Verify();
        }

        [TestMethod]
        public void HandledType_Test()
        {
            // Arrange

            this.handledTypeGeneratorMock
                .Setup(m => m.GetObject(typeof(KeyValuePair<int, string>), It.IsAny<TypeGeneratorContext>()))
                .Returns(new KeyValuePair<int, string>(3, "ABCD"));

            // Act

            var result =
                (KeyValuePair<int, string>) this.typeGenerator.GetObject(typeof(KeyValuePair<int, string>), null,
                    new TypeGeneratorContext(null));

            // Assert

            Assert.AreEqual(3, result.Key);
            Assert.AreEqual("ABCD", result.Value);

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Once);

            this.recursionGuardMock.Verify(m => m.Push(It.IsAny<Type>(),
                It.IsAny<IEnumerable<ExplicitPropertySetter>>(), It.IsAny<ObjectGraphNode>()), Times.Once);
        }

        [TestMethod]
        public void GetObject_Uninstantiatable_Dependency_ResultsIn_NullRootObject()
        {
            // Act

            object resultObject =
                this.typeGenerator.GetObject<WithUninstantiatableDependency>(new TypeGeneratorContext(null));

            // Assert

            Assert.IsNull(resultObject);

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Once);
            this.recursionGuardMock.Verify();
        }

        [TestMethod]
        public void GetObject_NonEmpty_Constructor_Test()
        {
            // Arrange

            var expected = new SecondClass();

            var context = new TypeGeneratorContext(null);

            this.valueGeneratorMock.Setup(m => m.GetValue(
                    null,
                    It.Is<Type>(p => p == typeof(SecondClass)), It.IsAny<TypeGeneratorContext>()))
                .Returns(expected)
                .Verifiable();

            this.valueGeneratorMock.Setup(
                    m => m.GetValue(It.Is<PropertyInfoProxy>(p => p.PropertyType == typeof(SecondClass)),
                        It.Is<ObjectGraphNode>(p => p.PropertyInfoProxy.PropertyType == typeof(SecondClass)),
                        It.IsAny<TypeGeneratorContext>()))
                .Returns(expected)
                .Verifiable();

            // Act

            object result = this.typeGenerator.GetObject<ClassWithConstructor>(context);

            // Assert

            var secondClassObject = result as ClassWithConstructor;
            Assert.IsNotNull(secondClassObject);

            Assert.AreEqual(expected, secondClassObject.SecondClass);

            this.valueGeneratorMock.Verify();
            this.recursionGuardMock.Verify(m => m.Pop(), Times.Once);
            this.recursionGuardMock.Verify();
        }

        [TestMethod]
        public void GetObject_WithAConstructor_ThatThrows_Test()
        {
            // Act

            object result = this.typeGenerator.GetObject<ClassWithConstructorThatThrows>(new TypeGeneratorContext(null));

            // Assert

            this.valueGeneratorMock.Verify(m => m.GetValue(It.IsAny<PropertyInfoProxy>(), It.IsAny<ObjectGraphNode>(),
                It.IsAny<TypeGeneratorContext>()), Times.Never);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetObject_ValueGenerator_IntrinsicValue_Is_Not_Null_Test()
        {
            // Arrange

            var expected = new object();

            this.valueGeneratorMock
                .Setup(m => m.GetIntrinsicValue(null, typeof(SubjectClass), It.IsAny<TypeGeneratorContext>()))
                .Returns(expected);

            // Act

            object result = this.typeGenerator.GetObject<SubjectClass>(null);

            // Assert

            Assert.AreEqual(expected, result);

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Never);

            this.recursionGuardMock.Verify(m => m.Push(It.IsAny<Type>(),
                It.IsAny<IEnumerable<ExplicitPropertySetter>>(), It.IsAny<ObjectGraphNode>()), Times.Never);
        }
    }
}