using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.WritePrimitives
{
    public class SqlClientWritePrimitives : DbProviderWritePrimitives
    {
        private readonly IRandomSymbolStringGenerator symbolGenerator;

        public SqlClientWritePrimitives(string connectionStringWithDefaultCatalogue, DbProviderFactory dbProviderFactory,
            IValueFormatter formatter, IRandomSymbolStringGenerator symbolGenerator, bool mustBeInATransaction,
            NameValueCollection configuration) : base(connectionStringWithDefaultCatalogue, dbProviderFactory, formatter, mustBeInATransaction, configuration)
        {
            this.symbolGenerator = symbolGenerator;
        }

        public override object SelectIdentity(string columnName)
        {
            string symbol = this.symbolGenerator.GetRandomString(10);

            this.ExecutionStatements.AppendLine($"declare @{symbol} bigint;");
            this.ExecutionStatements.AppendLine($"select @{symbol} = @@identity;");
            this.ExecutionStatements.AppendLine($"select '{columnName}'");
            this.ExecutionStatements.AppendLine($"select @{symbol}");
            this.ExecutionStatements.AppendLine();

            var result = new Variable(symbol);
            return result;
        }

        public override object WriteGuid(string columnName)
        {
            string symbol = this.symbolGenerator.GetRandomString(10);
            this.ExecutionStatements.AppendLine($"declare @{symbol} uniqueidentifier;");
            this.ExecutionStatements.AppendLine($"select @{symbol} = NEWID();");
            this.ExecutionStatements.AppendLine($"select '{columnName}'");
            this.ExecutionStatements.AppendLine($"select @{symbol}");
            this.ExecutionStatements.AppendLine();

            var result = new Variable(symbol);
            return result;
        }
    }  
}
