/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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

using System.Collections.Concurrent;
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
            // Arrange

            var assemblyMock = new Mock<AssemblyWrapper>();
            assemblyMock.Setup(m => m.GetReferencedAssemblies()).Returns(new[] { new AssemblyNameWrapper(), });
            assemblyMock.Setup(m => m.Equals(assemblyMock.Object)).Returns(true);

            // Act

            bool result = this.tableTypeCache.IsAssemblyCachePopulated(assemblyMock.Object);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsAssemblyCachePopulated_Yes_Test()
        {
            // Arrange

            var assemblyMock = new Mock<AssemblyWrapper>();
            var appDomainMock = Helpers.GetMock<AppDomainWrapper>();
            this.tableTypeCacheServiceMock.Setup(m => m.CreateDomain()).Returns(appDomainMock.Object);

            assemblyMock.Setup(m => m.GetReferencedAssemblies()).Returns(new[] { new AssemblyNameWrapper(), });
            assemblyMock.Setup(m => m.Equals(assemblyMock.Object)).Returns(true);

            // Act

            this.tableTypeCache.PopulateAssemblyCache(assemblyMock.Object, null, null);
            bool result = this.tableTypeCache.IsAssemblyCachePopulated(assemblyMock.Object);

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
        public void GetCachedTableType_ScanInitialAssembly_WithInitialAssembly()
        {
            var computedResultFromInitialAssemblyToScan = new TypeInfoWrapper();
            TypeInfoWrapper result = this.GetCachedTableType_Test(true, computedResultFromInitialAssemblyToScan);
            Assert.AreEqual(computedResultFromInitialAssemblyToScan, result);
        }

        [TestMethod]
        public void GetCachedTableType_DoNotScanInitialAssembly_WithInitialAssembly()
        {
            var computedResultFromInitialAssemblyToScan = new TypeInfoWrapper();
            TypeInfoWrapper result = this.GetCachedTableType_Test(false, computedResultFromInitialAssemblyToScan);
            Assert.AreEqual(computedResultFromInitialAssemblyToScan, result);
        }

        [TestMethod]
        public void GetCachedTableType_ScanInitialAssembly_WithAllAssemblies()
        {
            var computedResultFromAllAssemblies = new TypeInfoWrapper();
            TypeInfoWrapper result = this.GetCachedTableType_Test(true, null, computedResultFromAllAssemblies);
            Assert.AreEqual(computedResultFromAllAssemblies, result);
        }

        [TestMethod]
        public void GetCachedTableType_DoNotScanInitialAssembly_WithAllAssemblies()
        {
            var computedResultFromAllAssemblies = new TypeInfoWrapper();
            TypeInfoWrapper result = this.GetCachedTableType_Test(false, null, computedResultFromAllAssemblies);
            Assert.IsNull(result);
        }

        private TypeInfoWrapper GetCachedTableType_Test(bool canScanAllAssemblies,
            TypeInfoWrapper computedResultFromInitialAssemblyToScan,
            TypeInfoWrapper computedResultFromAllAssemblies = null)
        {
            // Arrange

            Mock<AssemblyWrapper> mockInitialAssemblyToScan = Helpers.GetMock<AssemblyWrapper>();
            mockInitialAssemblyToScan.Setup(m => m.Equals(mockInitialAssemblyToScan.Object)).Returns(true);

            Mock<AppDomainWrapper> mockDomain = Helpers.GetMock<AppDomainWrapper>();

            var foreignKeyAttribute = new ForeignKeyAttribute("tableName", "keyName");
            var mockGetTableAttibute = new Mock<GetTableAttribute>();

            Mock<TypeInfoWrapper> mockForeignType = Helpers.GetMock<TypeInfoWrapper>();

            var mockTableAttribute = new Mock<TableAttribute>("name");

            mockGetTableAttibute.Setup(m => m(mockForeignType.Object)).Returns(mockTableAttribute.Object);

            this.tableTypeCacheServiceMock.Setup(
                    m => m.GetCachedTableType(foreignKeyAttribute, mockTableAttribute.Object, It.IsAny<AssemblyLookupContext>()))
                .Returns(computedResultFromInitialAssemblyToScan);

            this.tableTypeCacheServiceMock.Setup(m => m.CreateDomain()).Returns(mockDomain.Object);

            this.tableTypeCacheServiceMock.Setup(m => m.GetCachedTableTypeUsingAllAssemblies(
                foreignKeyAttribute,
                mockTableAttribute.Object,
                this.tableTypeCacheServiceMock.Object.GetCachedTableType,
                It.IsAny<ConcurrentDictionary<AssemblyWrapper, AssemblyLookupContext>>())
            ).Returns(computedResultFromAllAssemblies).Verifiable();

            // Act

            this.tableTypeCache.PopulateAssemblyCache(mockInitialAssemblyToScan.Object, null, null);

            TypeInfoWrapper result = this.tableTypeCache.GetCachedTableType(foreignKeyAttribute, mockForeignType.Object,
                mockInitialAssemblyToScan.Object,
                mockGetTableAttibute.Object, canScanAllAssemblies);

            // Assert

            this.tableTypeCacheServiceMock.Verify(
                m => m.GetCachedTableType(foreignKeyAttribute, mockTableAttribute.Object, It.IsAny<AssemblyLookupContext>()));

            return result;
        }

        [TestMethod]
        public void PopulateAssemblyCache_Test()
        {
            // Arrange

            var assemblyMock = new Mock<AssemblyWrapper>();

            GetTableAttribute getTableAttribute = type => null;

            string defaultSchema = "DefaultSchema";

            var referencedAssemblyNames = new[]
                {new AssemblyNameWrapper(), new AssemblyNameWrapper()};

            var assemblyName = new AssemblyNameWrapper();

            assemblyMock.Setup(m => m.GetReferencedAssemblies())
                .Returns(referencedAssemblyNames.ToArray());

            assemblyMock.Setup(m => m.GetName()).Returns(assemblyName);

            var domainMock = new Mock<AppDomainWrapper>();

            this.tableTypeCacheServiceMock.Setup(m => m.CreateDomain()).Returns(domainMock.Object);

            // Act

            this.tableTypeCache.PopulateAssemblyCache(assemblyMock.Object, getTableAttribute, defaultSchema);

            // Assert

            referencedAssemblyNames.Concat(new[] {assemblyName}).ToList().ForEach(

                name => this.tableTypeCacheServiceMock.Verify(m => m.PopulateAssemblyCache(
                        domainMock.Object,
                        name,
                        getTableAttribute,
                        defaultSchema,
                        this.tableTypeCacheServiceMock.Object.TryAssociateTypeToTable,
                        It.IsAny<AssemblyLookupContext>()
                    ),

                    Times.Once
                ));

            domainMock.Verify(m => m.Unload());
        }
    }
}