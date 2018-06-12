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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class ForeignKeyAttributeTest
    {
        [TestMethod]
        public void Constructor_PrimaryTableType_Test()
        {
            const string primaryKeyName = "primaryKeyName123";
            var foreignKeyAttribute = new ForeignKeyAttribute(typeof(string), primaryKeyName);

            Assert.AreEqual(typeof(string), foreignKeyAttribute.PrimaryTableType);
            Assert.AreEqual(primaryKeyName, foreignKeyAttribute.PrimaryKeyName);
            Assert.AreEqual(typeof(string).Name, foreignKeyAttribute.PrimaryTableName);
            Assert.IsFalse(foreignKeyAttribute.IsDefaultSchema);
        }

        [TestMethod]
        public void Constructor_PrimaryTableName_Test()
        {
            const string primaryTableName = "primaryTableName123", primaryKeyName = "primaryKeyName123";
            var foreignKeyAttribute = new ForeignKeyAttribute(primaryTableName, primaryKeyName);

            Assert.IsNull(foreignKeyAttribute.Schema);
            Assert.IsNull(foreignKeyAttribute.PrimaryTableType);
            Assert.IsTrue(foreignKeyAttribute.IsDefaultSchema);
            Assert.AreEqual(primaryTableName, foreignKeyAttribute.PrimaryTableName);
            Assert.AreEqual(primaryKeyName, foreignKeyAttribute.PrimaryKeyName);
        }

        [TestMethod]
        public void Constructor_Schema_Test()
        {
            const string primaryTableName = "primaryTableName123",
                primaryKeyName = "primaryKeyName123",
                schema = "schema123";

            var foreignKeyAttribute = new ForeignKeyAttribute(schema, primaryTableName, primaryKeyName);

            Assert.IsNull(foreignKeyAttribute.PrimaryTableType);
            Assert.IsFalse(foreignKeyAttribute.IsDefaultSchema);
            Assert.AreEqual(schema, foreignKeyAttribute.Schema);
            Assert.AreEqual(primaryTableName, foreignKeyAttribute.PrimaryTableName);
            Assert.AreEqual(primaryKeyName, foreignKeyAttribute.PrimaryKeyName);
        }

        [TestMethod]
        public void Constructor_NoPrimaryTableType_Exception()
        {
            Helpers.ExceptionTest(() => { new ForeignKeyAttribute((Type) null, "xxx"); }, typeof(ArgumentNullException),
                "Value cannot be null.\r\nParameter name: primaryTableType");
        }

        [TestMethod]
        public void GetAttributeUsingDefaultSchema_TableName_Test()
        {
            // Arrange. 

            var concreteOriginalAttribute = new ForeignKeyAttribute("tableName123", "primaryKeyName123");
            ICanHaveDefaultSchema originalForeignKeyAttribute = concreteOriginalAttribute;

            // Act.

            const string defaultSchema = "defaultSchema123";
            Attribute newAttribute = originalForeignKeyAttribute.GetAttributeUsingDefaultSchema(defaultSchema);
            var newForeignKeyAttribute = newAttribute as ForeignKeyAttribute;

            // Assert

            Assert.IsNotNull(newForeignKeyAttribute);
            Assert.IsFalse(newForeignKeyAttribute.IsDefaultSchema);
            Assert.AreEqual(defaultSchema, newForeignKeyAttribute.Schema);
            Assert.AreEqual(concreteOriginalAttribute.PrimaryKeyName, newForeignKeyAttribute.PrimaryKeyName);
            Assert.AreEqual(concreteOriginalAttribute.PrimaryTableName, newForeignKeyAttribute.PrimaryTableName);
        }

        [TestMethod]
        public void GetAttributeUsingDefaultSchema_TableType_Test()
        {
            // Arrange. 

            var concreteOriginalAttribute = new ForeignKeyAttribute(typeof(string), "primaryKeyName123");
            ICanHaveDefaultSchema originalForeignKeyAttribute = concreteOriginalAttribute;

            // Act.

            const string defaultSchema = "defaultSchema123";
            Attribute newAttribute = originalForeignKeyAttribute.GetAttributeUsingDefaultSchema(defaultSchema);
            var newForeignKeyAttribute = newAttribute as ForeignKeyAttribute;

            // Assert

            Assert.IsNotNull(newForeignKeyAttribute);
            Assert.IsFalse(newForeignKeyAttribute.IsDefaultSchema);
            Assert.AreEqual(defaultSchema, newForeignKeyAttribute.Schema);
            Assert.AreEqual(concreteOriginalAttribute.PrimaryKeyName, newForeignKeyAttribute.PrimaryKeyName);
            Assert.IsNull(newForeignKeyAttribute.PrimaryTableType);
        }

        [TestMethod]
        public void IsDefaultSchema_TrueFalse_Test()
        {
            // Arrange. Act.

            ICanHaveDefaultSchema foreignKeyAttributeWithPrimaryType =
                new ForeignKeyAttribute(typeof(string), "primaryKeyName123");
            ICanHaveDefaultSchema foreignKeyAttributeWithTableName =
                new ForeignKeyAttribute("primaryTableName123", "primaryKeyName123");
            ICanHaveDefaultSchema foreignKeyAttributeWithSchema =
                new ForeignKeyAttribute("schema123", "primaryTableName123", "primaryKeyName");

            // Assert

            Assert.IsFalse(foreignKeyAttributeWithPrimaryType.IsDefaultSchema);
            Assert.IsTrue(foreignKeyAttributeWithTableName.IsDefaultSchema);
            Assert.IsFalse(foreignKeyAttributeWithSchema.IsDefaultSchema);
        }
    }
}