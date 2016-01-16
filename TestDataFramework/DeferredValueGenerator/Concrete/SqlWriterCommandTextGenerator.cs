using System;
using System.Reflection;
using TestDataFramework.Helpers;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class SqlWriterCommandTextGenerator
    {
        public virtual string WriteString(PropertyInfo propertyInfo)
        {
            string tableName = "[" + Helper.GetTableName(propertyInfo.DeclaringType) + "]";
            string columnName = "[" + Helper.GetColunName(propertyInfo) + "]";

            string result =
                $"Select Max({columnName}) from {tableName} where {columnName} not like '%[^A-Z]%' And LEN({columnName}) = (Select Max(Len({columnName})) From {tableName} where {columnName} not like '%[^A-Z]%' )";

            return result;
        }

        public virtual string WriteNumber(PropertyInfo propertyInfo)
        {
            string result = $"Select MAX([{Helper.GetColunName(propertyInfo)}]) From [{Helper.GetTableName(propertyInfo.DeclaringType)}]";
            return result;
        }
    }
}