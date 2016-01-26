using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Exceptions;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.TypeGenerator;
using TestDataFramework.ValueGenerator;
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

            this.typeGenerator = new StandardTypeGenerator(x => this.valueGeneratorMock.Object,
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
        }

        [TestMethod]
        public void GetObject_RecursiveTypeSetToNull()
        {
            this.typeGenerator.GetObject(typeof(InfiniteRecursiveClass1));
            this.typeGenerator.GetObject(typeof(InfiniteRecursiveClass2));
            this.typeGenerator.GetObject(typeof(InfiniteRecursiveClass3));

            object result = this.typeGenerator.GetObject(typeof(InfiniteRecursiveClass1));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetObject_NoDefaultConstructor_ReturnsNull_Test()
        {
            throw new NotImplementedException();

            this.typeGenerator.GetObject(typeof (ClassWithoutADefaultConstructor));
        }

        [TestMethod]
        public void RecursionGuard_Test()
        {
            throw new NotImplementedException();
        }
    }
}
