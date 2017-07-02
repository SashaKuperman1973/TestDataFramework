using System;
using log4net;
using log4net.Core;

namespace TestDataFramework.Logger
{
    public class NullLogger : ILog
    {
        public ILogger Logger { get; }
        public void Debug(object message)
        {
            // NoOp
        }

        public void Debug(object message, Exception exception)
        {
            // NoOp
        }

        public void DebugFormat(string format, params object[] args)
        {
            // NoOp
        }

        public void DebugFormat(string format, object arg0)
        {
            // NoOp
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            // NoOp
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            // NoOp
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            // NoOp
        }

        public void Info(object message)
        {
            // NoOp
        }

        public void Info(object message, Exception exception)
        {
            // NoOp
        }

        public void InfoFormat(string format, params object[] args)
        {
            // NoOp
        }

        public void InfoFormat(string format, object arg0)
        {
            // NoOp
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            // NoOp
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            // NoOp
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            // NoOp
        }

        public void Warn(object message)
        {
            // NoOp
        }

        public void Warn(object message, Exception exception)
        {
            // NoOp
        }

        public void WarnFormat(string format, params object[] args)
        {
            // NoOp
        }

        public void WarnFormat(string format, object arg0)
        {
            // NoOp
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            // NoOp
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            // NoOp
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            // NoOp
        }

        public void Error(object message)
        {
            // NoOp
        }

        public void Error(object message, Exception exception)
        {
            // NoOp
        }

        public void ErrorFormat(string format, params object[] args)
        {
            // NoOp
        }

        public void ErrorFormat(string format, object arg0)
        {
            // NoOp
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            // NoOp
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            // NoOp
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            // NoOp
        }

        public void Fatal(object message)
        {
            // NoOp
        }

        public void Fatal(object message, Exception exception)
        {
            // NoOp
        }

        public void FatalFormat(string format, params object[] args)
        {
            // NoOp
        }

        public void FatalFormat(string format, object arg0)
        {
            // NoOp
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            // NoOp
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            // NoOp
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            // NoOp
        }

        public bool IsDebugEnabled { get; }
        public bool IsInfoEnabled { get; }
        public bool IsWarnEnabled { get; }
        public bool IsErrorEnabled { get; }
        public bool IsFatalEnabled { get; }
    }
}
