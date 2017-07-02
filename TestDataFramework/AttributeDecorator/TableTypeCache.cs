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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Util;
using TestDataFramework.Exceptions;

namespace TestDataFramework.AttributeDecorator
{
    public class TableTypeCache
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TableTypeCache));

        private readonly TypeDictionaryEqualityComparer typeDictionaryEqualityComparer =
            new TypeDictionaryEqualityComparer();

        private readonly ConcurrentDictionary<Assembly, AssemblyLookupContext> tableTypeDictionary =
            new ConcurrentDictionary<Assembly, AssemblyLookupContext>();

        private readonly ITableTypeCacheService tableTypeCacheService;

        public TableTypeCache(Func<TableTypeCache, ITableTypeCacheService> createTableTypeCacheService)
        {
            this.tableTypeCacheService = createTableTypeCacheService(this);
        }

        public virtual bool IsAssemblyCachePopulated(Assembly assembly)
        {
            TableTypeCache.Logger.Debug("Entering IsAssemblyCachePopulated");
            
            bool result = this.tableTypeDictionary.ContainsKey(assembly);

            TableTypeCache.Logger.Debug("Exiting IsAssemblyCachePopulated");
            return result;
        }

        public virtual Type GetCachedTableType(ForeignKeyAttribute foreignKeyAttribute, Type foreignType,
            Assembly initialAssemblyToScan, Func<Type, TableAttribute> getTableAttibute,
            bool canScanAllCachedAssemblies = true)
        {
            TableTypeCache.Logger.Debug("Entering GetCachedTableType");

            AssemblyLookupContext assemblyLookupContext = this.tableTypeDictionary.GetOrAdd(initialAssemblyToScan, a =>
            {
                throw new TableTypeLookupException(Messages.AssemblyCacheNotPopulated, initialAssemblyToScan);
            });

            TableAttribute tableAttribute = getTableAttibute(foreignType);

            Type result = this.GetCachedTableType(foreignKeyAttribute, tableAttribute, assemblyLookupContext) ??

                          (canScanAllCachedAssemblies
                              ? this.GetCachedTableTypeUsingAllAssemblies(foreignKeyAttribute, tableAttribute)
                              : null);

            TableTypeCache.Logger.Debug("Exiting GetCachedTableType");
            return result;
        }

        public class AssemblyLookupContext
        {
            public ConcurrentDictionary<Table, Type> TypeDictionary { get; set; }
            public ConcurrentDictionary<Table, List<Type>> CollisionDictionary { get; set; }
        }

        public virtual void PopulateAssemblyCache(Assembly assembly, Func<Type, TableAttribute> getTableAttibute, string defaultSchema)
        {
            TableTypeCache.Logger.Debug("Entering PopulateAssemblyCache");

            AssemblyLookupContext assemblyLookupContext = this.tableTypeDictionary.AddOrUpdate(assembly,
                new AssemblyLookupContext
                {
                    TypeDictionary = new ConcurrentDictionary<Table, Type>(this.typeDictionaryEqualityComparer),

                    CollisionDictionary =
                        new ConcurrentDictionary<Table, List<Type>>(this.typeDictionaryEqualityComparer)
                },
                (a, context) => context);

            List<AssemblyName> assemblyNameList = assembly.GetReferencedAssemblies().ToList();
            assemblyNameList.Add(assembly.GetName());

            AppDomain domain = AppDomain.CreateDomain("TestDataFramework_" + Guid.NewGuid(), null, AppDomain.CurrentDomain.SetupInformation);

            assemblyNameList.ForEach(assemblyName =>
            {
                Assembly loadedAssembly;

                try
                {
                    loadedAssembly = domain.Load(assemblyName);
                }
                catch (System.IO.FileNotFoundException exception)
                {
                    Logger.Warn($"TestDataFramework - PopulateAssemblyCache: {exception.Message}");
                    return;
                }

                loadedAssembly.DefinedTypes.ToList().ForEach(definedType =>
                {

                    TableAttribute tableAttribute = getTableAttibute(definedType);

                    Table table = tableAttribute != null
                        ? new Table(tableAttribute)
                        : new Table(definedType, defaultSchema);

                    this.TryAdd(table, definedType, assemblyLookupContext);
                });
            });

            AppDomain.Unload(domain);

            TableTypeCache.Logger.Debug("Exiting PopulateAssemblyCache");
        }

        private Type GetCachedTableTypeUsingAllAssemblies(ForeignKeyAttribute foreignKeyAttribute, TableAttribute tableAttribute)
        {
            TableTypeCache.Logger.Debug("Entering GetCachedTableTypeUsingAllAssemblies");

            foreach (KeyValuePair<Assembly, AssemblyLookupContext> tableTypeKvp in this.tableTypeDictionary)
            {
                Type result = this.GetCachedTableType(foreignKeyAttribute, tableAttribute, tableTypeKvp.Value);

                if (result != null)
                {
                    return result;
                }
            }

            TableTypeCache.Logger.Debug("Exiting GetCachedTableTypeUsingAllAssemblies");
            return null;
        }

        private void TryAdd(Table table, Type definedType, AssemblyLookupContext assemblyLookupContext)
        {
            TableTypeCache.Logger.Debug("Entering TryAdd");

            // Note: If HasCatlogueName then HasTableAttribute

            TypeDictionaryEqualityComparer.EqualsCriteriaDelegate equalsCriteria =
                (fromSet, input) =>

                    fromSet.HasCatalogueName && fromSet.CatalogueName.Equals(input.CatalogueName) ||
                    fromSet.HasTableAttribute && input.HasTableAttribute && !fromSet.HasCatalogueName &&
                    !input.HasCatalogueName ||
                    !fromSet.HasTableAttribute && !input.HasTableAttribute;

            this.typeDictionaryEqualityComparer.SetEqualsCriteria(equalsCriteria);

            bool tryAddResult = assemblyLookupContext.TypeDictionary.TryAdd(table, definedType);

            if (tryAddResult)
            {
                TableTypeCache.Logger.Debug("Exiting TryAdd");
                return;
            }

            TableTypeCache.Logger.Debug($"Table class collision detected. Table object: {table}");

            assemblyLookupContext.CollisionDictionary.AddOrUpdate(table, new List<Type>
            {
                // first item of collision to add to list

                assemblyLookupContext.TypeDictionary.GetOrAdd(table,
                    t =>
                    {
                        throw new TableTypeCacheException(Messages.ErrorGettingDefinedType, table);
                    }),

                // second item of collision to add to list

                definedType
            },

                // collision key already exists. update collision list with newly attempted type.
                        
                (tbl, list) =>
                {
                    list.Add(definedType);
                    return list;
                });

            TableTypeCache.Logger.Debug("Exiting TryAdd");
        }

        private Type GetCachedTableType(ForeignKeyAttribute foreignAttribute, TableAttribute tableAttribute, AssemblyLookupContext assemblyLookupContext)
        {
            TableTypeCache.Logger.Debug("Entering GetCachedTableType");

            var table = new Table(foreignAttribute, tableAttribute);

            // Note: If HasCatlogueName then HasTableAttribute

            TypeDictionaryEqualityComparer.EqualsCriteriaDelegate completeMatchCriteria =
                (fromSet, input) =>
                    fromSet.HasCatalogueName && input.HasCatalogueName &&
                    fromSet.CatalogueName.Equals(input.CatalogueName);

            TypeDictionaryEqualityComparer.EqualsCriteriaDelegate matchOnWhatIsDecorated =
                (fromSet, input) =>
                    (!input.HasCatalogueName || !fromSet.HasCatalogueName) && fromSet.HasTableAttribute;

            TypeDictionaryEqualityComparer.EqualsCriteriaDelegate matchOnEverythingNotAlreadyTried =
                (fromSet, input) =>
                    !input.HasCatalogueName || !fromSet.HasCatalogueName;

            // The dictionary strategy AND's the externally set input condition 
            // with the result of comparing input and dictionary collection
            // schema and table.

            Type result;

            // 1.
            // Test for a complete match
            if ((result = this.GetTableTypeByCriteria(table, completeMatchCriteria, assemblyLookupContext)) != null)
            {
                TableTypeCache.Logger.Debug("Complete match found. Exiting GetCachedTableType.");
                return result;
            }

            // 2.
            // !input.HasCataloguName && fromSet.HasCatalogueName
            if ((result = this.GetTableTypeWithCatalogue(table, assemblyLookupContext)) != null)
            {
                TableTypeCache.Logger.Debug("!input.HasCataloguName && fromSet.HasCatalogueName matche found. Exiting GetCachedTableType.");
                return result;
            }

            // 3.
            // Match on what's decorated
            if ((result = this.GetTableTypeByCriteria(table, matchOnWhatIsDecorated, assemblyLookupContext)) != null)
            {
                TableTypeCache.Logger.Debug("Match on what's decorated found. Exiting GetCachedTableType.");
                return result;
            }

            // 4.
            // Match on everything not already tried
            if ((result = this.GetTableTypeByCriteria(table, matchOnEverythingNotAlreadyTried, assemblyLookupContext)) != null)
            {
                TableTypeCache.Logger.Debug("Match on everything not already tried found. Exiting GetCachedTableType.");
                return result;
            }

            TableTypeCache.Logger.Debug("No matches found. Exiting GetCachedTableType.");
            return null;
        }

        private Type GetTableTypeByCriteria(Table table, TypeDictionaryEqualityComparer.EqualsCriteriaDelegate matchCriteria, AssemblyLookupContext assemblyLookupContext)
        {
            TableTypeCache.Logger.Debug("Entering GetTableTypeByCriteria.");

            Type result;
            List<Type> collisionTypes;

            this.typeDictionaryEqualityComparer.SetEqualsCriteria(matchCriteria);

            if (assemblyLookupContext.CollisionDictionary.TryGetValue(table, out collisionTypes))
            {
                throw new TableTypeCacheException(Messages.DuplicateTableName, collisionTypes);
            }

            TableTypeCache.Logger.Debug("Exiting GetTableTypeByCriteria.");
            return assemblyLookupContext.TypeDictionary.TryGetValue(table, out result) ? result : null;
        }

        private Type GetTableTypeWithCatalogue(Table table, AssemblyLookupContext assemblyLookupContext)
        {
            TableTypeCache.Logger.Debug("Entering GetTableTypeWithCatalogue.");

            if (table.HasCatalogueName)
            {
                TableTypeCache.Logger.Debug("Table has no catalogue name. Exiting GetTableTypeWithCatalogue.");
                return null;                
            }

            Type result;

            this.typeDictionaryEqualityComparer.SetEqualsCriteria((fromSet, input) => fromSet.HasCatalogueName);

            if (!assemblyLookupContext.TypeDictionary.TryGetValue(table, out result)) return null;

            // Test for collision where !input.HasCatalogueName and values 
            // match on input but have different catalogue names specified.

            var resultTableAttribute = this.tableTypeCacheService.GetSingleAttribute<TableAttribute>(result);

            this.typeDictionaryEqualityComparer.SetEqualsCriteria(
                (fromSet, input) =>
                    fromSet.HasCatalogueName &&
                    !fromSet.CatalogueName.Equals(resultTableAttribute.CatalogueName)
                );

            Type abmigousConditionType;

            if (assemblyLookupContext.TypeDictionary.TryGetValue(table, out abmigousConditionType))
            {
                throw new TableTypeCacheException(Messages.AmbigousTableSearchConditions, table, result,
                    abmigousConditionType);
            }

            TableTypeCache.Logger.Debug("Exiting GetTableTypeWithCatalogue.");
            return result;
        }

        private class TypeDictionaryEqualityComparer : IEqualityComparer<Table>
        {
            public delegate bool EqualsCriteriaDelegate(Table fromSet, Table input);
            private EqualsCriteriaDelegate equalsCriteria;

            public void SetEqualsCriteria(EqualsCriteriaDelegate equalsCriteria)
            {
                this.equalsCriteria = equalsCriteria;
            }

            public bool Equals(Table fromSet, Table input)
            {
                bool result = this.equalsCriteria(fromSet, input) && fromSet.BasicFieldsEqual(input);
                return result;
            }

            public int GetHashCode(Table obj)
            {
                return obj.GetHashCode();
            }
        }
        
    }
}
