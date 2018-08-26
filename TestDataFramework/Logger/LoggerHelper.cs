using log4net;

namespace TestDataFramework.Logger
{
    public static class LoggerHelper
    {
        public static void Entering(this ILog logger, string method)
        {
            logger.Debug("Entering " + method);
        }

        public static void Entering(this ILog logger, string method, object context)
        {
            logger.Debug($"Entering {method}: Context: {context}");
        }

        public static void Exiting(this ILog logger, string method)
        {
            logger.Debug("Exiting " + method);
        }

        public static void Exiting(this ILog logger, string method, object result)
        {
            logger.Debug($"Exiting {method}: Result: {result}");
        }

        public static void Calling(this ILog logger, string method, object context)
        {
            logger.Debug($"Calling {method}: Context: {context}");
        }

        public static void Calling(this ILog logger, string method)
        {
            logger.Debug($"Calling {method}");
        }
    }
}
