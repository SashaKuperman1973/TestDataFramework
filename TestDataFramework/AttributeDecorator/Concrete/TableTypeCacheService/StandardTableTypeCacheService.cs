using System;
using log4net;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Logger;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService
{
    public class StandardTableTypeCacheService : ITableTypeCacheService
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardTableTypeCacheService));

        private readonly TableTypeLookup tableTypeLookup;

        public StandardTableTypeCacheService(TableTypeLookup tableAttribute)
        {
            this.tableTypeLookup = tableAttribute;
        }

        //public StandardTableTypeCacheService(TableTypeLookup tableTypeLookup)
        //{
        //    this.tableTypeLookup = tableTypeLookup;
        //}

        public virtual Type GetCachedTableType(ForeignKeyAttribute foreignAttribute, TableAttribute tableAttribute, AssemblyLookupContext assemblyLookupContext)
        {
            StandardTableTypeCacheService.Logger.Debug("Entering GetCachedTableType");

            var table = new Table(foreignAttribute, tableAttribute);

            // The dictionary strategy AND's the externally set input condition 
            // with the result of comparing input and dictionary collection
            // schema and table.

            Type result;

            // 1.
            // Test for a complete match
            if ((result = this.tableTypeLookup.GetTableTypeByCriteria(table, TableTypeCriteria.CompleteMatchCriteria, assemblyLookupContext)) != null)
            {
                StandardTableTypeCacheService.Logger.Debug("Complete match found. Exiting GetCachedTableType.");
                return result;
            }

            // 2.
            // !input.HasCataloguName && fromSet.HasCatalogueName
            if ((result = this.tableTypeLookup.GetTableTypeWithCatalogue(table, assemblyLookupContext)) != null)
            {
                StandardTableTypeCacheService.Logger.Debug("!input.HasCataloguName && fromSet.HasCatalogueName match found. Exiting GetCachedTableType.");
                return result;
            }

            // 3.
            // Match on what's decorated
            if ((result = this.tableTypeLookup.GetTableTypeByCriteria(table, TableTypeCriteria.MatchOnWhatIsDecorated, assemblyLookupContext)) != null)
            {
                StandardTableTypeCacheService.Logger.Debug("Match on what's decorated found. Exiting GetCachedTableType.");
                return result;
            }

            // 4.
            // Match on everything not already tried
            if ((result = this.tableTypeLookup.GetTableTypeByCriteria(table, TableTypeCriteria.MatchOnEverythingNotAlreadyTried, assemblyLookupContext)) != null)
            {
                StandardTableTypeCacheService.Logger.Debug("Match on everything not already tried found. Exiting GetCachedTableType.");
                return result;
            }

            StandardTableTypeCacheService.Logger.Debug("No matches found. Exiting GetCachedTableType.");
            return null;
        }

        public virtual TestDataAppDomain CreateDomain()
        {
            AppDomain domain = AppDomain.CreateDomain("TestDataFramework_" + Guid.NewGuid(), null, AppDomain.CurrentDomain.SetupInformation);
            var result = new TestDataAppDomain(domain);
            return result;
        }

        public virtual void UnloadDomain(TestDataAppDomain domain)
        {
            AppDomain.Unload(domain.AppDomain);
        }
    }
}
