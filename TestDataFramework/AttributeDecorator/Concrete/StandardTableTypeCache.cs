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
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Logger;

namespace TestDataFramework.AttributeDecorator.Concrete
{
    public class StandardTableTypeCache
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardTableTypeCache));

        private readonly ConcurrentDictionary<Assembly, AssemblyLookupContext> tableTypeDictionary =
            new ConcurrentDictionary<Assembly, AssemblyLookupContext>();

        private readonly ITableTypeCacheService tableTypeCacheService;

        public StandardTableTypeCache(ITableTypeCacheService tableTypeCacheService)
        {
            this.tableTypeCacheService = tableTypeCacheService;
        }

        public virtual bool IsAssemblyCachePopulated(Assembly assembly)
        {
            StandardTableTypeCache.Logger.Debug("Entering IsAssemblyCachePopulated");
            
            bool result = this.tableTypeDictionary.ContainsKey(assembly);

            StandardTableTypeCache.Logger.Debug("Exiting IsAssemblyCachePopulated");
            return result;
        }

        public virtual Type GetCachedTableType(ForeignKeyAttribute foreignKeyAttribute, Type foreignType,
            Assembly initialAssemblyToScan, Func<Type, TableAttribute> getTableAttibute,
            bool canScanAllCachedAssemblies = true)
        {
            StandardTableTypeCache.Logger.Debug("Entering GetCachedTableType");

            AssemblyLookupContext assemblyLookupContext = this.tableTypeDictionary.GetOrAdd(initialAssemblyToScan, a =>
            {
                throw new TableTypeLookupException(Messages.AssemblyCacheNotPopulated, initialAssemblyToScan);
            });

            TableAttribute tableAttribute = getTableAttibute(foreignType);

            Type result = this.tableTypeCacheService.GetCachedTableType(foreignKeyAttribute, tableAttribute, assemblyLookupContext) ??

                          (canScanAllCachedAssemblies
                              ? this.GetCachedTableTypeUsingAllAssemblies(foreignKeyAttribute, tableAttribute)
                              : null);

            StandardTableTypeCache.Logger.Debug("Exiting GetCachedTableType");
            return result;
        }

        public virtual void PopulateAssemblyCache(Assembly assembly, Func<TestDataTypeInfo, TableAttribute> getTableAttibute, string defaultSchema)
        {
            StandardTableTypeCache.Logger.Debug("Entering PopulateAssemblyCache");

            AssemblyLookupContext assemblyLookupContext = this.tableTypeDictionary.AddOrUpdate(assembly, a =>
                {
                    var typeDictionaryEqualityComparer = new TypeDictionaryEqualityComparer();

                    var resultContext = new AssemblyLookupContext
                    {
                        TypeDictionary = new ConcurrentDictionary<Table, TestDataTypeInfo>(typeDictionaryEqualityComparer),

                        CollisionDictionary =
                            new ConcurrentDictionary<Table, List<TestDataTypeInfo>>(typeDictionaryEqualityComparer),

                        TypeDictionaryEqualityComparer = typeDictionaryEqualityComparer
                    };

                    return resultContext;
                },
                (a, context) => context);

            List<AssemblyName> assemblyNameList = assembly.GetReferencedAssemblies().ToList();
            assemblyNameList.Add(assembly.GetName());

            TestDataAppDomain domain = this.tableTypeCacheService.CreateDomain();

            assemblyNameList.ForEach(assemblyName =>
            {
                TestDataAssembly loadedAssembly;

                try
                {
                    loadedAssembly = domain.Load(assemblyName);
                }
                catch (System.IO.FileNotFoundException exception)
                {
                    StandardTableTypeCache.Logger.Warn($"TestDataFramework - PopulateAssemblyCache: {exception.Message}");
                    return;
                }

                List<TestDataTypeInfo> loadedAssemblyTypes;

                try
                {
                    loadedAssemblyTypes = loadedAssembly.DefinedTypes.ToList();
                }
                catch (ReflectionTypeLoadException exception)
                {
                    StandardTableTypeCache.Logger.Warn($"TestDataFramework - PopulateAssemblyCache: {exception.Message}");
                    return;
                }

                loadedAssemblyTypes.ForEach(definedType =>
                {

                    TableAttribute tableAttribute = getTableAttibute(definedType);

                    Table table = tableAttribute != null
                        ? new Table(tableAttribute)
                        : new Table(definedType, defaultSchema);

                    this.TryAdd(table, definedType, assemblyLookupContext);
                });
            });

            this.tableTypeCacheService.UnloadDomain(domain);

            StandardTableTypeCache.Logger.Debug("Exiting PopulateAssemblyCache");
        }

        private Type GetCachedTableTypeUsingAllAssemblies(ForeignKeyAttribute foreignKeyAttribute, TableAttribute tableAttribute)
        {
            StandardTableTypeCache.Logger.Debug("Entering GetCachedTableTypeUsingAllAssemblies");

            foreach (KeyValuePair<Assembly, AssemblyLookupContext> tableTypeKvp in this.tableTypeDictionary)
            {
                Type result = this.tableTypeCacheService.GetCachedTableType(foreignKeyAttribute, tableAttribute, tableTypeKvp.Value);

                if (result != null)
                {
                    return result;
                }
            }

            StandardTableTypeCache.Logger.Debug("Exiting GetCachedTableTypeUsingAllAssemblies");
            return null;
        }

        private void TryAdd(Table table, TestDataTypeInfo definedType, AssemblyLookupContext assemblyLookupContext)
        {
            StandardTableTypeCache.Logger.Debug("Entering TryAdd");

            // Note: If HasCatlogueName then HasTableAttribute

            TypeDictionaryEqualityComparer.EqualsCriteriaDelegate equalsCriteria =
                (fromSet, input) =>

                    fromSet.HasCatalogueName && fromSet.CatalogueName.Equals(input.CatalogueName) ||
                    fromSet.HasTableAttribute && input.HasTableAttribute && !fromSet.HasCatalogueName &&
                    !input.HasCatalogueName ||
                    !fromSet.HasTableAttribute && !input.HasTableAttribute;

            assemblyLookupContext.TypeDictionaryEqualityComparer.SetEqualsCriteria(equalsCriteria);

            bool tryAddResult = assemblyLookupContext.TypeDictionary.TryAdd(table, definedType);

            if (tryAddResult)
            {
                StandardTableTypeCache.Logger.Debug("Exiting TryAdd");
                return;
            }

            StandardTableTypeCache.Logger.Debug($"Table class collision detected. Table object: {table}");

            assemblyLookupContext.CollisionDictionary.AddOrUpdate(table, new List<TestDataTypeInfo>
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

            StandardTableTypeCache.Logger.Debug("Exiting TryAdd");
        }
    }
}
