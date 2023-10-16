/*
    Copyright 2016, 2017, 2018, 2019, 2023 Alexander Kuperman

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
using TestDataFramework.Populator;
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
        private readonly DbProviderFactory dbProviderFactory;
        private readonly IValueFormatter formatter;
        private readonly bool mustBeInATransaction;
        private readonly DbClientConnection connection;

        protected readonly StringBuilder ExecutionStatements = new StringBuilder();

        protected DbProviderWritePrimitives(
            DbClientConnection connection,
            DbProviderFactory dbProviderFactory,
            IValueFormatter formatter, bool mustBeInATransaction, NameValueCollection configuration)
        {
            DbProviderWritePrimitives.Logger.Debug("Entering constructor");

            this.dbProviderFactory = dbProviderFactory;
            this.formatter = formatter;
            this.mustBeInATransaction = mustBeInATransaction;
            this.configuration = configuration ?? new NameValueCollection();
            this.connection = connection;

            DbProviderWritePrimitives.Logger.Debug("Exiting constructor");
        }

        public void Insert(string catalogueName, string schema, string tableName, IEnumerable<Column> columns)
        {
            DbProviderWritePrimitives.Logger.Debug("Entering Insert");

            columns = columns.ToList();

            DbProviderWritePrimitives.Logger.Debug(
                $"Entering Insert. tableName: {tableName}. schema: {schema ?? "<null>"}. catalogueName: {catalogueName ?? "<null>"} columns: {Helper.ToCompositeString(columns)}");

            string insertStatement = this.BuildInsertStatement(catalogueName, schema, tableName, columns);
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

            if (this.mustBeInATransaction && !Helper.InAmbientTransaction && this.connection.DbTransaction == null)
                throw new NotInATransactionException();

            var result = new List<object>();

            using (DbCommand command = this.dbProviderFactory.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.Connection = this.connection.DbConnection;

                if (this.connection.DbTransaction != null)
                {
                    command.Transaction = this.connection.DbTransaction;
                }

                string commands = this.ExecutionStatements.ToString();

                bool dumpSqlInput = false;
                bool.TryParse(this.configuration["TestDataFramework_DumpSqlInput"], out dumpSqlInput);

                if (dumpSqlInput)
                    LogManager.GetLogger(typeof(DbProviderWritePrimitives)).Debug("\r\n" + commands);

                command.CommandText = commands;

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

            this.ExecutionStatements.Clear();

            DbProviderWritePrimitives.Logger.Debug($"Exiting Execute. result set count: {result.Count}");
            return result.ToArray();
        }

        public abstract object SelectIdentity(string columnName);

        public abstract object WriteGuid(string columnName);

        private string BuildInsertStatement(string catalogueName, string schema, string tableName,
            IEnumerable<Column> columns)
        {
            columns = columns.ToList();
            string parameterList = SqlClientWritePrimitives.BuildParameterListText(columns);
            string valueList = this.BuildValueListText(columns);

            string fullTableName = SqlClientWritePrimitives.BuildFullTableName(catalogueName, schema, tableName);

            string result = $"insert into {fullTableName} " + parameterList + " values " + valueList + ";";
            return result;
        }

        private string BuildValueListText(IEnumerable<Column> columns)
        {
            string result = "(" + string.Join(", ", columns.Select(c => this.formatter.Format(c.Value))) + ")";
            return result;
        }
    }
}