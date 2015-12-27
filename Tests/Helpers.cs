using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
