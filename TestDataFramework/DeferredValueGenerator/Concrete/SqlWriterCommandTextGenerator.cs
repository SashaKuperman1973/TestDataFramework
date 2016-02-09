using System.Reflection;
using log4net;
using TestDataFramework.Helpers;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class SqlWriterCommandTextGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlWriterCommandTextGenerator));

        public virtual string WriteString(PropertyInfo propertyInfo)
        {
            SqlWriterCommandTextGenerator.Logger.Debug("Entering WriteString. propertyInfo:" + propertyInfo.GetExtendedMemberInfoString());

            string tableName = "[" + Helper.GetTableName(propertyInfo.DeclaringType) + "]";
            string columnName = "[" + Helper.GetColumnName(propertyInfo) + "]";

            string result =
                $"Select Max({columnName}) from {tableName} where {columnName} not like '%[^A-Z]%' And LEN({columnName}) = (Select Max(Len({columnName})) From {tableName} where {columnName} not like '%[^A-Z]%' )";

            SqlWriterCommandTextGenerator.Logger.Debug($"Result statement: {result}");

            SqlWriterCommandTextGenerator.Logger.Debug("Exiting WriteString");
            return result;
        }

        public virtual string WriteNumber(PropertyInfo propertyInfo)
        {
            SqlWriterCommandTextGenerator.Logger.Debug("Entering WriteNumber. propertyInfo:" + propertyInfo.GetExtendedMemberInfoString());

            string result = $"Select MAX([{Helper.GetColumnName(propertyInfo)}]) From [{Helper.GetTableName(propertyInfo.DeclaringType)}]";

            SqlWriterCommandTextGenerator.Logger.Debug($"Result statement: {result}");

            SqlWriterCommandTextGenerator.Logger.Debug("Exiting WriteNumber");
            return result;
        }
    }
}