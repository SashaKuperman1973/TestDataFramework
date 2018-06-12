using System;
using System.Collections.Specialized;
using System.Configuration;
using log4net;

namespace TestDataFramework.Logger
{
    public class StandardLogManager
    {
        private static readonly Lazy<NullLogger> LazyNullLogger = new Lazy<NullLogger>(() => new NullLogger());

        public static ILog GetLogger(Type type, NameValueCollection appSettings = null)
        {
            bool enableLogger;

            if (bool.TryParse((appSettings ?? ConfigurationManager.AppSettings)["TestDataFramework-EnableLogger"],
                    out enableLogger) &&
                enableLogger)
                return LogManager.GetLogger(type);

            return StandardLogManager.LazyNullLogger.Value;
        }
    }
}