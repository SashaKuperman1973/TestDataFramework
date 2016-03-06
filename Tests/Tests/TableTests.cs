using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class TableTests
    {
        [TestMethod]
        public void ForeignKeyAttribute_Constructor_Test()
        {
            // Arrange

            const string schema = "schemaABC";
            const string primaryTableName = "primaryTableNameABC";
            const string primaryKeyName = "primaryKeyNameABC";

            var fkAttribute = new ForeignKeyAttribute(schema, primaryTableName, primaryKeyName);

            // Act

            var table = new Table(fkAttribute);

            // Assert

            Assert.AreEqual(Table.HasTableAttributeEnum.NotSet, table.HasTableAttribute);
            Assert.AreEqual(null, table.CatalogueName);
            Assert.AreEqual(schema, table.Schema);
            Assert.AreEqual(primaryTableName, table.TableName);

            Assert.AreEqual(schema.GetHashCode() ^ primaryTableName.GetHashCode() , table.GetHashCode());
        }

        [TestMethod]
        public void Type_Constructor_Test()
        {
            // Arrange. Act.

            var table = new Table(typeof(SubjectClass));

            // Assert

            Assert.AreEqual(Table.HasTableAttributeEnum.False, table.HasTableAttribute);
            Assert.AreEqual(null, table.CatalogueName);
            Assert.AreEqual("dbo", table.Schema);
            Assert.AreEqual(nameof(SubjectClass), table.TableName);

            Assert.AreEqual("dbo".GetHashCode() ^ nameof(SubjectClass).GetHashCode(), table.GetHashCode());
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

            Assert.AreEqual(Table.HasTableAttributeEnum.True, table.HasTableAttribute);
            Assert.AreEqual(catalogueName, table.CatalogueName);
            Assert.AreEqual(schema, table.Schema);
            Assert.AreEqual(tableName, table.TableName);

            Assert.AreEqual(schema.GetHashCode() ^ tableName.GetHashCode() ^ catalogueName.GetHashCode(), table.GetHashCode());
        }

        #region Equals tests

        [TestMethod]
        public void Different_Object_Equals_Test()
        {
            String s = "ABCD";

            var table = new Table(typeof(SubjectClass));

            Assert.IsFalse(table.Equals(s));
        }

        [TestMethod]
        public void NullSchema_NullCatalogue_Equals_Test()
        {
            var table1 = new Table(typeof(SubjectClass));
            var table2 = new Table(typeof(SubjectClass));

            Assert.IsTrue(table1.Equals(table2));
        }

        [TestMethod]
        public void NullCatalogue_SameSchema_Equals_Test()
        {
            const string schema = "schemaABC";
            const string primaryTableName = "primaryTableNameABC";
            const string primaryKeyName = "primaryKeyNameABC";

            var fkAttribute1 = new ForeignKeyAttribute(schema, primaryTableName, primaryKeyName);
            var table1 = new Table(fkAttribute1);

            var fkAttribute2 = new ForeignKeyAttribute(schema, primaryTableName, primaryKeyName);
            var table2 = new Table(fkAttribute2);

            Assert.IsTrue(table1.Equals(table2));
        }

        [TestMethod]
        public void NullCatalogue_DifferentSchema_DoesNotEqual_Test()
        {
            const string schema1 = "schema1ABC";
            const string schema2 = "schema2ABC";
            const string primaryTableName = "primaryTableNameABC";
            const string primaryKeyName = "primaryKeyNameABC";

            var fkAttribute1 = new ForeignKeyAttribute(schema1, primaryTableName, primaryKeyName);
            var table1 = new Table(fkAttribute1);

            var fkAttribute2 = new ForeignKeyAttribute(schema2, primaryTableName, primaryKeyName);
            var table2 = new Table(fkAttribute2);

            Assert.IsFalse(table1.Equals(table2));
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

            Assert.IsFalse(table1.Equals(table2));
        }

        [TestMethod]
        public void DifferentCatalogue_SameSchema_DoesNotEqual_Test()
        {
            const string catalogue1 = "catalogue1ABC";
            const string catalogue2 = "catalogue2ABC";
            const string schema = "schema1ABC";
            const string tableName = "primaryTableNameABC";

            var tableAttribute1 = new TableAttribute(catalogue1, schema, tableName);
            var table1 = new Table(tableAttribute1);

            var tableAttribute2 = new TableAttribute(catalogue2, schema, tableName);
            var table2 = new Table(tableAttribute2);

            Assert.IsFalse(table1.Equals(table2));
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

            Assert.IsFalse(table1.Equals(table2));
        }

        #endregion Equals tests
    }
}
