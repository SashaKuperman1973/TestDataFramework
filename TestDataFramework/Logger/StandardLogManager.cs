using System;
using System.Configuration;
using log4net;

namespace TestDataFramework.Logger
{
    public class StandardLogManager
    {
        private static Lazy<NullLogger> LazyNullLogger = new Lazy<NullLogger>(() => new NullLogger());

        public static ILog GetLogger(Type type)
        {
            bool enableLogger;

            if (bool.TryParse(ConfigurationManager.AppSettings["TestDataFramework-EnableLogger"], out enableLogger) &&
                enableLogger)
            {
                return LogManager.GetLogger(type);
            }

            return StandardLogManager.LazyNullLogger.Value;
        }
    }
}
