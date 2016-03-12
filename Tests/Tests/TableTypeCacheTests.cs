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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.Exceptions;

namespace Tests.Tests
{
    [TestClass]
    public class TableTypeCacheTests
    {
        private TableTypeCache tableTypeCache;

        [TestInitialize]
        public void Initialize()
        {
            this.tableTypeCache = new TableTypeCache();
        }

        #region IsAssemblyCachePopulated tests

        [TestMethod]
        public void IsAssemblyCachePopulated_No_Test()
        {
            // Act

            bool result = this.tableTypeCache.IsAssemblyCachePopulated(this.GetType().Assembly);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsAssemblyCachePopulated_Yes_Test()
        {
            // Arrange
            Type foreignType = typeof(TestModels.Foreign.ForeignClass);
            this.tableTypeCache.PopulateAssemblyCache(foreignType.Assembly, type => (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute)));

            // Act

            bool result = this.tableTypeCache.IsAssemblyCachePopulated(this.GetType().Assembly);

            // Assert

            Assert.IsTrue(result);
        }

        #endregion IsAssemblyCachePopulated tests

        #region GetCachedTableType non collision tests

        [TestMethod]
        public void GetCachedTableType_AssembyCacheNotPopulated_Test()
        {
            // Act

            Helpers.ExceptionTest(() => this.tableTypeCache.GetCachedTableType(null, this.GetType(), CustomAttributeExtensions.GetCustomAttribute<TableAttribute>),
                typeof (TableTypeLookupException),
                string.Format(Messages.AssemblyCacheNotPopulated, this.GetType().Assembly));
        }

        [TestMethod]
        public void GetCachedTableType_GetCached_DecoratedTableType_WithNonDecoratedInCache_Test()
        {
            // Arrange

            Type foreignType = typeof (TestModels.Foreign.ForeignClass);

            var foreignAttribute =
                foreignType.GetProperty("TableName_ForeignKey").GetCustomAttribute<ForeignKeyAttribute>();

            this.tableTypeCache.PopulateAssemblyCache(foreignType.Assembly, type => (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute)));

            // Act

            Type result = this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType, CustomAttributeExtensions.GetCustomAttribute<TableAttribute>);

            // Assert

            Assert.AreEqual(typeof(TestModels.A.TableNameClass), result);
        }

        [TestMethod]
        public void GetCachedTableType_GetCached_UndecoratedType_Test()
        {
            // Arrange

            Type foreignType = typeof(TestModels.Simple.ForeignClass);

            var foreignAttribute =
                foreignType.GetProperty("ForeignKey").GetCustomAttribute<ForeignKeyAttribute>();

            this.tableTypeCache.PopulateAssemblyCache(foreignType.Assembly, type => (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute)));

            // Act

            Type result = this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType, CustomAttributeExtensions.GetCustomAttribute<TableAttribute>);

            // Assert

            Assert.AreEqual(typeof(TestModels.Simple.PrimaryClass), result);
        }

        [TestMethod]
        public void GetCachedTableType_GetCached_UndecoratedType_Repeated_Test()
        {
            // Arrange

            Type foreignType = typeof(TestModels.Simple.ForeignClass);

            var foreignAttribute =
                foreignType.GetProperty("ForeignKey").GetCustomAttribute<ForeignKeyAttribute>();

            this.tableTypeCache.PopulateAssemblyCache(foreignType.Assembly, type => (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute)));

            // Act

            Type result1 = this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType, CustomAttributeExtensions.GetCustomAttribute<TableAttribute>);
            Type result2 = this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType, CustomAttributeExtensions.GetCustomAttribute<TableAttribute>);

            // Assert

            Assert.AreEqual(typeof(TestModels.Simple.PrimaryClass), result1);
            Assert.AreEqual(typeof(TestModels.Simple.PrimaryClass), result2);
        }

        #endregion GetCachedTableType non collision tests

        #region GetCachedTableType collision tests

        [TestMethod]
        public void GetCachedTableType_GetCached_DecoratedType_Two_Item_Collision_Test()
        {
            // Arrange

            Type foreignType = typeof (TestModels.Foreign.ForeignClass);

            var foreignAttribute =
                foreignType.GetProperty("DecoratedCollision_ForeignKey").GetCustomAttribute<ForeignKeyAttribute>();

            this.tableTypeCache.PopulateAssemblyCache(foreignType.Assembly, type => (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute)));

            // Act

            Helpers.ExceptionTest(() => this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType, CustomAttributeExtensions.GetCustomAttribute<TableAttribute>),
                typeof (TableTypeCacheException), string.Format(Messages.DuplicateTableName, 
                string.Join(", ", new object[] {
                    typeof (Test.TestModels.DecoratedCollision.B.DecoratedCollisionClass),
                    typeof (Test.TestModels.DecoratedCollision.A.DecoratedCollisionClass)}
                )));
        }

        [TestMethod]
        public void GetCachedTableType_GetCached_DecoratedType_Three_Item_Collision_Test()
        {
            // Arrange

            Type foreignType = typeof(TestModels.Foreign.ForeignClass);

            var foreignAttribute =
                foreignType.GetProperty("DecoratedCollision3Way_ForeignKey").GetCustomAttribute<ForeignKeyAttribute>();

            this.tableTypeCache.PopulateAssemblyCache(foreignType.Assembly, type => (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute)));

            // Act

            Helpers.ExceptionTest(() => this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType, CustomAttributeExtensions.GetCustomAttribute<TableAttribute>),
                typeof (TableTypeCacheException), string.Format(Messages.DuplicateTableName,
                    string.Join(", ", new object[]
                    {
                        typeof (Test.TestModels.DecoratedCollision.ThreeWay.DecoratedCollisionClass3Way),
                        typeof (Test.TestModels.DecoratedCollision.B.DecoratedCollisionClass3Way),
                        typeof (Test.TestModels.DecoratedCollision.A.DecoratedCollisionClass3Way)
                    }
                        )));
        }

        [TestMethod]
        public void GetCachedTableType_GetCached_DecoratedTypes_With_DifferentClassNames_Two_Item_Collision_Test()
        {
            // Arrange

            Type foreignType = typeof(TestModels.Foreign.ForeignClass);

            var foreignAttribute =
                foreignType.GetProperty("DecoratedCollisionWithDifferentClassName_ForeignKey").GetCustomAttribute<ForeignKeyAttribute>();

            this.tableTypeCache.PopulateAssemblyCache(foreignType.Assembly, type => (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute)));

            // Act

            Helpers.ExceptionTest(() => this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType, CustomAttributeExtensions.GetCustomAttribute<TableAttribute>),
                typeof (TableTypeCacheException), string.Format(Messages.DuplicateTableName,
                    string.Join(", ", new object[]
                    {
                        typeof (
                            Test.TestModels.DecoratedCollisionWithDifferentClassName.B.
                                DecoratedCollisionWithDifferentClassName_B),
                        typeof (
                            Test.TestModels.DecoratedCollisionWithDifferentClassName.A.
                                DecoratedCollisionWithDifferentClassName_A)
                    }
                        )));
        }

        [TestMethod]
        public void GetCachedTableType_GetCached_DecoratedTypes_With_DifferentClassNames_Three_Item_Collision_Test()
        {
            // Arrange

            Type foreignType = typeof(TestModels.Foreign.ForeignClass);

            var foreignAttribute =
                foreignType.GetProperty("DecoratedCollisionWithDifferentClassName3Way_ForeignKey").GetCustomAttribute<ForeignKeyAttribute>();

            this.tableTypeCache.PopulateAssemblyCache(foreignType.Assembly, type => (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute)));

            // Act

            Helpers.ExceptionTest(() => this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType, CustomAttributeExtensions.GetCustomAttribute<TableAttribute>),
                typeof(TableTypeCacheException), string.Format(Messages.DuplicateTableName,
                    string.Join(", ", new object[]
                    {
                        typeof (Test.TestModels.DecoratedCollisionWithDifferentClassName.ThreeWay.DecoratedCollisionWithDifferentClassName_3Way_ThreeWay),
                        typeof (Test.TestModels.DecoratedCollisionWithDifferentClassName.B.DecoratedCollisionWithDifferentClassName_3Way_B),
                        typeof (Test.TestModels.DecoratedCollisionWithDifferentClassName.A.DecoratedCollisionWithDifferentClassName_3Way_A)
                    }
                        )));
        }

        #endregion GetCachedTableType collision tests
    }
}
