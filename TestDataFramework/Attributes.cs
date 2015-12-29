using System;
using TestDataFramework.Randomizer;

namespace TestDataFramework
{
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

        public Type PrimaryTableType { get; }

        public string PrimaryKeyName { get; }
    }

    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public class AutoIdentityAttribute : Attribute
    {   
    }

    public class PrimaryKeyAttribute : Attribute
    {
        public enum KeyTypeEnum
        {
            Auto,
            Manual,
            None,
        }

        public PrimaryKeyAttribute(KeyTypeEnum keyType)
        {
            this.KeyType = keyType;
        }

        public KeyTypeEnum KeyType { get; }
    }
}
