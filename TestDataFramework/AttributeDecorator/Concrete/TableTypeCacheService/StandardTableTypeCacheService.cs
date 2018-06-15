using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
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

        public virtual TypeInfoWrapper GetCachedTableType(ForeignKeyAttribute foreignAttribute,
            TableAttribute tableAttribute, AssemblyLookupContext assemblyLookupContext)
        {
            StandardTableTypeCacheService.Logger.Debug("Entering GetCachedTableType");

            var table = new Table(foreignAttribute, tableAttribute);

            // The dictionary strategy AND's the externally set input condition 
            // with the result of comparing input and dictionary collection
            // schema and table.

            TypeInfoWrapper result;

            // 1.
            // Test for a complete match
            if ((result = this.tableTypeLookup.GetTableTypeByCriteria(table, TableTypeCriteria.CompleteMatchCriteria,
                    assemblyLookupContext)) != null)
            {
                StandardTableTypeCacheService.Logger.Debug("Complete match found. Exiting GetCachedTableType.");
                return result;
            }

            // 2.
            // !input.HasCataloguName && fromSet.HasCatalogueName
            if ((result = this.tableTypeLookup.GetTableTypeWithCatalogue(table, assemblyLookupContext)) != null)
            {
                StandardTableTypeCacheService.Logger.Debug(
                    "!input.HasCataloguName && fromSet.HasCatalogueName match found. Exiting GetCachedTableType.");
                return result;
            }

            // 3.
            // Match on what's decorated
            if ((result = this.tableTypeLookup.GetTableTypeByCriteria(table, TableTypeCriteria.MatchOnWhatIsDecorated,
                    assemblyLookupContext)) != null)
            {
                StandardTableTypeCacheService.Logger.Debug(
                    "Match on what's decorated found. Exiting GetCachedTableType.");
                return result;
            }

            // 4.
            // Match on everything not already tried
            if ((result = this.tableTypeLookup.GetTableTypeByCriteria(table,
                    TableTypeCriteria.MatchOnEverythingNotAlreadyTried, assemblyLookupContext)) != null)
            {
                StandardTableTypeCacheService.Logger.Debug(
                    "Match on everything not already tried found. Exiting GetCachedTableType.");
                return result;
            }

            StandardTableTypeCacheService.Logger.Debug("No matches found. Exiting GetCachedTableType.");
            return null;
        }

        public virtual AppDomainWrapper CreateDomain()
        {
            AppDomain domain = AppDomain.CreateDomain("TestDataFramework_" + Guid.NewGuid(), null,
                AppDomain.CurrentDomain.SetupInformation);
            var result = new AppDomainWrapper(domain);
            return result;
        }

        public virtual void TryAssociateTypeToTable(TypeInfoWrapper definedType,
            AssemblyLookupContext assemblyLookupContext, GetTableAttribute getTableAttibute,
            string defaultSchema)
        {
            StandardTableTypeCacheService.Logger.Debug("Entering TryAdd");

            TableAttribute tableAttribute = getTableAttibute(definedType);

            Table table = tableAttribute != null
                ? new Table(tableAttribute)
                : new Table(definedType, defaultSchema);

            // Note: If HasCatalogueName then HasTableAttribute

            bool EqualsCriteria(Table fromSet, Table input)
            {
                return fromSet.HasCatalogueName &&
                       fromSet.CatalogueName.Equals(input.CatalogueName) ||
                       fromSet.HasTableAttribute && input.HasTableAttribute &&
                       !fromSet.HasCatalogueName && !input.HasCatalogueName ||
                       !fromSet.HasTableAttribute && !input.HasTableAttribute;
            }

            assemblyLookupContext.TypeDictionaryEqualityComparer.SetEqualsCriteria(EqualsCriteria);

            var tryAddResult = assemblyLookupContext.TypeDictionary.TryAdd(table, definedType);

            if (tryAddResult)
            {
                StandardTableTypeCacheService.Logger.Debug("Exiting TryAdd");
                return;
            }

            StandardTableTypeCacheService.Logger.Debug($"Table class collision detected. Table object: {table}");

            assemblyLookupContext.CollisionDictionary.AddOrUpdate(table,
                
                // Add
                StandardTableTypeCacheService.AddToTypeDictionary(assemblyLookupContext, table, definedType),

                // Update
                // Collision key already exists. Update collision list with newly attempted type.
                (tablePlaceholder, list) =>
                {
                    list.Add(definedType);
                    return list;
                });

            StandardTableTypeCacheService.Logger.Debug("Exiting TryAdd");
        }

        private static IList<TypeInfoWrapper> AddToTypeDictionary(AssemblyLookupContext assemblyLookupContext, Table table, TypeInfoWrapper definedType)
        {
            var result = new List<TypeInfoWrapper>()
            {
                // first item of collision to add to list

                assemblyLookupContext.TypeDictionary.GetOrAdd(table,
                    t => throw new TableTypeCacheException(Messages.ErrorGettingDefinedType, table)),

                // second item of collision to add to list

                definedType
            };

            return result;
        }

        public virtual void PopulateAssemblyCache(AppDomainWrapper domain, AssemblyNameWrapper assemblyName,
            GetTableAttribute getTableAttibute, string defaultSchema,
            TryAssociateTypeToTable tryAssociateTypeToTable,
            AssemblyLookupContext assemblyLookupContext)
        {
            AssemblyWrapper loadedAssembly;

            try
            {
                loadedAssembly = domain.Load(assemblyName);
            }
            catch (FileNotFoundException exception)
            {
                StandardTableTypeCacheService.Logger.Warn(
                    $"TestDataFramework - PopulateAssemblyCache: {exception.Message}");
                return;
            }

            List<TypeInfoWrapper> loadedAssemblyTypes;

            try
            {
                loadedAssemblyTypes = loadedAssembly.DefinedTypes.ToList();
            }
            catch (ReflectionTypeLoadException exception)
            {
                StandardTableTypeCacheService.Logger.Warn(
                    $"TestDataFramework - PopulateAssemblyCache: {exception.Message}");
                return;
            }

            loadedAssemblyTypes.ForEach(definedType => tryAssociateTypeToTable(definedType, assemblyLookupContext,
                getTableAttibute, defaultSchema));
        }

        public virtual TypeInfoWrapper GetCachedTableTypeUsingAllAssemblies(ForeignKeyAttribute foreignKeyAttribute,
            TableAttribute tableAttribute, GetCachedTableType getCachedTableType,
            ConcurrentDictionary<AssemblyWrapper, AssemblyLookupContext> tableTypeDictionary)
        {
            StandardTableTypeCacheService.Logger.Debug("Entering GetCachedTableTypeUsingAllAssemblies");

            foreach (KeyValuePair<AssemblyWrapper, AssemblyLookupContext> tableTypeKvp in tableTypeDictionary)
            {
                TypeInfoWrapper result =
                    getCachedTableType(foreignKeyAttribute, tableAttribute, tableTypeKvp.Value);

                if (result != null)
                    return result;
            }

            StandardTableTypeCacheService.Logger.Debug("Exiting GetCachedTableTypeUsingAllAssemblies");
            return null;
        }
    }
}