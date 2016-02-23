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
        public ForeignKeyAttribute(Type primaryTable, string primaryKeyName)
        {
            this.PrimaryTableType = primaryTable;
            this.PrimaryKeyName = primaryKeyName;
        }

        public ForeignKeyAttribute(string primaryTableName, string primaryKeyName)
        {
            this.PrimaryTableName = primaryTableName;
            this.PrimaryKeyName = primaryKeyName;
        }

        public Type PrimaryTableType { get; }

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
        public string Name { get; set; }
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