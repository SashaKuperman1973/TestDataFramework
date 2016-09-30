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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class TableTests
    {
        #region Constructor tests

        [TestMethod]
        public void ForeignKeyAttribute_Constructor_Test()
        {
            // Arrange

            const string schema = "schemaABC";
            const string primaryTableName = "primaryTableNameABC";
            const string primaryKeyName = "primaryKeyNameABC";

            var fkAttribute = new ForeignKeyAttribute(schema, primaryTableName, primaryKeyName);

            // Act

            var table = new Table(fkAttribute, null);

            // Assert

            Assert.IsFalse(table.HasTableAttribute);
            Assert.IsFalse(table.HasCatalogueName);
            Assert.IsNull(table.CatalogueName);
            Assert.AreEqual(schema, table.Schema);
            Assert.AreEqual(primaryTableName, table.TableName);

            Assert.AreEqual(schema.GetHashCode() ^ primaryTableName.GetHashCode(), table.GetHashCode());
        }

        [TestMethod]
        public void ForeignKeyAttribute_TableAttribute_Constructor_Test()
        {
            // Arrange

            const string schema = "schemaABC";
            const string primaryTableName = "primaryTableNameABC";
            const string primaryKeyName = "primaryKeyNameABC";

            var fkAttribute = new ForeignKeyAttribute(schema, primaryTableName, primaryKeyName);

            const string catalogueName = "catalogueNameABC";
            
            var tableAttribute = new TableAttribute(catalogueName, "fkTableSchemaABC", "fkTableName");

            // Act

            var table = new Table(fkAttribute, tableAttribute);

            // Assert

            Assert.IsTrue(table.HasTableAttribute);
            Assert.IsTrue(table.HasCatalogueName);
            Assert.AreEqual(catalogueName, table.CatalogueName);

            Assert.AreEqual(schema.GetHashCode() ^ primaryTableName.GetHashCode(), table.GetHashCode());
        }

        [TestMethod]
        public void Type_Constructor_Test()
        {
            // Arrange

            const string defaultSchema = "schema123";

            // Act

            var table = new Table(typeof(SubjectClass), defaultSchema);

            // Assert

            Assert.IsFalse(table.HasTableAttribute);
            Assert.IsNull(table.CatalogueName);
            Assert.AreEqual(defaultSchema, table.Schema);
            Assert.AreEqual(nameof(SubjectClass), table.TableName);

            Assert.AreEqual(defaultSchema.GetHashCode() ^ nameof(SubjectClass).GetHashCode(), table.GetHashCode());
        }

        [TestMethod]
        public void TableAttribute_Constructor_Test()
        {
            // Arrange

            const string catalogueName = "catalogueABC";
            const string schema = "schemaABC";
            const string tableName = "tableNameABC";

            var tableAttribute = new TableAttribute(catalogueName, schema, tableName);

            // Act

            var table = new Table(tableAttribute);

            // Assert

            Assert.IsTrue(table.HasTableAttribute);
            Assert.AreEqual(catalogueName, table.CatalogueName);
            Assert.AreEqual(schema, table.Schema);
            Assert.AreEqual(tableName, table.TableName);

            Assert.AreEqual(schema.GetHashCode() ^ tableName.GetHashCode(), table.GetHashCode());
        }

        #endregion Constructor tests

        #region BasicFieldsEqual tests

        [TestMethod]
        public void Different_Object_Equals_Test()
        {
            String s = "ABCD";

            var table = new Table(typeof(SubjectClass), null);

            Assert.IsFalse(table.BasicFieldsEqual(s));
        }

        [TestMethod]
        public void NoSchemaInput_NoCatalogueInput_Equals_Test()
        {
            var table1 = new Table(typeof(SubjectClass), null);
            var table2 = new Table(typeof(SubjectClass), null);

            Assert.IsTrue(table1.BasicFieldsEqual(table2));
        }

        [TestMethod]
        public void InputForTryGet_NoTableAttribute_Equals_Test()
        {
            const string schema = "schemaABC";
            const string primaryTableName = "primaryTableNameABC";
            const string primaryKeyName = "primaryKeyNameABC";

            var fkAttribute1 = new ForeignKeyAttribute(schema, primaryTableName, primaryKeyName);
            var table1 = new Table(fkAttribute1, null);

            var fkAttribute2 = new ForeignKeyAttribute(schema, primaryTableName, primaryKeyName);
            var table2 = new Table(fkAttribute2, null);

            Assert.IsTrue(table1.BasicFieldsEqual(table2));
        }

        [TestMethod]
        public void DifferentSchema_NoTableAttribute_DoesNotEqual_Test()
        {
            const string schema1 = "schema1ABC";
            const string schema2 = "schema2ABC";
            const string primaryTableName = "primaryTableNameABC";
            const string primaryKeyName = "primaryKeyNameABC";

            var fkAttribute1 = new ForeignKeyAttribute(schema1, primaryTableName, primaryKeyName);
            var table1 = new Table(fkAttribute1, null);

            var fkAttribute2 = new ForeignKeyAttribute(schema2, primaryTableName, primaryKeyName);
            var table2 = new Table(fkAttribute2, null);

            Assert.IsFalse(table1.BasicFieldsEqual(table2));
        }

        [TestMethod]
        public void SameCatalogue_DifferentSchema_DoesNotEqual_Test()
        {
            const string catalogue = "catalogueABC";
            const string schema1 = "schema1ABC";
            const string schema2 = "schema2ABC";
            const string tableName = "primaryTableNameABC";

            var tableAttribute1 = new TableAttribute(catalogue, schema1, tableName);
            var table1 = new Table(tableAttribute1);

            var tableAttribute2 = new TableAttribute(catalogue, schema2, tableName);
            var table2 = new Table(tableAttribute2);

            Assert.IsFalse(table1.BasicFieldsEqual(table2));
        }

        [TestMethod]
        public void DifferentCatalogue_SameSchema_Equals_Test()
        {
            const string catalogue1 = "catalogue1ABC";
            const string catalogue2 = "catalogue2ABC";
            const string schema = "schema1ABC";
            const string tableName = "primaryTableNameABC";

            var tableAttribute1 = new TableAttribute(catalogue1, schema, tableName);
            var table1 = new Table(tableAttribute1);

            var tableAttribute2 = new TableAttribute(catalogue2, schema, tableName);
            var table2 = new Table(tableAttribute2);

            Assert.IsTrue(table1.BasicFieldsEqual(table2));
        }

        [TestMethod]
        public void SameCatalogue_SameSchema_DifferentTable_DoesNotEqual_Test()
        {
            const string catalogue = "catalogue2ABC";
            const string schema = "schema1ABC";
            const string tableName1 = "primaryTableName1ABC";
            const string tableName2 = "primaryTableName2ABC";

            var tableAttribute1 = new TableAttribute(catalogue, schema, tableName1);
            var table1 = new Table(tableAttribute1);

            var tableAttribute2 = new TableAttribute(catalogue, schema, tableName2);
            var table2 = new Table(tableAttribute2);

            Assert.IsFalse(table1.BasicFieldsEqual(table2));
        }

        #endregion BasicFieldsEqual tests
    }
}
