using System.Collections.Specialized;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Logger;

namespace Tests.Tests
{
    [TestClass]
    public class StandardLogManagerTests
    {
        [TestMethod]
        public void GetLogger_Gets_Production_Logger_Test()
        {
            // Arrange

            var appSettings = new NameValueCollection();
            appSettings.Add("TestDataFramework-EnableLogger", "true");

            // Act

            ILog logger = StandardLogManager.GetLogger(typeof(StandardLogManagerTests), appSettings);

            // Assert

            Assert.AreNotEqual(typeof(NullLogger), logger.GetType());
        }

        [TestMethod]
        public void GetLogger_Gets_Null_Logger_Test()
        {
            const string key = "TestDataFramework-EnableLogger";

            StandardLogManagerTests.GetLogger_Gets_Null_Logger(key, "false");
            StandardLogManagerTests.GetLogger_Gets_Null_Logger(key, "not_boolean");
            StandardLogManagerTests.GetLogger_Gets_Null_Logger(key, null);
            StandardLogManagerTests.GetLogger_Gets_Null_Logger("not_logger_key", "true");
        }

        private static void GetLogger_Gets_Null_Logger(string key, string value)
        {
            // Arrange

            var appSettings = new NameValueCollection();
            appSettings.Add(key, value);

            // Act

            ILog logger = StandardLogManager.GetLogger(typeof(StandardLogManagerTests), appSettings);

            // Assert

            Assert.AreEqual(typeof(NullLogger), logger.GetType());
        }
    }
}