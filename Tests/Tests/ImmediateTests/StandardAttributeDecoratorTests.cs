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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.ValueGenerator.Interfaces;
using Tests.TestModels;
using Tests.TestModels.Simple;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class StandardAttributeDecoratorTests
    {
        private class Populator : BasePopulator
        {
            public Populator(IAttributeDecorator attributeDecorator) : base(attributeDecorator)
            { }

            public override void Bind()
            {
                throw new NotImplementedException();
            }

            protected internal override void Bind(RecordReference recordReference)
            {
                throw new NotImplementedException();
            }

            protected internal override void Bind<T>(OperableList<T> operableList)
            {
                throw new NotImplementedException();
            }

            public override OperableList<T> Add<T>(int copies, params RecordReference[] primaryRecordReferences)
            {
                throw new NotImplementedException();
            }

            public override RecordReference<T> Add<T>(params RecordReference[] primaryRecordReferences)
            {
                throw new NotImplementedException();
            }

            public override void Extend(Type type, HandledTypeValueGetter valueGetter)
            {
                throw new NotImplementedException();
            }

            public override IValueGenerator ValueGenerator { get; }
            public override void Clear()
            {
                throw new NotImplementedException();
            }
        }

        private Populator populator;
        private Mock<StandardTableTypeCache> tableTypeCacheMock;
        private StandardAttributeDecorator attributeDecorator;

        [TestInitialize]
        public void TestInitialize()
        {
            this.tableTypeCacheMock = new Mock<StandardTableTypeCache>((Func<StandardTableTypeCache, ITableTypeCacheService>)(x => null));
            this.attributeDecorator = new StandardAttributeDecorator(this.tableTypeCacheMock.Object, this.GetType().Assembly, null);
            this.populator = new Populator(this.attributeDecorator);
        }

        #region GetCustomAttribute Tests (Returns single value)

        [TestMethod]
        public void GetCustomAttribute_Programmatic_Test()
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
        public void GetCustomAttribute_Declarative_Test()
        {
            // Act

            var attribute =
                this.attributeDecorator.GetCustomAttribute<StringLengthAttribute>(
                    typeof (AttributeReadWriteTestClass).GetProperty("Text"));

            // Assert

            Assert.AreEqual(20, attribute.Length);
        }

        [TestMethod]
        public void GetCustomAttribute_Mixed_Test()
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

        #endregion GetCustomAttribute Tests (Returns single value)

        #region GetSingleAttribute

        [TestMethod]
        public void GetSingleAttribute_FindAnAttribute_Test()
        {
            PrimaryKeyAttribute result = this.attributeDecorator.GetSingleAttribute<PrimaryKeyAttribute>(typeof (PrimaryTable).GetProperty("Key"));

            Assert.AreEqual(PrimaryKeyAttribute.KeyTypeEnum.Auto, result.KeyType);
        }

        [TestMethod]
        public void GetSingleAttribute_DoNotFindAnAttribute_Test()
        {
            StringLengthAttribute result = this.attributeDecorator.GetSingleAttribute<StringLengthAttribute>(typeof(PrimaryTable).GetProperty("Key"));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetSingleAttribute_DuplicatePropertyAttributesThrow_Test()
        {
            this.GetSingleAttribute_DuplicateAttributesThrow_Test(Messages.AmbigousPropertyAttributeMatch,
                typeof (AmbiguousAttributeClass).GetProperty("A"));
        }

        [TestMethod]
        public void GetSingleAttribute_DuplicateClassAttributesThrow_Test()
        {
            this.GetSingleAttribute_DuplicateAttributesThrow_Test(Messages.AmbigousTypeAttributeMatch,
                typeof(AmbiguousAttributeClass));
        }

        [TestMethod]
        public void GetSingleAttribute_DuplicateFieldAttributesThrow_Test()
        {
            this.GetSingleAttribute_DuplicateAttributesThrow_Test(Messages.AmbigousAttributeMatch,
                typeof(AmbiguousAttributeClass).GetField("B"));
        }

        private void GetSingleAttribute_DuplicateAttributesThrow_Test(string message, MemberInfo memberInfo)
        {
            Func<MemberInfo, MultiAllowedAttribute> func = this.attributeDecorator.GetSingleAttribute<MultiAllowedAttribute>;
            string funcMessage = string.Format(message, typeof (MultiAllowedAttribute), memberInfo.Name, memberInfo.DeclaringType);

            Helpers.ExceptionTest(
                () => func(memberInfo),
                typeof(AmbiguousMatchException),
                funcMessage
                );
        }

        #endregion GetSingleAttribute

        #region GetCustomAttributes<T> Tests (Returns many values)

        [TestMethod]
        public void GetCustomAttributesOfT_Programmatic_Test()
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
        public void GetCustomAttributesOfT_Declarative_Test()
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
        public void GetCustomAttributesOfT_Mixed_Test()
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

        #endregion GetCustomAttributes<T> Tests (Returns many values)

        #region GetCustomAttributes Non-generic Tests (Returns many values)

        [TestMethod]
        public void GetCustomAttributes_Programmatic_Test()
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
        public void GetCustomAttributes_Declarative_Test()
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
        public void GetCustomAttributes_Mixed_Test()
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

        #endregion GetCustomAttributes Non-generic Tests (Returns many values)

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

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(this.GetType().Assembly)).Returns(false);

            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                    this.GetType().Assembly, this.attributeDecorator.GetSingleAttribute<TableAttribute>, true))
                .Returns(returnedType);

            // Act

            this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignType);

            // Assert

            this.tableTypeCacheMock.Verify(
                m => m.PopulateAssemblyCache(foreignType.Assembly,
                    this.attributeDecorator.GetSingleAttribute<TableAttribute>, null), Times.Once);
        }

        [TestMethod]
        public void GetTableType_TableTypeCache_IsNot_Populated_Test()
        {
            // Arrange

            Type foreignType = typeof(ForeignClass);

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(this.GetType().Assembly)).Returns(true);

            // Act
            // Assert

            Helpers.ExceptionTest(() => this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignType),
                    typeof(AttributeDecoratorException),
                    string.Format(Messages.CannotResolveForeignKey, foreignKeyAtribute, foreignType));
        }

        [TestMethod]
        public void GetTableType_Cannot_Find_ForeignKey_In_ForeignKeyType_Assembly()
        {
            // Arrange

            Type foreignType = typeof(ForeignClass);

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(this.GetType().Assembly)).Returns(false);

            // Act
            // Assert

            Helpers.ExceptionTest(() => this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignType),
                typeof(AttributeDecoratorException),
                string.Format(Messages.CannotResolveForeignKey, foreignKeyAtribute, foreignType));

            this.tableTypeCacheMock.Verify(m => m.PopulateAssemblyCache(It.IsAny<Assembly>(),
                this.attributeDecorator.GetSingleAttribute<TableAttribute>, null), Times.Exactly(2));

            this.tableTypeCacheMock.Verify(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                It.IsAny<Assembly>(), this.attributeDecorator.GetSingleAttribute<TableAttribute>, true), Times.Once);

            this.tableTypeCacheMock.Verify(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                It.IsAny<Assembly>(), this.attributeDecorator.GetSingleAttribute<TableAttribute>, false), Times.Once);
        }

        [TestMethod]
        public void GetTableType_ForeignKey_Found_In_ForeignKeyType_Assembly()
        {
            // Arrange

            Type foreignType = typeof(ForeignClass);
            Type primaryKeyTableType = typeof(PrimaryTable);

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(this.GetType().Assembly)).Returns(false);

            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                    It.IsAny<Assembly>(), this.attributeDecorator.GetSingleAttribute<TableAttribute>, false))
                .Returns(primaryKeyTableType);

            // Act

            Type result = this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignType);

            // Assert

            Assert.AreEqual(primaryKeyTableType, result);

            this.tableTypeCacheMock.Verify(m => m.PopulateAssemblyCache(It.IsAny<Assembly>(),
                this.attributeDecorator.GetSingleAttribute<TableAttribute>, null), Times.Exactly(2));

            this.tableTypeCacheMock.Verify(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                It.IsAny<Assembly>(), this.attributeDecorator.GetSingleAttribute<TableAttribute>, true), Times.Once);

            this.tableTypeCacheMock.Verify(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                It.IsAny<Assembly>(), this.attributeDecorator.GetSingleAttribute<TableAttribute>, false), Times.Once);
        }

        [TestMethod]
        public void GetTableType_AssemblyCacheIsPopulated_Test()
        {
            // Arrange

            Type foreignType = typeof(ForeignClass);

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);

            Type returnedType = typeof(PrimaryClass);

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(foreignType.Assembly)).Returns(true);
            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                    this.GetType().Assembly, this.attributeDecorator.GetSingleAttribute<TableAttribute>, true))
                .Returns(returnedType);

            // Act

            this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignType);

            // Assert

            this.tableTypeCacheMock.Verify(m => m.PopulateAssemblyCache(foreignType.Assembly, this.attributeDecorator.GetSingleAttribute<TableAttribute>, null), Times.Never);
        }

        [TestMethod]
        public void GetTableType_TableTypeCache_GetCachedTableType_Test()
        {
            // Arrange

            Type foreignType = typeof(ForeignClass);

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);

            Type expected = typeof(PrimaryClass);

            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignType, this.GetType().Assembly, this.attributeDecorator.GetSingleAttribute<TableAttribute>, true))
                .Returns(expected);

            // Act

            Type result = this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignType);

            // Assert

            Assert.AreEqual(expected, result);
        }

        #endregion GetTableType tests

        #region Default schema handling

        [TestMethod]
        public void GetCustomAttributes_NonGeneric_InsertDefaultSchema_ForeignKeyAttribute_Test()
        {
            // Arrange

            const string defaultSchema = "defaultSchema123";

            var attributeDecorator = new StandardAttributeDecorator(null, null, defaultSchema);

            // Act

            IEnumerable<Attribute> resultSet =
                attributeDecorator.GetCustomAttributes(typeof (ForeignClass).GetProperty("ForeignKey"));

            Attribute result = resultSet.First();

            // Assert

            Assert.AreEqual(defaultSchema, ((ForeignKeyAttribute)result).Schema);
        }

        [TestMethod]
        public void GetCustomAttributes_NonGeneric_InsertDefaultSchema_ExplicitSchema_Test()
        {
            // Arrange

            const string defaultSchema = "defaultSchema123";

            var attributeDecorator = new StandardAttributeDecorator(null, null, defaultSchema);

            // Act

            IEnumerable<Attribute> resultSet =
                attributeDecorator.GetCustomAttributes(typeof(ClassWithSchemaInForeignKey).GetProperty("ForeignKey"));

            Attribute result = resultSet.First();

            // Assert

            Assert.AreEqual(ClassWithSchemaInForeignKey.Schema, ((ForeignKeyAttribute)result).Schema);
        }

        [TestMethod]
        public void GetCustomAttributes_Generic_InsertDefaultSchema_ForeignKeyAttribute_Test()
        {
            // Arrange

            const string defaultSchema = "defaultSchema123";

            var attributeDecorator = new StandardAttributeDecorator(null, null, defaultSchema);

            // Act

            IEnumerable<ForeignKeyAttribute> resultSet =
                attributeDecorator.GetCustomAttributes<ForeignKeyAttribute>(
                    typeof (ForeignClass).GetProperty("ForeignKey"));

            ForeignKeyAttribute result = resultSet.First();

            // Assert

            Assert.AreEqual(defaultSchema, result.Schema);
        }

        [TestMethod]
        public void GetCustomAttributes_Generic_InsertDefaultSchema_ExplicitSchema_Test()
        {
            // Arrange

            const string defaultSchema = "defaultSchema123";

            var attributeDecorator = new StandardAttributeDecorator(null, null, defaultSchema);

            // Act

            IEnumerable<ForeignKeyAttribute> resultSet =
                attributeDecorator.GetCustomAttributes<ForeignKeyAttribute>(
                    typeof (ClassWithSchemaInForeignKey).GetProperty("ForeignKey"));

            ForeignKeyAttribute result = resultSet.First();

            // Assert

            Assert.AreEqual(ClassWithSchemaInForeignKey.Schema, result.Schema);
        }

        #endregion Default schema handling

        #region GetUniqueAttributes Tests

        [TestMethod]
        public void GetUniqueAttributes_Test()
        {
            // Act

            IEnumerable<MaxAttribute> result = this.attributeDecorator.GetUniqueAttributes<MaxAttribute>(typeof(SubjectClass));

            // Assert

            List<MaxAttribute> attributes = result.ToList();

            Assert.AreEqual(3, attributes.Count);
            Assert.IsTrue(attributes.All(attribute => attribute.Max == SubjectClass.Max));
        }

        #endregion GetUniqueAttributes Tests
    }
}
