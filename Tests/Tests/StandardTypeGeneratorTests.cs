using System;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Exceptions;
using TestDataFramework.TypeGenerator;
using TestDataFramework.ValueGenerator;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardTypeGeneratorTests
    {
        private Mock<IValueGenerator> valueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.valueGeneratorMock = new Mock<IValueGenerator>();
        }

        [TestMethod]
        public void GetObject_Test()
        {
            // Arrange

            const int expected = 5;
            this.valueGeneratorMock.Setup(m => m.GetValue(It.Is<PropertyInfo>(p => p.PropertyType == typeof (int))))
                .Returns(expected);

            // Act

            var typeGenerator = new StandardTypeGenerator(x => this.valueGeneratorMock.Object);
            object result = typeGenerator.GetObject(typeof (SecondClass));

            // Assert

            SecondClass secondClassObject = result as SecondClass;
            Assert.IsNotNull(secondClassObject);

            Assert.AreEqual(expected, secondClassObject.SecondInteger);
        }

        [TestMethod]
        public void GetObject_RecursiveTypeSetToNull()
        {
            var typeGenerator = new StandardTypeGenerator(x => this.valueGeneratorMock.Object);

            typeGenerator.GetObject(typeof(InfiniteRecursiveClass1));
            typeGenerator.GetObject(typeof(InfiniteRecursiveClass2));
            typeGenerator.GetObject(typeof(InfiniteRecursiveClass3));

            object result = typeGenerator.GetObject(typeof(InfiniteRecursiveClass1));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetObject_NoDefaultConstructorException()
        {
            var typeGenerator = new StandardTypeGenerator(x => this.valueGeneratorMock.Object);

            Action action = () => typeGenerator.GetObject(typeof (ClassWithoutADefaultConstructor));

            string expectedMessage = Messages.NoDefaultConstructor +
                                     typeof (ClassWithoutADefaultConstructor);

            Helpers.ExceptionTest(action, typeof (NoDefaultConstructorException), expectedMessage);
        }

        [TestMethod]
        public void ResetRecursionGuard_Test()
        {
            // Arrange

            const int expected = 5;
            this.valueGeneratorMock.Setup(m => m.GetValue(It.Is<PropertyInfo>(p => p.PropertyType == typeof(int))))
                .Returns(expected);

            // Act

            var typeGenerator = new StandardTypeGenerator(x => this.valueGeneratorMock.Object);
            object result1 = typeGenerator.GetObject(typeof(SecondClass));

            typeGenerator.ResetRecursionGuard();
            object result2 = typeGenerator.GetObject(typeof(SecondClass));

            // Assert

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
        }
    }
}
