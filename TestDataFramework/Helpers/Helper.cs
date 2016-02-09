using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.Helpers
{
    public delegate DateTime DateTimeProvider();

    public static class Helper
    {
        public static ulong DefaultInitalCount => 1;

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

        public static string GetColumnName(PropertyInfo propertyInfo)
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

                sb.Append(propertyInfo.GetExtendedMemberInfoString() + ": " + value + ",");
            }

            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        public static string PrintType(Type type)
        {
            return type.ToString();
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

            results = results.Where(r => r.CanRead && r.CanWrite && !r.GetIndexParameters().Any()).ToArray();

            return results;
        }

        public static bool InAmbientTransaction
        {
            get
            {
                Transaction transaction = Transaction.Current;

                return transaction?.TransactionInformation.Status == TransactionStatus.Active;
            }
        }

        public static bool IsGuid(this Type type)
        {
            bool result = (Nullable.GetUnderlyingType(type) ?? type) ==
                          typeof(Guid);

            return result;
        }

        public static Type GetUnderLyingType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static bool IsValueLikeType(this Type type)
        {
            bool result = type.IsValueType || type == typeof(string);
            return result;
        }

        public static object GetDefaultValue(Type forType)
        {
            return forType.IsValueType
                ? Activator.CreateInstance(forType)
                : null;
        }

        public static string GetExtendedMemberInfoString(this MemberInfo memberInfo)
        {
            return memberInfo + " - " + memberInfo.DeclaringType;
        }

        public static string GetRecordTypesString(this IEnumerable<AbstractRepositoryOperation> recordOperations)
        {
            return string.Join(", ", recordOperations.Select(p => p.RecordReference?.RecordType));
        }

        public static object ToCompositeString(IEnumerable<object> columns)
        {
            return string.Join(" || ", columns);
        }
    }
}
