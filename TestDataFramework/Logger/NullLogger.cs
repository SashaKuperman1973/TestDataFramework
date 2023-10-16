/*
    Copyright 2016, 2017, 2018, 2019, 2023 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

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