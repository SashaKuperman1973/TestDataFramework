using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class AttributeDecoratorTests
    {
        private class Populator : BasePopulator
        {
            public Populator(IAttributeDecorator attributeDecorator) : base(attributeDecorator)
            { }
        }

        private Populator populator;
        private IAttributeDecorator attributeDecorator;

        [TestInitialize]
        public void TestInitialize()
        {
            this.attributeDecorator = new AttributeDecorator();
            this.populator = new Populator(this.attributeDecorator);
        }

        #region GetCustomAttributeHelper Tests (Returns single value)

        [TestMethod]
        public void GetCustomAttributeHelper_Programmatic_Test()
        {
            // Arrange

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToMember(c => c.Key1, new PrimaryKeyAttribute {KeyType = PrimaryKeyAttribute.KeyTypeEnum.Auto});

            // Act

            var attribute =
                this.attributeDecorator.GetCustomAttribute<PrimaryKeyAttribute>(
                    typeof (AttributeReadWriteTestClass).GetProperty("Key1"));

            // Assert

            Assert.AreEqual(PrimaryKeyAttribute.KeyTypeEnum.Auto, attribute.KeyType);
        }

        [TestMethod]
        public void GetCustomAttributeHelper_Declarative_Test()
        {
            // Act

            var attribute =
                this.attributeDecorator.GetCustomAttribute<StringLengthAttribute>(
                    typeof (AttributeReadWriteTestClass).GetProperty("Text"));

            // Assert

            Assert.AreEqual(20, attribute.Length);
        }

        [TestMethod]
        public void GetCustomAttributeHelper_Mixed_Test()
        {
            // Arrange

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToMember(c => c.Text,
                    new PrimaryKeyAttribute {KeyType = PrimaryKeyAttribute.KeyTypeEnum.Auto});

            // Act

            var primaryKeyAttribute =
                this.attributeDecorator.GetCustomAttribute<PrimaryKeyAttribute>(
                    typeof (AttributeReadWriteTestClass).GetProperty("Text"));

            var stringLengthAttribute =
                this.attributeDecorator.GetCustomAttribute<StringLengthAttribute>(
                    typeof (AttributeReadWriteTestClass).GetProperty("Text"));

            // Assert

            Assert.AreEqual(PrimaryKeyAttribute.KeyTypeEnum.Auto, primaryKeyAttribute.KeyType);
            Assert.AreEqual(20, stringLengthAttribute.Length);
        }

        #endregion GetCustomAttributeHelper Tests (Returns single value)

        #region GetCustomAttributesHelper<T> Tests (Returns many values)

        [TestMethod]
        public void GetCustomAttributesHelperOfT_Programmatic_Test()
        {
            // Arrange

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToMember(c => c.Key2, new StringLengthAttribute(20));

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToMember(c => c.Key2, new StringLengthAttribute(30));

            // Act

            IEnumerable<StringLengthAttribute> attributes =

                this.attributeDecorator.GetCustomAttributes<StringLengthAttribute>(
                    typeof (AttributeReadWriteTestClass).GetProperty("Key2")).ToList();

            // Assert

            Assert.AreEqual(2, attributes.Count());
            attributes.Single(a => a.Length == 20);
            attributes.Single(a => a.Length == 30);
        }

        [TestMethod]
        public void GetCustomAttributesHelperOfT_Declarative_Test()
        {
            // Arrange. Act.

            IEnumerable<MultiAllowedAttribute> attributes =

                this.attributeDecorator.GetCustomAttributes<MultiAllowedAttribute>(
                    typeof (AttributeReadWriteTestClass).GetProperty("MultiAllowedProperty")).ToList();

            // Assert

            Assert.AreEqual(2, attributes.Count());
            attributes.Single(a => a.I == 1);
            attributes.Single(a => a.I == 2);
        }

        [TestMethod]
        public void GetCustomAttributesHelperOfT_Mixed_Test()
        {
            // Arrange

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToMember(c => c.MultiAllowedProperty, new MultiAllowedAttribute {I = 55});

            // Act.

            IEnumerable<MultiAllowedAttribute> attributes =
                this.attributeDecorator.GetCustomAttributes<MultiAllowedAttribute>(
                    typeof (AttributeReadWriteTestClass).GetProperty("MultiAllowedProperty")).ToList();

            // Assert

            Assert.AreEqual(3, attributes.Count());
            attributes.Single(a => a.I == 1);
            attributes.Single(a => a.I == 2);
            attributes.Single(a => a.I == 55);
        }

        #endregion GetCustomAttributesHelper<T> Tests (Returns many values)

        #region GetCustomAttributesHelper Non-generic Tests (Returns many values)

        [TestMethod]
        public void GetCustomAttributesHelper_Programmatic_Test()
        {
            // Arrange

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToMember(c => c.Key2, new StringLengthAttribute(20));

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToMember(c => c.Key2, new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Auto));

            // Act

            IEnumerable<Attribute> attributes =
                this.attributeDecorator.GetCustomAttributes(typeof (AttributeReadWriteTestClass).GetProperty("Key2"))
                    .ToList();

            // Assert

            Assert.AreEqual(2, attributes.Count());
            attributes.Single(a => a.GetType() == typeof(StringLengthAttribute));
            attributes.Single(a => a.GetType() == typeof(PrimaryKeyAttribute));
        }

        [TestMethod]
        public void GetCustomAttributesHelper_Declarative_Test()
        {
            // Arrange. Act

            IEnumerable<Attribute> attributes =
                this.attributeDecorator.GetCustomAttributes(
                    typeof (AttributeReadWriteTestClass).GetProperty("MultiAtributeProperty"));

            // Assert

            Assert.AreEqual(2, attributes.Count());
            attributes.Single(a => a.GetType() == typeof(StringLengthAttribute));
            attributes.Single(a => a.GetType() == typeof(PrimaryKeyAttribute));
        }

        [TestMethod]
        public void GetCustomAttributesHelper_Mixed_Test()
        {
            // Arrange

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToMember(c => c.MultiAllowedProperty, new MultiAllowedAttribute { I = 55 });

            // Act.

            IEnumerable<Attribute> attributes =
                this.attributeDecorator.GetCustomAttributes(
                    typeof(AttributeReadWriteTestClass).GetProperty("MultiAllowedProperty")).ToList();

            // Assert

            Assert.AreEqual(3, attributes.Count());

            IEnumerable<MultiAllowedAttribute> specificAttributes = attributes.Cast<MultiAllowedAttribute>();

            specificAttributes.Single(a => a.I == 1);
            specificAttributes.Single(a => a.I == 2);
            specificAttributes.Single(a => a.I == 55);
        }

        #endregion GetCustomAttributesHelper Non-generic Tests (Returns many values)

        #region DecorateType Test

        [TestMethod]
        public void Decorate_Type_Programmatic_Test()
        {
            // Arrange

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToType(new TableAttribute { Name = "TableNameA"});

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToType(new TableAttribute { Name = "TableNameB" });

            // Act

            IEnumerable<TableAttribute> attributes =
                this.attributeDecorator.GetCustomAttributes<TableAttribute>(typeof(AttributeReadWriteTestClass))
                    .ToList();

            // Assert

            Assert.AreEqual(2, attributes.Count());
            attributes.Single(a => a.Name == "TableNameA");
            attributes.Single(a => a.Name == "TableNameB");
        }

        [TestMethod]
        public void Decorate_Type_Declarative_Test()
        {
            // Arrange. Act

            IEnumerable<MultiAllowedAttribute> attributes =
                this.attributeDecorator.GetCustomAttributes<MultiAllowedAttribute>(typeof (AttributeReadWriteTestClass))
                    .ToList();

            // Assert

            Assert.AreEqual(2, attributes.Count());
            attributes.Single(a => a.I == 1);
            attributes.Single(a => a.I == 2);
        }

        [TestMethod]
        public void Decorate_Type_Mixed_Test()
        {
            // Arrange

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToType(new MultiAllowedAttribute { I = 55 });

            // Act.

            IEnumerable<Attribute> attributes =
                this.attributeDecorator.GetCustomAttributes(
                    typeof(AttributeReadWriteTestClass)).ToList();

            // Assert

            Assert.AreEqual(3, attributes.Count());

            IEnumerable<MultiAllowedAttribute> specificAttributes = attributes.Cast<MultiAllowedAttribute>();

            specificAttributes.Single(a => a.I == 1);
            specificAttributes.Single(a => a.I == 2);
            specificAttributes.Single(a => a.I == 55);
        }

        #endregion DecorateType Test
    }
}
