/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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
using System.Data.Common;
using System.Linq;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.Logger;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.ValueFormatter.Interfaces;

namespace TestDataFramework.WritePrimitives.Concrete
{
    public class SqlClientWritePrimitives : DbProviderWritePrimitives
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(SqlClientWritePrimitives));

        private readonly IRandomSymbolStringGenerator symbolGenerator;

        public SqlClientWritePrimitives(DbClientConnection connection,
            DbProviderFactory dbProviderFactory,
            IValueFormatter formatter, IRandomSymbolStringGenerator symbolGenerator, bool mustBeInATransaction,
            NameValueCollection configuration) : base(connection, dbProviderFactory,
            formatter, mustBeInATransaction, configuration)
        {
            SqlClientWritePrimitives.Logger.Debug("Entering constructor");

            this.symbolGenerator = symbolGenerator;

            SqlClientWritePrimitives.Logger.Debug("Exiting constructor");
        }

        public override object SelectIdentity(string columnName)
        {
            SqlClientWritePrimitives.Logger.Debug($"Entering SelectIdentity. columnName: {columnName}");

            string symbol = this.symbolGenerator.GetRandomString(10);
            SqlClientWritePrimitives.Logger.Debug($"symbol: {symbol}");

            this.ExecutionStatements.AppendLine($"declare @{symbol} bigint;");
            this.ExecutionStatements.AppendLine($"select @{symbol} = @@identity;");
            this.ExecutionStatements.AppendLine($"select '{columnName}'");
            this.ExecutionStatements.AppendLine($"select @{symbol}");
            this.ExecutionStatements.AppendLine();

            var result = new Variable(symbol);

            SqlClientWritePrimitives.Logger.Debug($"Exiting SelectIdentity");
            return result;
        }

        public override object WriteGuid(string columnName)
        {
            SqlClientWritePrimitives.Logger.Debug($"Entering WriteGuid. columnName: {columnName}");

            string symbol = this.symbolGenerator.GetRandomString(10);
            SqlClientWritePrimitives.Logger.Debug($"symbol: {symbol}");

            this.ExecutionStatements.AppendLine($"declare @{symbol} uniqueidentifier;");
            this.ExecutionStatements.AppendLine($"select @{symbol} = NEWID();");
            this.ExecutionStatements.AppendLine($"select '{columnName}'");
            this.ExecutionStatements.AppendLine($"select @{symbol}");
            this.ExecutionStatements.AppendLine();

            var result = new Variable(symbol);

            SqlClientWritePrimitives.Logger.Debug("Exiting WriteGuid");
            return result;
        }

        public static string BuildFullTableName(string catalogueName, string schema, string tableName)
        {
            SqlClientWritePrimitives.Logger.Debug("Entering BuildFullTableName");

            string result = $"[{tableName}]";

            if (schema != null)
            {
                result = $"[{schema}]." + result;

                if (catalogueName != null)
                    result = $"[{catalogueName}]." + result;
            }
            else if (catalogueName != null)
            {
                throw new WritePrimitivesException(Messages.CatalogueAndNoSchema, catalogueName, tableName);
            }

            SqlClientWritePrimitives.Logger.Debug("Exiting BuildFullTableName");
            return result;
        }

        public static string BuildParameterListText(IEnumerable<Column> columns)
        {
            SqlClientWritePrimitives.Logger.Debug("Entering BuildParameterListText");

            string result = "(" + string.Join(", ", columns.Select(c => "[" + c.Name + "]")) + ")";

            SqlClientWritePrimitives.Logger.Debug("Exiting BuildParameterListText");
            return result;
        }
    }
}