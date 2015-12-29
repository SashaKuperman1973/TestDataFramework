using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TestDataFramework.Helpers
{
    public delegate DateTime DateTimeProvider();

    public static class Helper
    {
        public static DateTime Now => DateTime.Now;

        public static string GetTableName(Type recordType)
        {
            IEnumerable<TableAttribute> attrs = recordType.GetCustomAttributes<TableAttribute>();

            if (attrs == null || !(attrs = attrs.ToList()).Any())
            {
                return recordType.Name;
            }

            return attrs.First().Name;
        }

        public static string GetColunName(PropertyInfo propertyInfo)
        {
            return propertyInfo.Name;
        }

        public static string DumpObject(object objectValue)
        {
            var sb = new StringBuilder();

            sb.AppendLine(objectValue.GetType().ToString());

            IEnumerable<PropertyInfo> propertyInfos = objectValue.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object value = propertyInfo.GetValue(objectValue);
                string propertyName = propertyInfo.Name;

                sb.AppendLine(propertyName + ": " + value);
            }

            return sb.ToString();
        }

        public static string DumpMethod(Delegate operation)
        {
            return operation.Target.GetType().Name + "." + operation.Method.Name;
        }
    }
}
