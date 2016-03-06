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
using System.Linq;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.ValueProvider.Interfaces;

namespace TestDataFramework
{
    internal class AttributeHelper
    {
        public static object[] GetStrings(params object[] inputs)
        {
            object[] result = inputs.Select(i => i?.ToString() ?? "<null>").ToArray();
            return result;
        }
    }

    public class StringLengthAttribute : Attribute
    {
        public StringLengthAttribute(int length)
        {
            this.Length = length;
        }

        public int Length { get; }
    }

    public class PrecisionAttribute : Attribute
    {
        public PrecisionAttribute(int precision)
        {
            this.Precision = precision;
        }

        public int Precision { get; }
    }

    public class EmailAttribute : Attribute
    {
    }

    public class MaxAttribute : Attribute
    {
        public MaxAttribute(long max)
        {
            this.Max = max;
        }

        public long Max { get; }
    }

    public class PastOrFutureAttribute : Attribute
    {
        public PastOrFutureAttribute(PastOrFuture pastOrFuture)
        {
            this.PastOrFuture = pastOrFuture;
        }

        public PastOrFuture PastOrFuture { get; }
    }

    public class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute(Type primaryTableType, string primaryKeyName)
        {
            primaryTableType.IsNotNull(nameof(primaryTableType));

            this.PrimaryTableName = primaryTableType.Name;
            this.PrimaryTableType = primaryTableType;
            this.PrimaryKeyName = primaryKeyName;
        }

        /// <summary>
        /// This overload will set the schema to dbo.
        /// </summary>
        public ForeignKeyAttribute(string primaryTableName, string primaryKeyName)
        {
            this.PrimaryTableName = primaryTableName;
            this.PrimaryKeyName = primaryKeyName;
        }

        public ForeignKeyAttribute(string schema, string primaryTableName, string primaryKeyName)
        {
            this.PrimaryTableName = primaryTableName;
            this.PrimaryKeyName = primaryKeyName;
            this.Schema = schema;
        }

        public Type PrimaryTableType { get; }
        public string Schema { get; } = "dbo";
        public string PrimaryTableName { get; }

        public string PrimaryKeyName { get; }

        public override string ToString()
        {
            string result =
                string.Format("PrimaryTableType: {0}, PrimaryTableName: {1}, PrimaryKeyName: {2}",
                    AttributeHelper.GetStrings(this.PrimaryTableType, this.PrimaryTableName, this.PrimaryKeyName));

            return result;
        }
    }

    public class TableAttribute : Attribute
    {
        public TableAttribute(string catalogueName, string schema, string name)
        {
            name.IsNotNull(nameof(name));

            if (catalogueName != null && schema == null)
            {
                throw new TableAttributeException(Messages.CatalogueAndNoSchema, catalogueName, name);
            }

            this.CatalogueName = catalogueName;
            this.Name = name;
            this.Schema = schema;
        }

        public TableAttribute(string schema, string name)
        {
            name.IsNotNull(nameof(name));

            this.Name = name;
            this.Schema = schema;
        }

        public TableAttribute(string name)
        {
            name.IsNotNull(nameof(name));
            this.Name = name;
        }

        public string CatalogueName { get; }
        public string Name { get; }
        public string Schema { get; } = "dbo";

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ (this.Schema?.GetHashCode() ?? 0) ^
                   (this.CatalogueName?.GetHashCode() ?? 0);
        }

        public override bool Equals(object obj)
        {
            var value = obj as TableAttribute;

            bool result = value != null && 

                (this.Schema == null && value.Schema == null ||
                    (this.Schema?.Equals(value.Schema) ?? false)) &&
                
                    (this.CatalogueName == null && value.CatalogueName == null ||
                    (this.CatalogueName?.Equals(value.CatalogueName) ?? false)) &&

                this.Name.Equals(value.Name);
            return result;
        }

        public override string ToString()
        {
            string result = $"Name: {this.Name}, Schema: {this.Schema ?? "<null>"}";
            return result;
        }
    }

    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public class PrimaryKeyAttribute : Attribute
    {
        public enum KeyTypeEnum
        {
            Auto,
            Manual,
            None,
        }

        public PrimaryKeyAttribute(KeyTypeEnum keyType = KeyTypeEnum.Manual)
        {
            this.KeyType = keyType;
        }

        public KeyTypeEnum KeyType { get; set; }
    }
}