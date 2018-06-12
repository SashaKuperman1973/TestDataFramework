/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Logger;

namespace TestDataFramework.AttributeDecorator.Concrete
{
    public class StandardTableTypeCache
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardTableTypeCache));

        internal readonly ConcurrentDictionary<AssemblyWrapper, AssemblyLookupContext> TableTypeDictionary =
            new ConcurrentDictionary<AssemblyWrapper, AssemblyLookupContext>();

        private readonly ITableTypeCacheService tableTypeCacheService;

        public StandardTableTypeCache(ITableTypeCacheService tableTypeCacheService)
        {
            this.tableTypeCacheService = tableTypeCacheService;
        }

        public virtual bool IsAssemblyCachePopulated(AssemblyWrapper assembly)
        {
            StandardTableTypeCache.Logger.Debug("Entering IsAssemblyCachePopulated");
            
            bool result = this.TableTypeDictionary.ContainsKey(assembly);

            StandardTableTypeCache.Logger.Debug("Exiting IsAssemblyCachePopulated");
            return result;
        }

        public virtual TypeInfoWrapper GetCachedTableType(ForeignKeyAttribute foreignKeyAttribute, TypeInfoWrapper foreignType,
            AssemblyWrapper initialAssemblyToScan, Func<TypeInfoWrapper, TableAttribute> getTableAttibute,
            bool canScanAllCachedAssemblies = true)
        {
            StandardTableTypeCache.Logger.Debug("Entering GetCachedTableType");

            AssemblyLookupContext assemblyLookupContext = this.TableTypeDictionary.GetOrAdd(initialAssemblyToScan, a =>
                throw new TableTypeLookupException(Messages.AssemblyCacheNotPopulated, initialAssemblyToScan));

            TableAttribute tableAttribute = getTableAttibute(foreignType);

            TypeInfoWrapper result =
                this.tableTypeCacheService.GetCachedTableType(foreignKeyAttribute, tableAttribute,
                    assemblyLookupContext);

            if (result != null)
            {
                return result;
            }

            result = canScanAllCachedAssemblies
                ? this.GetCachedTableTypeUsingAllAssemblies(foreignKeyAttribute, tableAttribute)
                : null;

            StandardTableTypeCache.Logger.Debug("Exiting GetCachedTableType");
            return result;
        }

        public virtual void PopulateAssemblyCache(AssemblyWrapper assembly, GetTableAttribute getTableAttibute, string defaultSchema)
        {
            StandardTableTypeCache.Logger.Debug("Entering PopulateAssemblyCache");

            AssemblyLookupContext assemblyLookupContext = this.TableTypeDictionary.GetOrAdd(
                assembly,
                a => StandardTableTypeCache.CreateAsemblyLookupContext());

            List<AssemblyNameWrapper> assemblyNameList = assembly.GetReferencedAssemblies().ToList();
            assemblyNameList.Add(assembly.GetName());

            AppDomainWrapper domain = this.tableTypeCacheService.CreateDomain();

            assemblyNameList.ForEach(assemblyName =>
            {
                this.tableTypeCacheService.PopulateAssemblyCache(domain, assemblyName, getTableAttibute, defaultSchema,
                    this.tableTypeCacheService.TryAssociateTypeToTable, assemblyLookupContext);
            });

            domain.Unload();

            StandardTableTypeCache.Logger.Debug("Exiting PopulateAssemblyCache");
        }

        private static AssemblyLookupContext CreateAsemblyLookupContext()
        {
            var typeDictionaryEqualityComparer = new TypeDictionaryEqualityComparer();

            var resultContext = new AssemblyLookupContext
            {
                TypeDictionary = new ConcurrentDictionary<Table, TypeInfoWrapper>(typeDictionaryEqualityComparer),

                CollisionDictionary =
                    new ConcurrentDictionary<Table, List<TypeInfoWrapper>>(typeDictionaryEqualityComparer),

                TypeDictionaryEqualityComparer = typeDictionaryEqualityComparer
            };

            return resultContext;
        }

        private TypeInfoWrapper GetCachedTableTypeUsingAllAssemblies(ForeignKeyAttribute foreignKeyAttribute, TableAttribute tableAttribute)
        {
            StandardTableTypeCache.Logger.Debug("Entering GetCachedTableTypeUsingAllAssemblies");

            foreach (KeyValuePair<AssemblyWrapper, AssemblyLookupContext> tableTypeKvp in this.TableTypeDictionary)
            {
                TypeInfoWrapper result = this.tableTypeCacheService.GetCachedTableType(foreignKeyAttribute, tableAttribute, tableTypeKvp.Value);

                if (result != null)
                {
                    return result;
                }
            }

            StandardTableTypeCache.Logger.Debug("Exiting GetCachedTableTypeUsingAllAssemblies");
            return null;
        }
    }
}
