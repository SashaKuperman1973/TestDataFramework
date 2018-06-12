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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class StandardTableTypeCacheTests
    {
        private StandardTableTypeCache tableTypeCache;

        private Mock<ITableTypeCacheService> tableTypeServicedMock;

        [TestInitialize]
        public void Initialize()
        {
            this.tableTypeServicedMock = new Mock<ITableTypeCacheService>();
            this.tableTypeCache = new StandardTableTypeCache(this.tableTypeServicedMock.Object);
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
    }
}