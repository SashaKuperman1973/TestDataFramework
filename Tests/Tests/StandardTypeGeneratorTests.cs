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
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Concrete;
using TestDataFramework.HandledTypeGenerator;
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
            XmlConfigurator.Configure();

            this.valueGeneratorMock = new Mock<IValueGenerator>();
            this.handledTypeGeneratorMock = new Mock<IHandledTypeGenerator>();
            this.typeGeneratorServiceMock = new Mock<ITypeGeneratorService>();
            this.recursionGuardMock = new Mock<RecursionGuard>();

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
                    It.Is<PropertyInfo>(p => p.PropertyType == typeof(int)),
                    It.IsAny<ObjectGraphNode>()))
                .Returns(expected);

            // Act

            object result = this.typeGenerator.GetObject<SecondClass>(new List<ExplicitPropertySetter>());

            // Assert

            var secondClassObject = result as SecondClass;
            Assert.IsNotNull(secondClassObject);

            Assert.AreEqual(expected, secondClassObject.SecondInteger);

            this.valueGeneratorMock.Verify(
                m => m.GetValue(It.Is<PropertyInfo>(p => p.PropertyType == typeof(int)), It.IsAny<ObjectGraphNode>()),
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

            var explicitPropertySetters = new ExplicitPropertySetter[0];

            // Act

            var result =
                this.typeGenerator.GetObject<InfiniteRecursiveClass1>(explicitPropertySetters) as
                    RecursionRootClass;

            // Assert

            this.recursionGuardMock.Verify(m => m.Push(typeof(InfiniteRecursiveClass1),
                It.IsAny<IEnumerable<ExplicitPropertySetter>>(), It.IsAny<ObjectGraphNode>()));

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Never);
        }

        // TODO 2 - Remove this and put it tn the RecursionGuard tests
        public void GetObject_ExplicitSetter_RecursionGuard_Test_ToReplace()
        {                
            // Arrange

            Type[] types =
            {
                typeof(InfiniteRecursiveClass1),
                typeof(InfiniteRecursiveClass2),
                typeof(InfiniteRecursiveClass1),
            };

            int i = 0;

            this.valueGeneratorMock.Setup(m => m.GetValue(It.IsAny<PropertyInfo>(), It.IsAny<ObjectGraphNode>()))

                .Returns<PropertyInfo, ObjectGraphNode>((pi, objectGraphNode) =>

                    this.typeGenerator.GetObject(types[i++], objectGraphNode));

            var objectGraphService = new ObjectGraphService();

            List<PropertyInfo> objectGraph = objectGraphService.GetObjectGraph<RecursionRootClass, InfiniteRecursiveClass2>(
                m => m.RecursionProperty1.InfinietRecursiveObjectA.InfiniteRecursiveObjectB.InfinietRecursiveObjectA);

            var valueForSetting = new InfiniteRecursiveClass2();

            var explicitPropertySetters = new List<ExplicitPropertySetter>
            {
                new ExplicitPropertySetter
                {
                    PropertyChain = objectGraph,

                    Action = @object => typeof(InfiniteRecursiveClass1)
                        .GetProperty(nameof(InfiniteRecursiveClass1.InfinietRecursiveObjectA))
                        .SetValue(@object, valueForSetting)
                }
            };

            this.typeGeneratorServiceMock.Setup(
                    m => m.GetExplicitlySetPropertySetters(It.IsAny<List<ExplicitPropertySetter>>(),
                        It.IsAny<ObjectGraphNode>()))
                .Returns<List<ExplicitPropertySetter>, ObjectGraphNode>((l, n) =>
                    i == 3
                        ? explicitPropertySetters
                        : Enumerable.Empty<ExplicitPropertySetter>());

            // Act

            var result =
                this.typeGenerator.GetObject<RecursionRootClass>(explicitPropertySetters) as
                    RecursionRootClass;

            // Assert

            Assert.IsNotNull(result.RecursionProperty1.InfinietRecursiveObjectA.InfiniteRecursiveObjectB.InfinietRecursiveObjectA);
            Assert.IsNull(result.RecursionProperty1.InfinietRecursiveObjectA.InfiniteRecursiveObjectB.InfinietRecursiveObjectA.InfiniteRecursiveObjectB);
            Assert.AreEqual(valueForSetting, result.RecursionProperty1.InfinietRecursiveObjectA.InfiniteRecursiveObjectB.InfinietRecursiveObjectA);
        }

        [TestMethod]
        public void GetObject_WithExplicitPropertySetters_Test()
        {
            // Arrange

            const int expected = 7;

            var explicitProperySetters = new List<ExplicitPropertySetter>();

            PropertyInfo propertyInfo = typeof(SecondClass).GetProperty(nameof(SecondClass.SecondInteger));

            Action<object> setter = @object => propertyInfo.SetValue(@object, expected);

            explicitProperySetters.Add(new ExplicitPropertySetter
            {
                Action = setter,
                PropertyChain = new List<PropertyInfo> {propertyInfo}
            });

            this.typeGeneratorServiceMock.Setup(m => m.GetExplicitlySetPropertySetters(explicitProperySetters,
                    It.Is<ObjectGraphNode>(node => node.PropertyInfo.Name == propertyInfo.Name)))
                .Returns(explicitProperySetters);

            this.typeGeneratorServiceMock.Setup(m => m.GetExplicitlySetPropertySetters(explicitProperySetters,
                    It.Is<ObjectGraphNode>(node => node.PropertyInfo.Name != propertyInfo.Name)))
                .Returns(Enumerable.Empty<ExplicitPropertySetter>());

            // Act

            var result = this.typeGenerator.GetObject<SecondClass>(explicitProperySetters) as SecondClass;

            // Assert

            Assert.AreEqual(expected, result.SecondInteger);

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Once);
            this.recursionGuardMock.Verify();
        }

        [TestMethod]
        public void GetObject_ValueType_Test()
        {
            // Act

            object result = this.typeGenerator.GetObject<AStruct>(new List<ExplicitPropertySetter>());

            // Assert

            Assert.IsTrue(result is AStruct);

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Once);
            this.recursionGuardMock.Verify();
        }

        [TestMethod]
        public void GetObject_NoDefaultConstructor_ReturnsNull_Test()
        {
            object result = this.typeGenerator.GetObject(typeof(ClassWithoutADefaultConstructor), null);

            Assert.IsNull(result);

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Once);
            this.recursionGuardMock.Verify();
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

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Never);

            this.recursionGuardMock.Verify(m => m.Push(It.IsAny<Type>(),
                It.IsAny<IEnumerable<ExplicitPropertySetter>>(), It.IsAny<ObjectGraphNode>()), Times.Never);
        }

        [TestMethod]
        public void GetObject_Uninstantiatable_Dependency_ResultsIn_NullRootObject()
        {
            // Act

            object resultObject =
                this.typeGenerator.GetObject<WithUninstantiatableDependency>(new List<ExplicitPropertySetter>());

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

            this.valueGeneratorMock.Setup(m => m.GetValue(
                    null,
                    It.Is<Type>(p => p == typeof(SecondClass))))
                .Returns(expected);

            // Act

            object result = this.typeGenerator.GetObject<ClassWithConstructor>(new List<ExplicitPropertySetter>());

            // Assert

            var secondClassObject = result as ClassWithConstructor;
            Assert.IsNotNull(secondClassObject);

            Assert.AreEqual(expected, secondClassObject.SecondClass);

            this.valueGeneratorMock.Verify(
                m => m.GetValue(null,
                    It.Is<Type>(p => p == typeof(SecondClass)))
                , Times.Once);

            this.recursionGuardMock.Verify(m => m.Pop(), Times.Once);
            this.recursionGuardMock.Verify();
        }

        [TestMethod]
        public void GetObject_ValueGenerator_IntrinsicValue_Is_Not_Null_Test()
        {
            // Arrange

            var expected = new object();

            this.valueGeneratorMock.Setup(m => m.GetIntrinsicValue(null, typeof(SubjectClass))).Returns(expected);

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