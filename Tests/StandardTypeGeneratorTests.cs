using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Exceptions;
using TestDataFramework.TypeGenerator;
using TestDataFramework.ValueGenerator;

namespace Tests
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
            // Arrange

            var typeGenerator = new StandardTypeGenerator(x => this.valueGeneratorMock.Object);

            // Act

            typeGenerator.GetObject(typeof (InfiniteRecursiveClass1));
            typeGenerator.GetObject(typeof(InfiniteRecursiveClass2));
            typeGenerator.GetObject(typeof(InfiniteRecursiveClass3));

            TypeRecursionException exception = null;

            try
            {
                typeGenerator.GetObject(typeof (InfiniteRecursiveClass1));
            }
            catch (TypeRecursionException ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);

            Assert.AreEqual(string.Format(Messages.TypeRecursionExceptionMessage,
                typeof (InfiniteRecursiveClass1) + " -> " + typeof (InfiniteRecursiveClass2) + " -> " +
                typeof (InfiniteRecursiveClass3), typeof(InfiniteRecursiveClass1)), exception.Message);
        }
    }
}
