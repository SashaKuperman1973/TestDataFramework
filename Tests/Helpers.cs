using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.TypeGenerator;
using Tests.TestModels;

namespace Tests
{
    public static class Helpers
    {
        public static void ExceptionTest(Action getValue, Type exceptionType, string message)
        {
            // Act

            Exception exception = null;

            try
            {
                getValue();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert

            Assert.IsNotNull(exception);
            Assert.AreEqual(exceptionType, exception.GetType());
            Assert.AreEqual(message, exception.Message);
        }

        public static Mock<ITypeGenerator> GetTypeGeneratorMock<T>(T returnObject)
        {
            var typeGeneratorMock = new Mock<ITypeGenerator>();

            typeGeneratorMock.Setup(
                m => m.GetObject<T>(It.IsAny<ConcurrentDictionary<PropertyInfo, Action<T>>>()))
                .Returns(returnObject);

            return typeGeneratorMock;
        }
    }
}
