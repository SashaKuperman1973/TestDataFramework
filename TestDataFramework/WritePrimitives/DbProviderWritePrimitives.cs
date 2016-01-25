using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using log4net;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.Exceptions;

namespace TestDataFramework.WritePrimitives
{
    public abstract class DbProviderWritePrimitives : IWritePrimitives
    {
        private readonly string connectionStringWithDefaultCatalogue;
        private readonly DbProviderFactory dbProviderFactory;
        private readonly bool mustBeInATransaction;
        private readonly NameValueCollection configuration;
        private readonly IValueFormatter formatter;

        protected StringBuilder ExecutionStatements = new StringBuilder();

        private static readonly ILog Logger = LogManager.GetLogger(typeof(DbProviderWritePrimitives));

        protected DbProviderWritePrimitives(string connectionStringWithDefaultCatalogue, DbProviderFactory dbProviderFactory,
            IValueFormatter formatter, bool mustBeInATransaction, NameValueCollection configuration)
        {
            this.connectionStringWithDefaultCatalogue = connectionStringWithDefaultCatalogue;
            this.dbProviderFactory = dbProviderFactory;
            this.formatter = formatter;
            this.mustBeInATransaction = mustBeInATransaction;
            this.configuration = configuration ?? new NameValueCollection();
        }

        public void Insert(string tableName, IEnumerable<Column> columns)
        {
            DbProviderWritePrimitives.Logger.Debug("Entering Insert");

            string insertStatement = this.BuildInsertStatement(tableName, columns);
            this.ExecutionStatements.AppendLine(insertStatement);
            this.ExecutionStatements.AppendLine();

            DbProviderWritePrimitives.Logger.Debug("Exiting Insert");
        }

        public void AddSqlCommand(string command)
        {
            DbProviderWritePrimitives.Logger.Debug("Entering AddACommand");

            this.ExecutionStatements.AppendLine(command);
            this.ExecutionStatements.AppendLine();

            DbProviderWritePrimitives.Logger.Debug("Exiting AddACommand");
        }

        public object[] Execute()
        {
            DbProviderWritePrimitives.Logger.Debug("Entering Execute");

            if (this.mustBeInATransaction && !Helper.InAmbientTransaction)
            {
                throw new NotInATransactionException();
            }

            var result = new List<object>();

            DbCommand command = this.dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.Connection = this.dbProviderFactory.CreateConnection();
            command.Connection.ConnectionString = this.connectionStringWithDefaultCatalogue;

            string commands = this.ExecutionStatements.ToString();

            bool dumpSqlInput = false;
            bool.TryParse(this.configuration["TestDataFramework_DumpSqlInput"], out dumpSqlInput);

            if (dumpSqlInput)
            {
                DbProviderWritePrimitives.Logger.Debug("\r\n" + commands);
            }

            command.CommandText = commands;

            using (command.Connection)
            {
                command.Connection.Open();
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var row = new object[reader.FieldCount];
                            reader.GetValues(row);
                            result.AddRange(row);
                        }
                        reader.NextResult();
                    }
                }
            }

            this.ExecutionStatements = new StringBuilder();

            DbProviderWritePrimitives.Logger.Debug("Exiting Execute");
            return result.ToArray();
        }

        private string BuildInsertStatement(string tableName, IEnumerable<Column> columns)
        {
            columns = columns.ToList();
            string parameterList = DbProviderWritePrimitives.BuildParameterListText(columns);
            string valueList = this.BuildValueListText(columns);

            string result = $"insert into [{tableName}] " + parameterList + " values " + valueList + ";";
            return result;
        }

        private static string BuildParameterListText(IEnumerable<Column> columns)
        {
            string result = "(" + string.Join(", ", columns.Select(c => "[" + c.Name + "]")) + ")";
            return result;
        }

        private string BuildValueListText(IEnumerable<Column> columns)
        {
            string result = "(" + string.Join(", ", columns.Select(c => this.formatter.Format(c.Value))) + ")";
            return result;
        }

        public abstract object SelectIdentity(string columnName);

        public abstract object WriteGuid(string columnName);
    }
}
