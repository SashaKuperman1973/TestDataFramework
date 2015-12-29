using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
    }
}
