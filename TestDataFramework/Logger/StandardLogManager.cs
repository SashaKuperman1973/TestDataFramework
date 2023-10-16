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