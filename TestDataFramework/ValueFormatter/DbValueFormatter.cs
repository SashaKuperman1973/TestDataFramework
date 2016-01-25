﻿using System;
using System.Collections.Generic;
using System.Globalization;
using TestDataFramework.Exceptions;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.ValueFormatter
{
    public abstract class DbValueFormatter : IValueFormatter
    {
        public virtual string Format(object value)
        {
            if (value == null)
            {
                return null;
            }

            Func<object, string> formatter;
            Type inputType = value.GetType();
            Type type = Nullable.GetUnderlyingType(inputType) ?? inputType;

            if (!DbValueFormatter.FormatterDictionary.TryGetValue(type, out formatter))
            {
                throw new NotSupportedException(string.Format(Messages.InsertionDoesNotSupportType, type, value));
            }

            string result = formatter(value);
            return result;
        }

        #region Formatters

        private static readonly Dictionary<Type, Func<object, string>> FormatterDictionary = new Dictionary<Type, Func<object, string>>
        {
            {typeof (int), DbValueFormatter.IntFormatter},
            {typeof (short), DbValueFormatter.IntFormatter},
            {typeof (long), DbValueFormatter.IntFormatter},
            {typeof (uint), DbValueFormatter.IntFormatter},
            {typeof (ushort), DbValueFormatter.IntFormatter},
            {typeof (ulong), DbValueFormatter.IntFormatter},
            {typeof (string), DbValueFormatter.StringFormatter},
            {typeof (char), DbValueFormatter.CharFormatter},
            {typeof (decimal), DbValueFormatter.DecimalFomatter},
            {typeof (double), DbValueFormatter.DoubleFomatter},
            {typeof (bool), DbValueFormatter.BoolFormatter},
            {typeof (DateTime), DbValueFormatter.DateTimeFormatter},
            {typeof (byte), DbValueFormatter.ByteFormatter}
        };

        private static string IntFormatter(object value)
        {
            string result = value.ToString();
            return result;
        }

        private static string StringFormatter(object value)
        {
            string result = "'" + (string)value + "'";
            return result;
        }

        private static string CharFormatter(object value)
        {
            string result = "'" + (char)value + "'";
            return result;
        }

        private static string DecimalFomatter(object value)
        {
            string result = ((decimal)value).ToString(NumberFormatInfo.InvariantInfo);
            return result;
        }

        private static string DoubleFomatter(object value)
        {
            string result = ((double)value).ToString(NumberFormatInfo.InvariantInfo);
            return result;
        }

        private static string BoolFormatter(object value)
        {
            string result = (bool)value ? "1" : "0";
            return result;
        }

        private static string DateTimeFormatter(object value)
        {
            var dateTime = (DateTime)value;
            string result = "'" + dateTime.ToString("s") + "'";

            return result;
        }

        private static string ByteFormatter(object value)
        {
            return ((byte)value).ToString(NumberFormatInfo.InvariantInfo);
        }

        #endregion Formatters

    }
}
