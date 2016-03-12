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
using TestDataFramework.Exceptions;

namespace TestDataFramework.AttributeDecorator
{
    public class TableTypeCache
    {
        private readonly TypeDictionaryEqualityComparer typeDictionaryEqualityComparer =
            new TypeDictionaryEqualityComparer();

        private readonly ConcurrentDictionary<Assembly, AssemblyLookupContext> tableTypeDictionary =
            new ConcurrentDictionary<Assembly, AssemblyLookupContext>();

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
            bool crtieriaTryAddResult =
                this.PerformTypeDictionaryOperation(
                    () => assemblyLookupContext.TypeDictionary.TryAdd(table, definedType));

            if (!crtieriaTryAddResult)
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

        private delegate bool TypeDictionaryOperationDelegate<TResult>(out TResult output);
        private delegate bool TypeDictionaryOperationDelegate();

        private bool PerformTypeDictionaryOperation(TypeDictionaryOperationDelegate typeDictionaryOperation)
        {
            object output;

            bool result = this.PerformTypeDictionaryOperation((out object x) =>
            {
                x = null;
                return typeDictionaryOperation();

            }, out output);

            return result;
        }

        private bool PerformTypeDictionaryOperation<TOut>(TypeDictionaryOperationDelegate<TOut> typeDictionaryOperation, out TOut output)
        {
            this.typeDictionaryEqualityComparer.SetEqualsCriteria(
                (fromSet, input) =>
                    fromSet.HasTableAttribute && fromSet.HasCatalogueName && input.HasTableAttribute &&
                    input.HasCatalogueName);

            // Note: If HasCatlogueName then HasTableAttribute

            var equalsCriteriaSet = new TypeDictionaryEqualityComparer
                .EqualsCriteriaDelegate[]
            {
                (fromSet, input) =>
                    fromSet.HasCatalogueName && input.HasCatalogueName &&
                    fromSet.CatalogueName.Equals(input.CatalogueName),

                // Higher priority because it is explicitly decorated
                (fromSet, input) =>
                    fromSet.HasTableAttribute,

                (fromSet, input) => true,

            };

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

        private Type GetCachedTableType(ForeignKeyAttribute foreignAttribute, TableAttribute tableAttribute, AssemblyLookupContext assemblyLookupContext)
        {
            var table = new Table(foreignAttribute, tableAttribute);

            List<Type> collisionTypes;
            if (
                this.PerformTypeDictionaryOperation(
                    (out List<Type> fnCollisionTypes) =>
                        assemblyLookupContext.CollisionDictionary.TryGetValue(table, out fnCollisionTypes),
                    out collisionTypes))
            {
                throw new TableTypeCacheException(Messages.DuplicateTableName, collisionTypes);
            }

            Type result;
            this.PerformTypeDictionaryOperation((out Type fnResult) => assemblyLookupContext.TypeDictionary.TryGetValue(table, out fnResult), out result);

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
                bool result = this.equalsCriteria(fromSet, input) && fromSet.Equals(input);
                return result;
            }

            public int GetHashCode(Table obj)
            {
                return obj.GetHashCode();
            }
        }
        
    }
}
