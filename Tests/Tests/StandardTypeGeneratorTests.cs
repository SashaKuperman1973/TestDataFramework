using System;
using System.Reflection;
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
            this.valueGeneratorMock = new Mock<IValueGenerator>();
        }

        [TestMethod]
        public void GetObject_Test()
        {
            // Arrange

            this.valueGeneratorMock.Setup(m => m.GetValue(It.Is<PropertyInfo>(p => p.PropertyType == typeof (int))))
                .Returns(5);

            // Act

            var typeGenerator = new StandardTypeGenerator(x => this.valueGeneratorMock.Object);
            object result = typeGenerator.GetObject(typeof (SecondClass));

            // Assert

            SecondClass secondClassObject = result as SecondClass;
            Assert.IsNotNull(secondClassObject);

            Assert.AreEqual(5, secondClassObject.SecondInteger);
        }

        [TestMethod]
        public void GetObject_RecursiveTypeException()
        {
            var typeGenerator = new StandardTypeGenerator(x => this.valueGeneratorMock.Object);

            typeGenerator.GetObject(typeof(InfiniteRecursiveClass1));
            typeGenerator.GetObject(typeof(InfiniteRecursiveClass2));
            typeGenerator.GetObject(typeof(InfiniteRecursiveClass3));

            Action action = () =>
            {
                typeGenerator.GetObject(typeof(InfiniteRecursiveClass1));
            };

            string expectedMessage = string.Format(Messages.TypeRecursion,
                typeof (InfiniteRecursiveClass1) + " -> " + typeof (InfiniteRecursiveClass2) + " -> " +
                typeof (InfiniteRecursiveClass3), typeof (InfiniteRecursiveClass1));

            Helpers.ExceptionTest(action, typeof(TypeRecursionException), expectedMessage);
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
    }
}
