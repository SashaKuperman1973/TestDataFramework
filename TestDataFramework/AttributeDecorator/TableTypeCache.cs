/*
    Copyright 2016 Alexander Kuperman

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
using log4net.Util;
using TestDataFramework.Exceptions;

namespace TestDataFramework.AttributeDecorator
{
    public class TableTypeCache
    {
        // TODO: Logging

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
            bool result = this.tableTypeDictionary.ContainsKey(assembly);
            return result;
        }

        public virtual Type GetCachedTableType(ForeignKeyAttribute foreignKeyAttribute, Type foreignType,
            Assembly initialAssemblyToScan, Func<Type, TableAttribute> getTableAttibute,
            bool canScanAllCachedAssemblies = true)
        {
            AssemblyLookupContext assemblyLookupContext = this.tableTypeDictionary.GetOrAdd(initialAssemblyToScan, a =>
            {
                throw new TableTypeLookupException(Messages.AssemblyCacheNotPopulated, initialAssemblyToScan);
            });

            TableAttribute tableAttribute = getTableAttibute(foreignType);

            Type result = this.GetCachedTableType(foreignKeyAttribute, tableAttribute, assemblyLookupContext) ??

                          (canScanAllCachedAssemblies
                              ? this.GetCachedTableTypeUsingAllAssemblies(foreignKeyAttribute, tableAttribute)
                              : null);

            return result;
        }

        public class AssemblyLookupContext
        {
            public ConcurrentDictionary<Table, Type> TypeDictionary { get; set; }
            public ConcurrentDictionary<Table, List<Type>> CollisionDictionary { get; set; }
        }

        public virtual void PopulateAssemblyCache(Assembly assembly, Func<Type, TableAttribute> getTableAttibute, string defaultSchema)
        {
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
                Assembly loadedAssembly = domain.Load(assemblyName);

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
        }

        private Type GetCachedTableTypeUsingAllAssemblies(ForeignKeyAttribute foreignKeyAttribute, TableAttribute tableAttribute)
        {
            foreach (KeyValuePair<Assembly, AssemblyLookupContext> tableTypeKvp in this.tableTypeDictionary)
            {
                Type result = this.GetCachedTableType(foreignKeyAttribute, tableAttribute, tableTypeKvp.Value);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private void TryAdd(Table table, Type definedType, AssemblyLookupContext assemblyLookupContext)
        {
            // Note: If HasCatlogueName then HasTableAttribute

            TypeDictionaryEqualityComparer.EqualsCriteriaDelegate equalsCriteria =
                (fromSet, input) =>

                    fromSet.HasCatalogueName && fromSet.CatalogueName.Equals(input.CatalogueName) ||
                    fromSet.HasTableAttribute && input.HasTableAttribute && !fromSet.HasCatalogueName &&
                    !input.HasCatalogueName ||
                    !fromSet.HasTableAttribute && !input.HasTableAttribute;

            this.typeDictionaryEqualityComparer.SetEqualsCriteria(equalsCriteria);

            bool tryAddResult = assemblyLookupContext.TypeDictionary.TryAdd(table, definedType);

            if (!tryAddResult)
            {
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
            }
        }

        private Type GetCachedTableType(ForeignKeyAttribute foreignAttribute, TableAttribute tableAttribute, AssemblyLookupContext assemblyLookupContext)
        {
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
                return result;
            }

            // 2.
            // !input.HasCataloguName && fromSet.HasCatalogueName
            if ((result = this.GetTableTypeWithCatalogue(table, assemblyLookupContext)) != null)
            {
                return result;
            }

            // 3.
            // Match on what's decorated
            if ((result = this.GetTableTypeByCriteria(table, matchOnWhatIsDecorated, assemblyLookupContext)) != null)
            {
                return result;
            }

            // 4.
            // Match on everything not already tried
            if ((result = this.GetTableTypeByCriteria(table, matchOnEverythingNotAlreadyTried, assemblyLookupContext)) != null)
            {
                return result;
            }

            return null;
        }

        private Type GetTableTypeByCriteria(Table table, TypeDictionaryEqualityComparer.EqualsCriteriaDelegate matchCriteria, AssemblyLookupContext assemblyLookupContext)
        {
            Type result;
            List<Type> collisionTypes;

            this.typeDictionaryEqualityComparer.SetEqualsCriteria(matchCriteria);

            if (assemblyLookupContext.CollisionDictionary.TryGetValue(table, out collisionTypes))
            {
                throw new TableTypeCacheException(Messages.DuplicateTableName, collisionTypes);
            }

            return assemblyLookupContext.TypeDictionary.TryGetValue(table, out result) ? result : null;
        }

        private Type GetTableTypeWithCatalogue(Table table, AssemblyLookupContext assemblyLookupContext)
        {
            if (table.HasCatalogueName) return null;

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
