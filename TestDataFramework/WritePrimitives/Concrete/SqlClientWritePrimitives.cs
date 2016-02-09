using System.Collections.Specialized;
using System.Data.Common;
using log4net;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.ValueFormatter.Interfaces;

namespace TestDataFramework.WritePrimitives.Concrete
{
    public class SqlClientWritePrimitives : DbProviderWritePrimitives
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (SqlClientWritePrimitives));

        private readonly IRandomSymbolStringGenerator symbolGenerator;

        public SqlClientWritePrimitives(string connectionStringWithDefaultCatalogue, DbProviderFactory dbProviderFactory,
            IValueFormatter formatter, IRandomSymbolStringGenerator symbolGenerator, bool mustBeInATransaction,
            NameValueCollection configuration) : base(connectionStringWithDefaultCatalogue, dbProviderFactory, formatter, mustBeInATransaction, configuration)
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

            SqlClientWritePrimitives.Logger.Debug($"Exiting WriteGuid");
            return result;
        }
    }  
}
