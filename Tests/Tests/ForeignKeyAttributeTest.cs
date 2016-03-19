using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;

namespace Tests.Tests
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
            Helpers.ExceptionTest(() => { new ForeignKeyAttribute((Type) null, "xxx"); }, typeof (ArgumentNullException),
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
            Assert.AreEqual(concreteOriginalAttribute.PrimaryTableType, newForeignKeyAttribute.PrimaryTableType);
        }

        [TestMethod]
        public void IsDefaultSchema_TrueFalse_Test()
        {
            // Arrange. Act.

            ICanHaveDefaultSchema foreignKeyAttributeWithPrimaryType = new ForeignKeyAttribute(typeof(string), "primaryKeyName123");
            ICanHaveDefaultSchema foreignKeyAttributeWithTableName = new ForeignKeyAttribute("primaryTableName123", "primaryKeyName123");
            ICanHaveDefaultSchema foreignKeyAttributeWithSchema = new ForeignKeyAttribute("schema123", "primaryTableName123", "primaryKeyName");

            // Assert

            Assert.IsFalse(foreignKeyAttributeWithPrimaryType.IsDefaultSchema);
            Assert.IsTrue(foreignKeyAttributeWithTableName.IsDefaultSchema);
            Assert.IsFalse(foreignKeyAttributeWithSchema.IsDefaultSchema);
        }
    }
}
