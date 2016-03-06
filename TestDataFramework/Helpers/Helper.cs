/*
    Copyright 2016 Alexander Kuperman

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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Transactions;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.Exceptions;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.Helpers
{
    public delegate DateTime DateTimeProvider();

    public static class Helper
    {
        public static ulong DefaultInitalCount => 1;

        public static DateTime Now => DateTime.Now;

        public static TableName GetTableName(Type recordType, IAttributeDecorator attributeDecorator)
        {
            IEnumerable<TableAttribute> attrs =
                attributeDecorator.GetCustomAttributes<TableAttribute>(recordType)?.ToList();

            if (attrs == null || !(attrs = attrs.ToList()).Any())
            {
                return new TableName(recordType.Name);
            }

            TableAttribute tableAttribute = attrs.First();

            return new TableName(tableAttribute.CatalogueName, tableAttribute.Schema, tableAttribute.Name);
        }

        public static string GetColumnName(PropertyInfo propertyInfo, IAttributeDecorator attributeDecorator)
        {
            var columnAttribute = attributeDecorator.GetCustomAttribute<ColumnAttribute>(propertyInfo);

            string result = columnAttribute?.Name ?? propertyInfo.Name;
            return result;
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
            return string.Join(", ", recordOperations.Select(p => p?.RecordReference?.RecordType));
        }

        public static object ToCompositeString(IEnumerable<object> columns)
        {
            return string.Join(" || ", columns);
        }

        public static MemberInfo ValidateFieldExpression<T, TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression)
        {
            if (fieldExpression.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            var memberExpression = fieldExpression.Body as MemberExpression;

            if (memberExpression != null) return memberExpression.Member;

            var unaryExpression = fieldExpression.Body as UnaryExpression;

            if (unaryExpression == null)
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            memberExpression = unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            return memberExpression.Member;
        }
    }
}
