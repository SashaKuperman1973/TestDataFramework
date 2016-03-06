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
        private readonly TypeDictionaryEqualityComparer typeDictionaryEqualityComparer = new TypeDictionaryEqualityComparer();

        private readonly ConcurrentDictionary<Assembly, AssemblyLookupContext> tableTypeDictionary =
            new ConcurrentDictionary<Assembly, AssemblyLookupContext>();

        public virtual bool IsAssemblyCachePopulated(Assembly assembly)
        {
            bool result = this.tableTypeDictionary.ContainsKey(assembly);
            return result;
        }

        public virtual Type GetCachedTableType(ForeignKeyAttribute foreignAttribute, Assembly assembly)
        {
            AssemblyLookupContext assemblyLookupContext = this.tableTypeDictionary.GetOrAdd(assembly, a =>
            {
                throw new TableTypeLookupException(Messages.AssemblyCacheNotPopulated, assembly);
            });

            this.typeDictionaryEqualityComparer.CompareForTableAttribute = true;

            Type result = TableTypeCache.GetCachedTableType(foreignAttribute, assemblyLookupContext);

            this.typeDictionaryEqualityComparer.CompareForTableAttribute = false;

            if (result == null)
            {
                result = TableTypeCache.GetCachedTableType(foreignAttribute, assemblyLookupContext);
            }

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

            foreach (AssemblyName assemblyName in assemblyNameList)
            {
                Assembly loadedAssembly = domain.Load(assemblyName);

                foreach (TypeInfo definedType in loadedAssembly.DefinedTypes)
                {
                    TableAttribute tableAttribute = getTableAttibute(definedType);

                    Table table = tableAttribute != null
                        ? new Table(tableAttribute)
                        : new Table(definedType);

                    TableTypeCache.TryAdd(table, definedType, assemblyLookupContext);
                }
            }

            AppDomain.Unload(domain);
        }

        private static void TryAdd(Table table, Type definedType, AssemblyLookupContext assemblyLookupContext)
        {
            if (!assemblyLookupContext.TypeDictionary.TryAdd(table, definedType))
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

        private static Type GetCachedTableType(ForeignKeyAttribute foreignAttribute, AssemblyLookupContext assemblyLookupContext)
        {
            var table = new Table(foreignAttribute);

            List<Type> collisionTypes;
            if (assemblyLookupContext.CollisionDictionary.TryGetValue(table, out collisionTypes))
            {
                throw new TableTypeCacheException(Messages.DuplicateTableName, collisionTypes);
            }

            Type cachedType;
            Type result = assemblyLookupContext.TypeDictionary.TryGetValue(table, out cachedType) ? cachedType : null;

            return result;
        }

        private class TypeDictionaryEqualityComparer : IEqualityComparer<Table>
        {
            public TypeDictionaryEqualityComparer()
            {
                this.CompareForTableAttribute = false;
            }

            private Func<Table, Table, bool> equalsFunc;

            public bool CompareForTableAttribute
            {
                set
                {
                    if (value)
                    {
                        this.equalsFunc = TypeDictionaryEqualityComparer.EqualsAndHasTableAttribute;
                    }
                    else
                    {
                        this.equalsFunc = TypeDictionaryEqualityComparer.StandardEquals;
                    }
                }

                get { return this.equalsFunc == TypeDictionaryEqualityComparer.EqualsAndHasTableAttribute; }
            }

            private static bool StandardEquals(Table x, Table y)
            {
                bool result = (y.HasTableAttribute == Table.HasTableAttributeEnum.NotSet ||
                               x.HasTableAttribute == y.HasTableAttribute) && x.Equals(y);
                return result;
            }

            private static bool EqualsAndHasTableAttribute(Table x, Table y)
            {
                if (y.HasTableAttribute != Table.HasTableAttributeEnum.NotSet)
                {
                    throw new ApplicationException("EqualsAndHasTableAttribute: Table y.HasTableAttribute should not be specified");
                }

                bool result = x.HasTableAttribute == Table.HasTableAttributeEnum.True && x.Equals(y);
                return result;
            }

            public bool Equals(Table x, Table y)
            {
                return this.equalsFunc(x, y);
            }

            public int GetHashCode(Table obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
