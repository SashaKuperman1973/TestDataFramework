﻿/*
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.Populator;
using Tests.TestModels;
using Tests.TestModels.Simple;

namespace Tests.Tests
{
    [TestClass]
    public class StandardAttributeDecoratorTests
    {
        private class Populator : BasePopulator
        {
            public Populator(IAttributeDecorator attributeDecorator) : base(attributeDecorator)
            { }
        }

        private Populator populator;
        private Mock<TableTypeCache> tableTypeCacheMock;
        private StandardAttributeDecorator attributeDecorator;

        [TestInitialize]
        public void TestInitialize()
        {
            this.tableTypeCacheMock = new Mock<TableTypeCache>();
            this.attributeDecorator = new StandardAttributeDecorator(this.tableTypeCacheMock.Object);
            this.populator = new Populator(this.attributeDecorator);
        }

        #region GetCustomAttributeHelper Tests (Returns single value)

        [TestMethod]
        public void GetCustomAttributeHelper_Programmatic_Test()
        {
            // Arrange

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToMember(c => c.Key1, new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Auto));

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
                    new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Auto));

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

        [TestMethod]
        public void GetSingleAttribute_Test()
        {
            throw new NotImplementedException();
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
                .AddAttributeToType(new TableAttribute("TableNameA"));

            this.populator.DecorateType<AttributeReadWriteTestClass>()
                .AddAttributeToType(new TableAttribute("TableNameB"));

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

        #region GetTableType tests

        [TestMethod]
        public void GetTableType_PrimaryTableType_Test()
        {
            // Arrange

            var foreignKeyAtribute = new ForeignKeyAttribute(typeof(TestModels.Simple.PrimaryClass), null);

            // Act

            Type result = this.attributeDecorator.GetTableType(foreignKeyAtribute, null);

            // Assert

            Assert.AreEqual(typeof(TestModels.Simple.PrimaryClass), result);
        }

        [TestMethod]
        public void GetTableType_AssemblyCacheIsNotPopulated_Test()
        {
            // Arrange

            Type foreignType = typeof (ForeignClass);

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);

            Type returnedType = typeof (PrimaryClass);

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(foreignType.Assembly)).Returns(false);
            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignType, this.attributeDecorator.GetSingleAttribute<TableAttribute>))
                .Returns(returnedType);

            // Act

            this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignType);

            // Assert

            this.tableTypeCacheMock.Verify(m => m.PopulateAssemblyCache(foreignType.Assembly, this.attributeDecorator.GetSingleAttribute<TableAttribute>), Times.Once);
        }

        [TestMethod]
        public void GetTableType_AssemblyCacheIsPopulated_Test()
        {
            // Arrange

            Type foreignType = typeof(ForeignClass);

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);

            Type returnedType = typeof(PrimaryClass);

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(foreignType.Assembly)).Returns(true);
            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignType, this.attributeDecorator.GetSingleAttribute<TableAttribute>))
                .Returns(returnedType);

            // Act

            this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignType);

            // Assert

            this.tableTypeCacheMock.Verify(m => m.PopulateAssemblyCache(foreignType.Assembly, this.attributeDecorator.GetSingleAttribute<TableAttribute>), Times.Never);
        }

        [TestMethod]
        public void GetTableType_TableTypeCache_GetCachedTableType_Test()
        {
            // Arrange

            Type foreignType = typeof(ForeignClass);

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);

            Type expected = typeof(PrimaryClass);

            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignType, this.attributeDecorator.GetSingleAttribute<TableAttribute>))
                .Returns(expected);

            // Act

            Type result = this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignType);

            // Assert

            Assert.AreEqual(expected, result);
        }

        #endregion GetTableType tests
    }
}
