using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using log4net;
using TestDataFramework.Helpers;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.Exceptions;

namespace TestDataFramework.WritePrimitives
{
    public class DbProviderWritePrimitives : IWritePrimitives
    {
        private readonly string connectionStringWithDefaultCatalogue;
        private readonly DbProviderFactory dbProviderFactory;
        private readonly IValueFormatter formatter;
        private readonly IRandomSymbolStringGenerator symbolGenerator;
        private readonly bool mustBeInATransaction;
        private readonly NameValueCollection configuration;

        private StringBuilder executionStatements = new StringBuilder();

        private static readonly ILog Logger = LogManager.GetLogger(typeof(DbProviderWritePrimitives));

        public DbProviderWritePrimitives(string connectionStringWithDefaultCatalogue, DbProviderFactory dbProviderFactory,
            IValueFormatter formatter, IRandomSymbolStringGenerator symbolGenerator, bool mustBeInATransaction, NameValueCollection configuration)
        {
            this.connectionStringWithDefaultCatalogue = connectionStringWithDefaultCatalogue;
            this.dbProviderFactory = dbProviderFactory;
            this.formatter = formatter;
            this.symbolGenerator = symbolGenerator;
            this.mustBeInATransaction = mustBeInATransaction;
            this.configuration = configuration ?? new NameValueCollection();
        }

        public void Reset()
        {
            this.executionStatements = new StringBuilder();
        }

        public void Insert(string tableName, IEnumerable<Column> columns)
        {
            DbProviderWritePrimitives.Logger.Debug("Entering Insert");

            string insertStatement = this.BuildInsertStatement(tableName, columns);
            this.executionStatements.AppendLine(insertStatement);
            this.executionStatements.AppendLine();

            DbProviderWritePrimitives.Logger.Debug("Exiting Insert");
        }

        public object SelectIdentity(string columnName)
        {
            string symbol = this.symbolGenerator.GetRandomString(10);

            this.executionStatements.AppendLine($"declare @{symbol} bigint;");
            this.executionStatements.AppendLine($"select @{symbol} = @@identity;");
            this.executionStatements.AppendLine($"select '{columnName}'");
            this.executionStatements.AppendLine($"select @{symbol}");
            this.executionStatements.AppendLine();

            var result = new Variable(symbol);
            return result;
        }

        public void AddSqlCommand(string command)
        {
            DbProviderWritePrimitives.Logger.Debug("Entering AddACommand");

            this.executionStatements.AppendLine(command);
            this.executionStatements.AppendLine();

            DbProviderWritePrimitives.Logger.Debug("Exiting AddACommand");
        }

        public object WriteGuid(string columnName)
        {
            string symbol = this.symbolGenerator.GetRandomString(10);
            this.executionStatements.AppendLine($"declare @{symbol} uniqueidentifier;");
            this.executionStatements.AppendLine($"select @{symbol} = NEWID();");
            this.executionStatements.AppendLine($"select '{columnName}'");
            this.executionStatements.AppendLine($"select @{symbol}");
            this.executionStatements.AppendLine();

            var result = new Variable(symbol);
            return result;
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

            string commands = this.executionStatements.ToString();

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
            string result = "(" + string.Join(", ", columns.Select(c => this.FormatValue(c.Value))) + ")";
            return result;
        }

        private string FormatValue(object value)
        {
            var variable = value as Variable;

            if (variable != null)
            {
                return "@" + variable.Symbol;
            }

            string result = this.formatter.Format(value);

            return result ?? "null";
        }
    }
}
