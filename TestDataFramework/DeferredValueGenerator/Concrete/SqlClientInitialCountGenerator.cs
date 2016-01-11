using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class SqlClientInitialCountGenerator : IDeferredValueGeneratorHandler<ulong>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlClientInitialCountGenerator));

        private LetterEncoder encoder;

        public SqlClientInitialCountGenerator(LetterEncoder encoder)
        {
            this.encoder = encoder;
        }

        public ulong NumberHandler(PropertyInfo propertyInfo, DbCommand command)
        {
            SqlClientInitialCountGenerator.Logger.Debug("Entering NumberHandler");

            string commandText = $"Select MAX([{Helper.GetColunName(propertyInfo)}]) From [{Helper.GetTableName(propertyInfo.DeclaringType)}]";
            command.CommandText = commandText;

            ulong result = Helper.DefaultInitalCount;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    object value = reader.GetValue(0);

                    if (value == DBNull.Value)
                    {
                        SqlClientInitialCountGenerator.Logger.Debug("Row not found. DBNull returned.");
                    }
                    else
                    {
                        SqlClientInitialCountGenerator.Logger.Debug("Row found");

                        if (
                            !new[] {typeof (byte), typeof (int), typeof (short), typeof (long)}.Contains(value.GetType()))
                        {
                            throw new UnexpectedTypeException(string.Format(Messages.UnexpectedHandlerType, propertyInfo,
                                value.GetType()));
                        }

                        result = (ulong) Convert.ChangeType(value, typeof (ulong)) + 1;
                    }
                }
            }

            SqlClientInitialCountGenerator.Logger.Debug("Exiting NumberHandler");
            return result;
        }

        public ulong StringHandler(PropertyInfo propertyInfo, DbCommand command)
        {
            SqlClientInitialCountGenerator.Logger.Debug("Entering StringHandler");

            string tableName = "[" + Helper.GetTableName(propertyInfo.DeclaringType) + "]";
            string columnName = "[" + Helper.GetColunName(propertyInfo) + "]";

            string commandText =
                $"Select Max({columnName}) from {tableName} where {columnName} not like '%[^A-Z]%' And LEN({columnName}) = (Select Max(Len({columnName})) From {tableName} where {columnName} not like '%[^A-Z]%' )";

            command.CommandText = commandText;

            ulong result = Helper.DefaultInitalCount;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    object value = reader.GetValue(0);

                    if (value == DBNull.Value)
                    {
                        SqlClientInitialCountGenerator.Logger.Debug("Row not found. DBNull returned.");
                    }
                    else
                    {
                        SqlClientInitialCountGenerator.Logger.Debug("Row found");

                        if (value.GetType() != typeof (string))
                        {
                            throw new UnexpectedTypeException(string.Format(Messages.UnexpectedHandlerType, propertyInfo,
                                value.GetType()));
                        }

                        result = this.encoder.Decode((string) value) + 1;
                    }
                }
            }

            SqlClientInitialCountGenerator.Logger.Debug("Exiting StringHandler");
            return result;
        }
    }
}
