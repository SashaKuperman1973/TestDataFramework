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
using System.Linq;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.ValueGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardTypeGeneratorTests
    {
        private Mock<IHandledTypeGenerator> handledTypeGeneratorMock;
        private StandardTypeGenerator typeGenerator;
        private Mock<IValueGenerator> valueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.valueGeneratorMock = new Mock<IValueGenerator>();
            this.handledTypeGeneratorMock = new Mock<IHandledTypeGenerator>();

            this.typeGenerator = new StandardTypeGenerator(this.valueGeneratorMock.Object,
                this.handledTypeGeneratorMock.Object);
        }

        [TestMethod]
        public void GetObject_Test()
        {
            // Arrange

            const int expected = 5;

            this.valueGeneratorMock.Setup(m => m.GetValue(
                    It.Is<PropertyInfo>(p => p.PropertyType == typeof(int)),
                    It.IsAny<ObjectGraphNode>()))
                .Returns(expected);

            var explicitPropertySetters = new List<ExplicitPropertySetters>();

            // Act

            object result = this.typeGenerator.GetObject<SecondClass>(explicitPropertySetters);

            // Assert

            var secondClassObject = result as SecondClass;
            Assert.IsNotNull(secondClassObject);

            Assert.AreEqual(expected, secondClassObject.SecondInteger);

            this.valueGeneratorMock.Verify(
                m => m.GetValue(It.Is<PropertyInfo>(p => p.PropertyType == typeof(int)), It.IsAny<ObjectGraphNode>()),
                Times.Once);
        }

        [TestMethod]
        public void GetObject_RecursionGuard_Test()
        {
            Type[] types =
            {
                typeof(InfiniteRecursiveClass2),
                typeof(InfiniteRecursiveClass1)
            };

            var i = 0;
            this.valueGeneratorMock.Setup(m => m.GetValue(It.IsAny<PropertyInfo>(), It.IsAny<ObjectGraphNode>()))
                .Returns<PropertyInfo, ObjectGraphNode>((pi, objectGraphNode) =>
                    this.typeGenerator.GetObject(types[i++], null));

            var explicitPropertySetters = new List<ExplicitPropertySetters>();

            var result =
                this.typeGenerator.GetObject<InfiniteRecursiveClass1>(explicitPropertySetters) as
                    InfiniteRecursiveClass1;

            Assert.IsNull(result.InfinietRecursiveClassA.InfiniteRecursiveClassB);
        }

        [TestMethod]
        public void GetObject_WithExplicitPropertySetters_Test()
        {
            // Arrange

            const int expected = 7;

            var explicitProperySetters = new List<ExplicitPropertySetters>();

            PropertyInfo propertyInfo = typeof(SecondClass).GetProperty("SecondInteger");

            Action<object> setter = @object => propertyInfo.SetValue(@object, expected);

            explicitProperySetters.Add(new ExplicitPropertySetters
            {
                Action = setter,
                PropertyChain = new List<PropertyInfo> {propertyInfo}
            });

            // Act

            var result = this.typeGenerator.GetObject<SecondClass>(explicitProperySetters) as SecondClass;

            // Assert

            Assert.AreEqual(expected, result.SecondInteger);
        }

        [TestMethod]
        public void GetObject_ValueType_Test()
        {
            // Act

            object result = this.typeGenerator.GetObject<AStruct>(new List<ExplicitPropertySetters>());

            // Assert

            Assert.IsTrue(result is AStruct);
        }

        [TestMethod]
        public void GetObject_NoDefaultConstructor_ReturnsNull_Test()
        {
            object result = this.typeGenerator.GetObject(typeof(ClassWithoutADefaultConstructor), null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void HandledType_Test()
        {
            // Arrange

            this.handledTypeGeneratorMock.Setup(m => m.GetObject(typeof(KeyValuePair<int, string>)))
                .Returns(new KeyValuePair<int, string>(3, "ABCD"));

            // Act

            var result =
                (KeyValuePair<int, string>) this.typeGenerator.GetObject(typeof(KeyValuePair<int, string>), null);

            // Assert

            Assert.AreEqual(3, result.Key);
            Assert.AreEqual("ABCD", result.Value);
        }

        [TestMethod]
        public void GetObject_Uninstantiatable_Dependency_ResultsIn_NullRootObject()
        {
            // Act

            object resultObject =
                this.typeGenerator.GetObject<WithUninstantiatableDependency>(new List<ExplicitPropertySetters>());

            // Assert

            Assert.IsNull(resultObject);
        }

        [TestMethod]
        public void GetObject_NonEmpty_Constructor_Test()
        {
            // Arrange

            var expected = new SecondClass();

            this.valueGeneratorMock.Setup(m => m.GetValue(
                    null,
                    It.Is<Type>(p => p == typeof(SecondClass))))
                .Returns(expected);

            // Act

            object result = this.typeGenerator.GetObject<ClassWithConstructor>(new List<ExplicitPropertySetters>());

            // Assert

            var secondClassObject = result as ClassWithConstructor;
            Assert.IsNotNull(secondClassObject);

            Assert.AreEqual(expected, secondClassObject.SecondClass);

            this.valueGeneratorMock.Verify(
                m => m.GetValue(null,
                        It.Is<Type>(p => p == typeof(SecondClass)))
                , Times.Once);
        }
    }
}