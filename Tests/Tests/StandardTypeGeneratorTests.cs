/*
    Copyright 2016 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.ValueGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardTypeGeneratorTests
    {
        private Mock<IValueGenerator> valueGeneratorMock;
        private Mock<IHandledTypeGenerator> handledTypeGeneratorMock;
        private StandardTypeGenerator typeGenerator;

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
            this.valueGeneratorMock.Setup(m => m.GetValue(It.Is<PropertyInfo>(p => p.PropertyType == typeof (int))))
                .Returns(expected);

            // Act

            object result = this.typeGenerator.GetObject(typeof (SecondClass));

            // Assert

            SecondClass secondClassObject = result as SecondClass;
            Assert.IsNotNull(secondClassObject);

            Assert.AreEqual(expected, secondClassObject.SecondInteger);
            this.valueGeneratorMock.Verify(m => m.GetValue(It.Is<PropertyInfo>(p => p.PropertyType == typeof (int))), Times.Once);
        }

        [TestMethod]
        public void GetObject_RecursionGuard_Test()
        {
            Type[] types = new[]
            {
                typeof (InfiniteRecursiveClass2),
                typeof (InfiniteRecursiveClass1),
            };

            int i = 0;
            this.valueGeneratorMock.Setup(m => m.GetValue(It.IsAny<PropertyInfo>()))
                .Returns<PropertyInfo>(pi =>
                    this.typeGenerator.GetObject(types[i++]));

            var result = this.typeGenerator.GetObject(typeof (InfiniteRecursiveClass1)) as InfiniteRecursiveClass1;

            Assert.IsNull(result.InfinietRecursiveClassA.InfiniteRecursiveClassB);
        }

        [TestMethod]
        public void GetObject_WithExplicitPropertySetters()
        {
            // Arrange

            const int expected = 7;

            ConcurrentDictionary<PropertyInfo, Action<SecondClass>> explicitProperySetters =
                        new ConcurrentDictionary<PropertyInfo, Action<SecondClass>>();

            PropertyInfo propertyInfo = typeof (SecondClass).GetProperty("SecondInteger");

            Action<SecondClass> setter = @object => propertyInfo.SetValue(@object, expected);
            explicitProperySetters.AddOrUpdate(propertyInfo, setter, (pi, lambda) => setter);

            // Act

            var result = this.typeGenerator.GetObject<SecondClass>(explicitProperySetters) as SecondClass;

            // Assert

            Assert.AreEqual(expected, result.SecondInteger);
        }

        [TestMethod]
        public void GetObject_NoDefaultConstructor_ReturnsNull_Test()
        {
            object result = this.typeGenerator.GetObject(typeof(ClassWithoutADefaultConstructor));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void HandledType_Test()
        {
            // Arrange

            this.handledTypeGeneratorMock.Setup(m => m.GetObject(typeof (KeyValuePair<int, string>))).Returns(new KeyValuePair<int, string>(3, "ABCD"));

            // Act

            var result = (KeyValuePair<int, string>)this.typeGenerator.GetObject(typeof (KeyValuePair<int, string>));

            // Assert

            Assert.AreEqual(3, result.Key);
            Assert.AreEqual("ABCD", result.Value);
        }
    }
}
