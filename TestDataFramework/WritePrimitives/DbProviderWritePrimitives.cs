/*
    Copyright 2016, 2017 Alexander Kuperman

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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.ValueFormatter.Interfaces;
using TestDataFramework.WritePrimitives.Concrete;
using TestDataFramework.WritePrimitives.Interfaces;

namespace TestDataFramework.WritePrimitives
{
    public abstract class DbProviderWritePrimitives : IWritePrimitives
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(DbProviderWritePrimitives));
        private readonly NameValueCollection configuration;
        private readonly string connectionStringWithDefaultCatalogue;
        private readonly DbProviderFactory dbProviderFactory;
        private readonly IValueFormatter formatter;
        private readonly bool mustBeInATransaction;

        protected StringBuilder ExecutionStatements = new StringBuilder();

        protected DbProviderWritePrimitives(string connectionStringWithDefaultCatalogue,
            DbProviderFactory dbProviderFactory,
            IValueFormatter formatter, bool mustBeInATransaction, NameValueCollection configuration)
        {
            DbProviderWritePrimitives.Logger.Debug("Entering constructor");

            this.connectionStringWithDefaultCatalogue = connectionStringWithDefaultCatalogue;
            this.dbProviderFactory = dbProviderFactory;
            this.formatter = formatter;
            this.mustBeInATransaction = mustBeInATransaction;
            this.configuration = configuration ?? new NameValueCollection();

            DbProviderWritePrimitives.Logger.Debug("Exiting constructor");
        }

        public void Insert(string catalogueName, string schema, string tableName, IEnumerable<Column> columns)
        {
            DbProviderWritePrimitives.Logger.Debug("Entering Insert");

            columns = columns.ToList();

            DbProviderWritePrimitives.Logger.Debug(
                $"Entering Insert. tableName: {tableName}. schema: {schema ?? "<null>"}. catalogueName: {catalogueName ?? "<null>"} columns: {Helper.ToCompositeString(columns)}");

            var insertStatement = this.BuildInsertStatement(catalogueName, schema, tableName, columns);
            DbProviderWritePrimitives.Logger.Debug($"insertStatement: {insertStatement}");

            this.ExecutionStatements.AppendLine(insertStatement);
            this.ExecutionStatements.AppendLine();

            DbProviderWritePrimitives.Logger.Debug("Exiting Insert");
        }

        public void AddSqlCommand(string command)
        {
            DbProviderWritePrimitives.Logger.Debug($"Entering AddACommand. command: {command}");

            this.ExecutionStatements.AppendLine(command);
            this.ExecutionStatements.AppendLine();

            DbProviderWritePrimitives.Logger.Debug("Exiting AddACommand");
        }

        public object[] Execute()
        {
            DbProviderWritePrimitives.Logger.Debug("Entering Execute");

            if (this.mustBeInATransaction && !Helper.InAmbientTransaction)
                throw new NotInATransactionException();

            var result = new List<object>();

            DbCommand command = this.dbProviderFactory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.Connection = this.dbProviderFactory.CreateConnection();
            command.Connection.ConnectionString = this.connectionStringWithDefaultCatalogue;

            var commands = this.ExecutionStatements.ToString();

            var dumpSqlInput = false;
            bool.TryParse(this.configuration["TestDataFramework_DumpSqlInput"], out dumpSqlInput);

            if (dumpSqlInput)
                LogManager.GetLogger(typeof(DbProviderWritePrimitives)).Debug("\r\n" + commands);

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

            DbProviderWritePrimitives.Logger.Debug($"Exiting Execute. result set count: {result.Count}");
            return result.ToArray();
        }

        public abstract object SelectIdentity(string columnName);

        public abstract object WriteGuid(string columnName);

        private string BuildInsertStatement(string catalogueName, string schema, string tableName,
            IEnumerable<Column> columns)
        {
            columns = columns.ToList();
            var parameterList = SqlClientWritePrimitives.BuildParameterListText(columns);
            var valueList = this.BuildValueListText(columns);

            var fullTableName = SqlClientWritePrimitives.BuildFullTableName(catalogueName, schema, tableName);

            var result = $"insert into {fullTableName} " + parameterList + " values " + valueList + ";";
            return result;
        }

        private string BuildValueListText(IEnumerable<Column> columns)
        {
            var result = "(" + string.Join(", ", columns.Select(c => this.formatter.Format(c.Value))) + ")";
            return result;
        }
    }
}