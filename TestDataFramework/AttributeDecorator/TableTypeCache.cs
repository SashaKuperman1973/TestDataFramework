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

        public virtual Type GetCachedTableType(ForeignKeyAttribute foreignKeyAttribute, Type foreignType, Func<Type, TableAttribute> getTableAttibute)
        {
            AssemblyLookupContext assemblyLookupContext = this.tableTypeDictionary.GetOrAdd(foreignType.Assembly, a =>
            {
                throw new TableTypeLookupException(Messages.AssemblyCacheNotPopulated, foreignType.Assembly);
            });

            TableAttribute tableAttribute = getTableAttibute(foreignType);

            Type result = this.GetCachedTableType(foreignKeyAttribute, tableAttribute, assemblyLookupContext);

            return result;
        }

        public class AssemblyLookupContext
        {
            public ConcurrentDictionary<Table, Type> TypeDictionary { get; set; }
            public ConcurrentDictionary<Table, List<Type>> CollisionDictionary { get; set; }
        }

        public virtual void PopulateAssemblyCache(Assembly assembly, Func<Type, TableAttribute> getTableAttibute)
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
                        : new Table(definedType);

                    this.TryAdd(table, definedType, assemblyLookupContext);
                });
            });

            AppDomain.Unload(domain);
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

            // The rules here are checked in order. First match wins.

            // The dictionary strategy AND's the externally set input condition 
            // with the result of comparing input and dictionary collection
            // schema and table.

            var collisionCriteriaSet = new TypeDictionaryEqualityComparer
                .EqualsCriteriaDelegate[]
            {
                // Match on all properties exist and corresponding properties are equal to each other
                completeMatchCriteria,

                // Consition where !input.HasCataloguName && fromSet.HasCatalogueName is processed below,
                // at this priority.

                // Match on what's decorated
                matchOnWhatIsDecorated,

                // Match on everything not already tried
                matchOnEverythingNotAlreadyTried
            };

            // Check for collision based on above criteria

            List<Type> collisionTypes;

            if (this.PerformTypeDictionaryOperation(
                    
                    (out List<Type> fnCollisionTypes) =>
                        assemblyLookupContext.CollisionDictionary.TryGetValue(table, out fnCollisionTypes),

                    collisionCriteriaSet, 

                    out collisionTypes
                )
            )
            {
                throw new TableTypeCacheException(Messages.DuplicateTableName, collisionTypes);
            }

            Type result;

            // Test for a complete match

            this.typeDictionaryEqualityComparer.SetEqualsCriteria(completeMatchCriteria);
            if (assemblyLookupContext.TypeDictionary.TryGetValue(table, out result))
            {
                return result;
            }

            // !input.HasCataloguName && fromSet.HasCatalogueName
            if (!table.HasCatalogueName)
            {
                this.typeDictionaryEqualityComparer.SetEqualsCriteria((fromSet, input) => fromSet.HasCatalogueName);

                if (assemblyLookupContext.TypeDictionary.TryGetValue(table, out result))
                {
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
            }

            var decoratedAndEverythingElseCriteriaSet = new TypeDictionaryEqualityComparer
                .EqualsCriteriaDelegate[]
            {
                // Match on what's decorated
                matchOnWhatIsDecorated,

                // Match on everything not already tried
                matchOnEverythingNotAlreadyTried
            };

            this.PerformTypeDictionaryOperation(
                (out Type fnResult) => assemblyLookupContext.TypeDictionary.TryGetValue(table, out fnResult),
                decoratedAndEverythingElseCriteriaSet, out result);

            return result;
        }

        private delegate bool TypeDictionaryOperationDelegate<TResult>(out TResult output);
        private delegate bool TypeDictionaryOperationDelegate();

        private bool PerformTypeDictionaryOperation(TypeDictionaryOperationDelegate typeDictionaryOperation, IEnumerable<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate> equalsCriteriaSet)
        {
            object output;

            bool result = this.PerformTypeDictionaryOperation(
                
                (out object x) =>
                {
                    x = null;
                    return typeDictionaryOperation();

                }, 
            
                equalsCriteriaSet, 
            
                out output
            );

            return result;
        }

        private bool PerformTypeDictionaryOperation<TOut>(TypeDictionaryOperationDelegate<TOut> typeDictionaryOperation, IEnumerable<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate> equalsCriteriaSet, out TOut output)
        {
            foreach (TypeDictionaryEqualityComparer.EqualsCriteriaDelegate criteria in equalsCriteriaSet)
            {
                this.typeDictionaryEqualityComparer.SetEqualsCriteria(criteria);

                if (typeDictionaryOperation(out output))
                {
                    return true;
                }
            }

            output = default(TOut);
            return false;
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
