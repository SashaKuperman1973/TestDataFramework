using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.WritePrimitives
{
    public class InsertStatementValueFormatter : IValueFormatter
    {
        public string Format(object value)
        {
            if (value == null)
            {
                return null;
            }

            Func<object, string> formatter;
            Type inputType = value.GetType();
            Type type = Nullable.GetUnderlyingType(inputType) ?? inputType;

            if (!InsertStatementValueFormatter.FormatterDictionary.TryGetValue(type, out formatter))
            {
                throw new NotSupportedException($"Insertion doesn't support type <{type}>, value <{value}>.");
            }

            string result = formatter(value);
            return result;
        }

        #region Formatters

        private static readonly Dictionary<Type, Func<object, string>> FormatterDictionary = new Dictionary<Type, Func<object, string>>
        {
            {typeof (int), InsertStatementValueFormatter.IntFormatter},
            {typeof (short), InsertStatementValueFormatter.IntFormatter},
            {typeof (long), InsertStatementValueFormatter.IntFormatter},
            {typeof (string), InsertStatementValueFormatter.StringFormatter},
            {typeof (char), InsertStatementValueFormatter.CharFormatter},
            {typeof (decimal), InsertStatementValueFormatter.DecimalFomatter},
            {typeof (bool), InsertStatementValueFormatter.BoolFormatter},
            {typeof (DateTime), InsertStatementValueFormatter.DateTimeFormatter},
            {typeof (byte), InsertStatementValueFormatter.ByteFormatter}
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
