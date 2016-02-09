using System.Collections.Generic;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.WritePrimitives.Interfaces
{
    public interface IWritePrimitives
    {
        void Insert(string tableName, IEnumerable<Column> columns);
        object SelectIdentity(string columnName);
        object[] Execute();
        void AddSqlCommand(string command);
        object WriteGuid(string columnName);
    }
}
