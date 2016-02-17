using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;
using TestDataFramework.Helpers;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class AttributeReadWriteTests
    {
        #region GetCustomAttributeHelper Tests (Returns single value)

        [TestMethod]
        public void GetCustomAttributeHelper_Programmatic_Test()
        {
            // Arrange

            Entity<AttributeReadWriteTestClass>.Decorate(c => c.Key1, new PrimaryKeyAttribute { KeyType = PrimaryKeyAttribute.KeyTypeEnum.Auto });

            // Act

            var attribute = typeof(AttributeReadWriteTestClass).GetProperty("Key1").GetCustomAttributeHelper<PrimaryKeyAttribute>();

            // Assert

            Assert.AreEqual(PrimaryKeyAttribute.KeyTypeEnum.Auto, attribute.KeyType);
        }

        [TestMethod]
        public void GetCustomAttributeHelper_Declarative_Test()
        {
            // Act

            var attribute = typeof(AttributeReadWriteTestClass).GetProperty("Text").GetCustomAttributeHelper<StringLengthAttribute>();

            // Assert

            Assert.AreEqual(20, attribute.Length);
        }

        [TestMethod]
        public void GetCustomAttributeHelper_Mixed_Test()
        {
            // Arrange

            Entity<AttributeReadWriteTestClass>.Decorate(c => c.Text, new PrimaryKeyAttribute { KeyType = PrimaryKeyAttribute.KeyTypeEnum.Auto });

            // Act

            var primaryKeyAttribute = typeof(AttributeReadWriteTestClass).GetProperty("Text").GetCustomAttributeHelper<PrimaryKeyAttribute>();
            var stringLengthAttribute = typeof(AttributeReadWriteTestClass).GetProperty("Text").GetCustomAttributeHelper<StringLengthAttribute>();

            // Assert

            Assert.AreEqual(PrimaryKeyAttribute.KeyTypeEnum.Auto, primaryKeyAttribute.KeyType);
            Assert.AreEqual(20, stringLengthAttribute.Length);
        }

        #endregion GetCustomAttributeHelper Tests (Returns single value)

        #region GetCustomAttributesHelper<T> Tests (Returns many values)

        [TestMethod]
        public void GetCustomAttributesHelperOfT_Test()
        {
            // Arrange

            Entity<AttributeReadWriteTestClass>.Decorate(c => c.Key2, new StringLengthAttribute(20));
            Entity<AttributeReadWriteTestClass>.Decorate(c => c.Key2, new StringLengthAttribute(30));

            // Act

            IEnumerable<StringLengthAttribute> attributes = typeof(AttributeReadWriteTestClass).GetProperty("Key2").GetCustomAttributesHelper<StringLengthAttribute>();

            // Assert

            attributes.Single(a => a.Length == 20);
            attributes.Single(a => a.Length == 30);
        }

        #endregion GetCustomAttributesHelper<T> Tests (Returns many values)
    }
}
