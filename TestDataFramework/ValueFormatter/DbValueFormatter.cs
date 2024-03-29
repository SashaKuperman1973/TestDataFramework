﻿/*
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
using System.Collections.Generic;
using System.Globalization;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Logger;
using TestDataFramework.ValueFormatter.Interfaces;

namespace TestDataFramework.ValueFormatter
{
    public abstract class DbValueFormatter : IValueFormatter
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(DbValueFormatter));

        public virtual string Format(object value)
        {
            DbValueFormatter.Logger.Debug($"Entering Format. value: {value?.GetType()}");

            if (value == null)
                return null;

            Func<object, string> formatter;
            Type inputType = value.GetType();
            Type type = Nullable.GetUnderlyingType(inputType) ?? inputType;

            if (!DbValueFormatter.FormatterDictionary.TryGetValue(type, out formatter))
                throw new NotSupportedException(string.Format(Messages.InsertionDoesNotSupportType, type, value));

            string result = formatter(value);

            DbValueFormatter.Logger.Debug($"Exiting Format. result: {result}");
            return result;
        }

        #region Formatters

        private static readonly Dictionary<Type, Func<object, string>> FormatterDictionary =
            new Dictionary<Type, Func<object, string>>
            {
                {typeof(int), DbValueFormatter.IntFormatter},
                {typeof(short), DbValueFormatter.IntFormatter},
                {typeof(long), DbValueFormatter.IntFormatter},
                {typeof(uint), DbValueFormatter.IntFormatter},
                {typeof(ushort), DbValueFormatter.IntFormatter},
                {typeof(ulong), DbValueFormatter.IntFormatter},
                {typeof(string), DbValueFormatter.StringFormatter},
                {typeof(char), DbValueFormatter.CharFormatter},
                {typeof(decimal), DbValueFormatter.DecimalFomatter},
                {typeof(double), DbValueFormatter.DoubleFomatter},
                {typeof(float), DbValueFormatter.FloatFomatter},
                {typeof(bool), DbValueFormatter.BoolFormatter},
                {typeof(DateTime), DbValueFormatter.DateTimeFormatter},
                {typeof(DateTimeOffset), DbValueFormatter.DateTimeOffsetFormatter},
                {typeof(byte), DbValueFormatter.ByteFormatter},
                {typeof(Guid), DbValueFormatter.GuidFormatter}
            };

        private static string IntFormatter(object value)
        {
            DbValueFormatter.Logger.Debug("Executing IntFormatter");
            string result = value.ToString();
            return result;
        }

        private static string StringFormatter(object value)
        {
            DbValueFormatter.Logger.Debug("Executing StringFormatter");
            string result = "'" + (string) value + "'";
            return result;
        }

        private static string CharFormatter(object value)
        {
            DbValueFormatter.Logger.Debug("Executing CharFormatter");
            string result = "'" + (char) value + "'";
            return result;
        }

        private static string DecimalFomatter(object value)
        {
            DbValueFormatter.Logger.Debug("Executing DecimalFomatter");
            string result = ((decimal) value).ToString(NumberFormatInfo.InvariantInfo);
            return result;
        }

        private static string DoubleFomatter(object value)
        {
            DbValueFormatter.Logger.Debug("Executing DoubleFomatter");
            string result = ((double) value).ToString(NumberFormatInfo.InvariantInfo);
            return result;
        }

        private static string FloatFomatter(object value)
        {
            DbValueFormatter.Logger.Debug("Executing FloatFomatter");
            string result = ((float) value).ToString(NumberFormatInfo.InvariantInfo);
            return result;
        }

        private static string BoolFormatter(object value)
        {
            DbValueFormatter.Logger.Debug("Executing BoolFormatter");
            string result = (bool) value ? "1" : "0";
            return result;
        }

        private const string DateAndTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff";

        private static string DateTimeFormatter(object value)
        {
            DbValueFormatter.Logger.Debug("Executing DateTimeFormatter");

            var dateTime = (DateTime) value;
            string result = "'" + dateTime.ToString(DbValueFormatter.DateAndTimeFormat) + "'";

            return result;
        }

        private static string DateTimeOffsetFormatter(object value)
        {
            DbValueFormatter.Logger.Debug("Executing DateTimeOffsetFormatter");

            var dateTimeOffset = (DateTimeOffset) value;
            string result = "'" + dateTimeOffset.ToString(DbValueFormatter.DateAndTimeFormat) + "'";

            return result;
        }

        private static string ByteFormatter(object value)
        {
            DbValueFormatter.Logger.Debug("Executing ByteFormatter");
            return ((byte) value).ToString(NumberFormatInfo.InvariantInfo);
        }

        private static string GuidFormatter(object value)
        {
            DbValueFormatter.Logger.Debug("Executing GuidFormatter");
            return $"'{value}'";
        }

        #endregion Formatters
    }
}