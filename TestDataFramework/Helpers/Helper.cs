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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.Helpers
{
    public delegate DateTime DateTimeProvider();

    public static class Helper
    {
        private static readonly List<Type> SpecialTypes = new List<Type>
        {
            typeof(Variable)
        };

        public static ulong DefaultInitalCount => 1;

        public static DateTime Now => DateTime.Now;

        public static bool InAmbientTransaction
        {
            get
            {
                Transaction transaction = Transaction.Current;

                return transaction?.TransactionInformation.Status == TransactionStatus.Active;
            }
        }

        public static TableName GetTableName(Type recordType, IAttributeDecorator attributeDecorator)
        {
            IEnumerable<TableAttribute> attrs =
                attributeDecorator.GetCustomAttributes<TableAttribute>(new TypeInfoWrapper(recordType))?.ToList();

            if (attrs == null || !(attrs = attrs.ToList()).Any())
                return new TableName(recordType.Name);

            TableAttribute tableAttribute = attrs.First();

            return new TableName(tableAttribute.CatalogueName, tableAttribute.Schema, tableAttribute.Name);
        }

        public static string GetColumnName(PropertyInfoProxy propertyInfo, IAttributeDecorator attributeDecorator)
        {
            ColumnAttribute columnAttribute = attributeDecorator.GetCustomAttribute<ColumnAttribute>(propertyInfo);

            string result = columnAttribute?.Name ?? propertyInfo.Name;
            return result;
        }

        public static string DumpObject(object objectValue)
        {
            if (objectValue == null)
                return "null reference";

            var sb = new StringBuilder();

            sb.AppendLine(objectValue.GetType().ToString());

            IEnumerable<PropertyInfoProxy> propertyInfos = objectValue.GetType().GetPropertiesHelper();

            foreach (PropertyInfoProxy propertyInfo in propertyInfos)
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

        public static bool IsSpecialType(this object value)
        {
            bool result = Helper.SpecialTypes.Any(st => st.IsInstanceOfType(value));
            return result;
        }

        public static PropertyInfoProxy[] GetPropertiesHelper(this Type type)
        {
            PropertyInfoProxy[] results = new TypeInfoWrapper(type).GetProperties();

            results = results.Where(r => r.CanRead && r.CanWrite && !r.GetIndexParameters().Any()).ToArray();

            return results;
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

        public static string GetExtendedMemberInfoString(this MemberInfoProxy memberInfo)
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

        private static bool IsForeignKeyExplicitlySetToPrimaryKey(
            RecordReference foreignRecordReference, 
            PropertyInfoProxy foreignKeyProperty, 
            RecordReference primaryRecordReference)
        {
            bool result = foreignRecordReference.ExplicitPrimaryKeyRecords.TryGetValue(
                              foreignKeyProperty.Name,
                              out RecordReference pkReferenceInForeignReference) &&
                          pkReferenceInForeignReference == primaryRecordReference;

            return result;
        }

        private static bool IsForeignKeyExplicitlySet(
            RecordReference foreignRecordReference,
            PropertyInfoProxy foreignKeyProperty)
        {
            bool result = foreignRecordReference
                .ExplicitPrimaryKeyRecords.ContainsKey(foreignKeyProperty.Name);

            return result;
        }

        public static bool IsForeignToPrimaryKeyMatch(
            RecordReference foreignRecordReference, 
            PropertyAttribute<ForeignKeyAttribute> fkpa, 
            RecordReference primaryKeyReference, 
            IAttributeDecorator attributeDecorator)
        {
            bool result = Helper.IsForeignKeyExplicitlySetToPrimaryKey(foreignRecordReference, fkpa.PropertyInfoProxy,
                    primaryKeyReference)

                ||

                !Helper.IsForeignKeyExplicitlySet(foreignRecordReference, fkpa.PropertyInfoProxy)

                &&

                primaryKeyReference.RecordType == attributeDecorator.GetPrimaryTableType(fkpa.Attribute,
                    new TypeInfoWrapper(foreignRecordReference.RecordType.GetTypeInfo()));

            return result;
        }

        public static PropertyInfoProxy GetPropertyInfoProxy(this Type type, string propertyName)
        {
            PropertyInfoProxy result = new TypeInfoWrapper(type).GetProperty(propertyName);
            return result;
        }

        public static PropertyInfoProxy[] GetPropertyInfoProxies(this Type type)
        {
            PropertyInfoProxy[] result = new TypeInfoWrapper(type).GetProperties();
            return result;
        }
    }
}