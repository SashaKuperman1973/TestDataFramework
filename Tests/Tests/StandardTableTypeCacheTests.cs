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

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;

namespace Tests.Tests
{
    [TestClass]
    public class StandardTableTypeCacheTests
    {
        private StandardTableTypeCache tableTypeCache;

        private Mock<ITableTypeCacheService> tableTypeCacheServiceMock;

        [TestInitialize]
        public void Initialize()
        {
            this.tableTypeCacheServiceMock = new Mock<ITableTypeCacheService>();
            this.tableTypeCache = new StandardTableTypeCache(this.tableTypeCacheServiceMock.Object);
        }

        [TestMethod]
        public void IsAssemblyCachePopulated_No_Test()
        {
            // Act

            var result = this.tableTypeCache.IsAssemblyCachePopulated(new AssemblyWrapper());

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsAssemblyCachePopulated_Yes_Test()
        {
            // Arrange

            var assembly = new AssemblyWrapper();
            this.tableTypeCache.TableTypeDictionary.TryAdd(assembly, new AssemblyLookupContext());

            // Act

            var result = this.tableTypeCache.IsAssemblyCachePopulated(assembly);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetCachedTableType_AssembyCacheNotPopulated_Test()
        {
            // Act

            var initialAssemblyToScan = new AssemblyWrapper();

            Helpers.ExceptionTest(
                () => this.tableTypeCache.GetCachedTableType(null, null, initialAssemblyToScan, null),
                typeof(TableTypeLookupException),
                string.Format(Messages.AssemblyCacheNotPopulated, initialAssemblyToScan));
        }

        [TestMethod]
        public void GetCachedTableType_Test()
        {
            TypeInfoWrapper result;

            var computedResultFromInitialAssemblyToScan = new TypeInfoWrapper();

            result = this.GetCachedTableType_Test(false, computedResultFromInitialAssemblyToScan);
            Assert.AreEqual(computedResultFromInitialAssemblyToScan, result);
            this.tableTypeCacheServiceMock.VerifyNoOtherCalls();
            this.tableTypeCacheServiceMock.ResetCalls();

            result = this.GetCachedTableType_Test(true, computedResultFromInitialAssemblyToScan);
            Assert.AreEqual(computedResultFromInitialAssemblyToScan, result);
            this.tableTypeCacheServiceMock.VerifyNoOtherCalls();
            this.tableTypeCacheServiceMock.ResetCalls();

            var computedResultFromAllAssemblies = new TypeInfoWrapper();

            result = this.GetCachedTableType_Test(true, null, computedResultFromAllAssemblies);
            Assert.AreEqual(computedResultFromAllAssemblies, result);
            this.tableTypeCacheServiceMock.Verify();
            this.tableTypeCacheServiceMock.ResetCalls();

            result = this.GetCachedTableType_Test(false, null, computedResultFromAllAssemblies);
            Assert.IsNull(result);
            this.tableTypeCacheServiceMock.VerifyNoOtherCalls();
            this.tableTypeCacheServiceMock.ResetCalls();
        }

        private TypeInfoWrapper GetCachedTableType_Test(bool canScanAllAssemblies, TypeInfoWrapper computedResultFromInitialAssemblyToScan,
            TypeInfoWrapper computedResultFromAllAssemblies = null)
        {
            // Arrange

            var initialAssemblyToScan = new AssemblyWrapper();
            var assemblyLookupContext = new AssemblyLookupContext();

            this.tableTypeCache.TableTypeDictionary.TryAdd(initialAssemblyToScan, assemblyLookupContext);

            var foreignKeyAttribute = new ForeignKeyAttribute("tableName", "keyName");
            var tableAttribute = new TableAttribute("name");
            var foreignType = new TypeInfoWrapper();
            var getTableAttibuteMock = new Mock<GetTableAttribute>();

            getTableAttibuteMock.Setup(m => m(foreignType)).Returns(tableAttribute);

            this.tableTypeCacheServiceMock.Setup(
                    m => m.GetCachedTableType(foreignKeyAttribute, tableAttribute, assemblyLookupContext))
                .Returns(computedResultFromInitialAssemblyToScan);

            this.tableTypeCacheServiceMock.Setup(m => m.GetCachedTableTypeUsingAllAssemblies(foreignKeyAttribute,
                tableAttribute, this.tableTypeCacheServiceMock.Object.GetCachedTableType,
                this.tableTypeCache.TableTypeDictionary)).Returns(computedResultFromAllAssemblies).Verifiable();

            // Act

            TypeInfoWrapper result = this.tableTypeCache.GetCachedTableType(foreignKeyAttribute, foreignType,
                initialAssemblyToScan,
                getTableAttibuteMock.Object, canScanAllAssemblies);

            this.tableTypeCacheServiceMock.Verify(
                m => m.GetCachedTableType(foreignKeyAttribute, tableAttribute, assemblyLookupContext));

            return result;
        }

        [TestMethod]
        public void PopulateAssemblyCache_Test()
        {
            // Arrange

            var assemblyMock = new Mock<AssemblyWrapper>();
            GetTableAttribute getTableAttribute = type => null;
            string defaultSchema = "DefaultSchema";

            var assemblyNames = new[]
                {new AssemblyNameWrapper(), new AssemblyNameWrapper(), new AssemblyNameWrapper()};

            assemblyMock.Setup(m => m.GetReferencedAssemblies())
                .Returns(assemblyNames.Take(2).ToArray());

            assemblyMock.Setup(m => m.GetName()).Returns(assemblyNames.Skip(2).Take(1).First());

            var domainMock = new Mock<AppDomainWrapper>();

            this.tableTypeCacheServiceMock.Setup(m => m.CreateDomain()).Returns(domainMock.Object);

            // Act

            this.tableTypeCache.PopulateAssemblyCache(assemblyMock.Object, getTableAttribute, defaultSchema);

            // Assert

            KeyValuePair<AssemblyWrapper, AssemblyLookupContext> typeDictionaryEntry = this.tableTypeCache.TableTypeDictionary.Single();
            Assert.AreEqual(assemblyMock.Object, typeDictionaryEntry.Key);
            Assert.IsInstanceOfType(typeDictionaryEntry.Value, typeof(AssemblyLookupContext));

            assemblyNames.ToList()
                .ForEach(assemblyName => this.tableTypeCacheServiceMock.Verify(m => m.PopulateAssemblyCache(domainMock.Object,
                    assemblyName, getTableAttribute, defaultSchema,
                    this.tableTypeCacheServiceMock.Object.TryAssociateTypeToTable, typeDictionaryEntry.Value)));

            domainMock.Verify(m => m.Unload());
        }
    }
}