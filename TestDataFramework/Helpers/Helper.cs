using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using TestDataFramework.RepositoryOperations.Model;

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

            IEnumerable<PropertyInfo> propertyInfos = objectValue.GetType().GetPropertiesHelper();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object value = propertyInfo.GetValue(objectValue);
                string propertyName = propertyInfo.Name;

                sb.AppendLine(propertyName + ": " + value);
            }

            return sb.ToString();
        }

        public static string PrintType(Type type)
        {
            return type.ToString();
        }

        public static string DumpMethod(Delegate operation)
        {
            return operation.Target.GetType().Name + "." + operation.Method.Name;
        }

        private static readonly List<Type> SpecialTypes = new List<Type>
        {
            typeof(Variable),
        };

        public static bool IsSpecialType(this object value)
        {
            bool result = Helper.SpecialTypes.Any(st => st.IsInstanceOfType(value));
            return result;
        }

        public static PropertyInfo[] GetPropertiesHelper(this Type type)
        {
            PropertyInfo[] results =
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty |
                                   BindingFlags.SetProperty);

            results = results.Where(r => r.CanRead && r.CanWrite).ToArray();

            return results;
        }

        public static bool InAmbientTransaction
        {
            get
            {
                System.Transactions.Transaction transaction = System.Transactions.Transaction.Current;

                return transaction?.TransactionInformation.Status == TransactionStatus.Active;
            }
        }

        public static ulong DefaultInitalCount => 0;
    }
}
