using System;

namespace TestDataFramework.RepositoryOperations.Model
{
    public class ColumnSymbol
    {
        public Type TableType { get; set; }
        public string ColumnName { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            string result = $"TableType: {this.TableType}, ColumnName: {this.ColumnName}, Value: {this.Value}";
            return result;
        }
    }
}

