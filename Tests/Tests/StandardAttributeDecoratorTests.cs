/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using TestDataFramework.ValueGenerator.Interfaces;
using Tests.TestModels;
using Tests.TestModels.Simple;

namespace Tests.Tests
{
    [TestClass]
    public class StandardAttributeDecoratorTests
    {
        private StandardAttributeDecorator attributeDecorator;
        private Mock<AssemblyWrapper> callingAssemblyWrapperMock;
        private Populator populator;
        private Schema schema = new Schema {Value = "Default Schema"};

        private Mock<StandardTableTypeCache> tableTypeCacheMock;

        [TestInitialize]
        public void TestInitialize()
        {
            this.tableTypeCacheMock = new Mock<StandardTableTypeCache>(null);
            this.callingAssemblyWrapperMock = new Mock<AssemblyWrapper>();
            this.schema = new Schema();
            this.attributeDecorator = new StandardAttributeDecorator(this.tableTypeCacheMock.Object,
                this.callingAssemblyWrapperMock.Object, this.schema);
            this.populator = new Populator(this.attributeDecorator);
        }

        #region GetUniqueAttributes Tests

        [TestMethod]
        public void GetUniqueAttributes_Test()
        {
            // Act

            IEnumerable<MaxAttribute> result =
                this.attributeDecorator.GetUniqueAttributes<MaxAttribute>(typeof(SubjectClass));

            // Assert

            List<MaxAttribute> attributes = result.ToList();

            Assert.AreEqual(3, attributes.Count);
            Assert.IsTrue(attributes.All(attribute => attribute.Max == SubjectClass.Max));
        }

        #endregion GetUniqueAttributes Tests

        private class Populator : BasePopulator
        {
            public Populator(IAttributeDecorator attributeDecorator) : base(attributeDecorator)
            {
            }

            public override IValueGenerator ValueGenerator { get; }

            public override void Bind()
            {
                throw new NotImplementedException();
            }

            internal override void Bind(RecordReference recordReference)
            {
                throw new NotImplementedException();
            }

            public override OperableListEx<T> Add<T>(int copies, params RecordReference[] primaryRecordReferences)
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

            public override void Clear()
            {
                throw new NotImplementedException();
            }
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
                    typeof(AttributeReadWriteTestClass).GetPropertyInfoProxy("Key1"));

            // Assert

            Assert.AreEqual(PrimaryKeyAttribute.KeyTypeEnum.Auto, attribute.KeyType);
        }

        [TestMethod]
        public void GetCustomAttribute_Declarative_Test()
        {
            // Act

            var attribute =
                this.attributeDecorator.GetCustomAttribute<StringLengthAttribute>(
                    typeof(AttributeReadWriteTestClass).GetPropertyInfoProxy("Text"));

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
                    typeof(AttributeReadWriteTestClass).GetPropertyInfoProxy("Text"));

            var stringLengthAttribute =
                this.attributeDecorator.GetCustomAttribute<StringLengthAttribute>(
                    typeof(AttributeReadWriteTestClass).GetPropertyInfoProxy("Text"));

            // Assert

            Assert.AreEqual(PrimaryKeyAttribute.KeyTypeEnum.Auto, primaryKeyAttribute.KeyType);
            Assert.AreEqual(20, stringLengthAttribute.Length);
        }

        #endregion GetCustomAttribute Tests (Returns single value)

        #region GetSingleAttribute

        [TestMethod]
        public void GetSingleAttribute_FindAnAttribute_Test()
        {
            var result =
                this.attributeDecorator
                    .GetSingleAttribute<PrimaryKeyAttribute>(typeof(PrimaryTable).GetPropertyInfoProxy("Key"));

            Assert.AreEqual(PrimaryKeyAttribute.KeyTypeEnum.Auto, result.KeyType);
        }

        [TestMethod]
        public void GetSingleAttribute_DoNotFindAnAttribute_Test()
        {
            var result =
                this.attributeDecorator.GetSingleAttribute<StringLengthAttribute>(
                    typeof(PrimaryTable).GetPropertyInfoProxy("Key"));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetSingleAttribute_DuplicatePropertyAttributesThrow_Test()
        {
            this.GetSingleAttribute_DuplicateAttributesThrow_Test(Messages.AmbigousPropertyAttributeMatch,
                typeof(AmbiguousAttributeClass).GetPropertyInfoProxy("A"));
        }

        [TestMethod]
        public void GetSingleAttribute_DuplicateClassAttributesThrow_Test()
        {
            this.GetSingleAttribute_DuplicateAttributesThrow_Test(Messages.AmbigousTypeAttributeMatch,
                new TypeInfoWrapper(typeof(AmbiguousAttributeClass)));
        }

        [TestMethod]
        public void GetSingleAttribute_DuplicateFieldAttributesThrow_Test()
        {
            this.GetSingleAttribute_DuplicateAttributesThrow_Test(Messages.AmbigousAttributeMatch,
                new TypeInfoWrapper(typeof(AmbiguousAttributeClass)).GetField("B"));
        }

        private void GetSingleAttribute_DuplicateAttributesThrow_Test(string message, MemberInfoProxy memberInfo)
        {
            Func<MemberInfoProxy, MultiAllowedAttribute> func = this.attributeDecorator
                .GetSingleAttribute<MultiAllowedAttribute>;
            string funcMessage = string.Format(message, typeof(MultiAllowedAttribute), memberInfo.Name,
                memberInfo.DeclaringType);

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
                    typeof(AttributeReadWriteTestClass).GetPropertyInfoProxy("Key2")).ToList();

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
                    typeof(AttributeReadWriteTestClass).GetPropertyInfoProxy("MultiAllowedProperty")).ToList();

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
                    typeof(AttributeReadWriteTestClass).GetPropertyInfoProxy("MultiAllowedProperty")).ToList();

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
                this.attributeDecorator.GetCustomAttributes(typeof(AttributeReadWriteTestClass).GetPropertyInfoProxy("Key2"))
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
                    typeof(AttributeReadWriteTestClass).GetPropertyInfoProxy("MultiAtributeProperty"));

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
                .AddAttributeToMember(c => c.MultiAllowedProperty, new MultiAllowedAttribute {I = 55});

            // Act.

            IEnumerable<Attribute> attributes =
                this.attributeDecorator.GetCustomAttributes(
                    typeof(AttributeReadWriteTestClass).GetPropertyInfoProxy("MultiAllowedProperty")).ToList();

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
                this.attributeDecorator.GetCustomAttributes<TableAttribute>(new TypeInfoWrapper(typeof(AttributeReadWriteTestClass)))
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
                this.attributeDecorator.GetCustomAttributes<MultiAllowedAttribute>(new TypeInfoWrapper(typeof(AttributeReadWriteTestClass)))
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
                .AddAttributeToType(new MultiAllowedAttribute {I = 55});

            // Act.

            IEnumerable<Attribute> attributes =
                this.attributeDecorator.GetCustomAttributes(
                    new TypeInfoWrapper(typeof(AttributeReadWriteTestClass))).ToList();

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

            var foreignKeyAtribute = new ForeignKeyAttribute(typeof(PrimaryClass), null);

            // Act

            Type result = this.attributeDecorator.GetTableType(foreignKeyAtribute, null);

            // Assert

            Assert.AreEqual(typeof(PrimaryClass), result);
        }

        [TestMethod]
        public void GetTableType_AssemblyCacheIsNotPopulated_Test()
        {
            // Arrange

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);
            var foreignTypeMock = new Mock<TypeInfoWrapper>();
            var foreignTypeAssembly = new AssemblyWrapper();
            foreignTypeMock.SetupGet(m => m.Assembly).Returns(foreignTypeAssembly);
            var expectedWrapper = new Mock<TypeInfoWrapper>();
            expectedWrapper.SetupGet(m => m.Type).Returns(typeof(PrimaryClass));

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(this.callingAssemblyWrapperMock.Object))
                .Returns(false).Verifiable();

            this.tableTypeCacheMock.Setup(m => m.PopulateAssemblyCache(this.callingAssemblyWrapperMock.Object,
                this.attributeDecorator.GetSingleAttribute<TableAttribute>, this.schema.Value)).Verifiable();

            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignTypeMock.Object,
                this.callingAssemblyWrapperMock.Object, this.attributeDecorator.GetSingleAttribute<TableAttribute>,
                true)).Returns((TypeInfoWrapper) null).Verifiable();

            this.tableTypeCacheMock.Setup(m => m.PopulateAssemblyCache(foreignTypeAssembly,
                this.attributeDecorator.GetSingleAttribute<TableAttribute>, this.schema.Value));

            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignTypeMock.Object,
                foreignTypeAssembly, this.attributeDecorator.GetSingleAttribute<TableAttribute>,
                false)).Returns(expectedWrapper.Object).Verifiable();

            // Act

            Type result = this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignTypeMock.Object);

            // Assert

            this.tableTypeCacheMock.Verify();

            Assert.AreEqual(typeof(PrimaryClass), result);
        }

        [TestMethod]
        public void GetTableType_TableTypeCache_IsNot_Populated_Test()
        {
            // Arrange

            var foreignTypeMock = new Mock<TypeInfoWrapper>();
            var foreignTypeAssembly = new AssemblyWrapper();
            foreignTypeMock.SetupGet(m => m.Assembly).Returns(foreignTypeAssembly);

            var foreignKeyAtribute = new ForeignKeyAttribute("PlaceHolder", null);

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(this.callingAssemblyWrapperMock.Object))
                .Returns(true).Verifiable();

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(foreignTypeAssembly))
                .Returns(false).Verifiable();

            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignTypeMock.Object,
                this.callingAssemblyWrapperMock.Object, this.attributeDecorator.GetSingleAttribute<TableAttribute>,
                true)).Returns((TypeInfoWrapper) null).Verifiable();

            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignTypeMock.Object,
                foreignTypeAssembly, this.attributeDecorator.GetSingleAttribute<TableAttribute>,
                false)).Returns((TypeInfoWrapper) null).Verifiable();

            // Act
            // Assert

            Helpers.ExceptionTest(
                () => this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignTypeMock.Object),
                typeof(AttributeDecoratorException),
                string.Format(Messages.CannotResolveForeignKey, foreignKeyAtribute, foreignTypeMock.Object));

            this.tableTypeCacheMock.Verify();

            this.tableTypeCacheMock.Verify(m => m.PopulateAssemblyCache(this.callingAssemblyWrapperMock.Object,
                this.attributeDecorator.GetSingleAttribute<TableAttribute>, this.schema.Value), Times.Never);

            this.tableTypeCacheMock.Verify(m => m.PopulateAssemblyCache(foreignTypeAssembly,
                this.attributeDecorator.GetSingleAttribute<TableAttribute>, this.schema.Value), Times.Once);
        }

        [TestMethod]
        public void GetTableType_TableTypeCache_Is_Populated_ButCannot_Resolve_ForeignKey_Test()
        {
            // Arrange

            var foreignTypeMock = new Mock<TypeInfoWrapper>();
            var foreignTypeAssembly = new AssemblyWrapper();
            foreignTypeMock.SetupGet(m => m.Assembly).Returns(foreignTypeAssembly);

            var foreignKeyAtribute = new ForeignKeyAttribute("PlaceHolder", null);

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(this.callingAssemblyWrapperMock.Object))
                .Returns(true).Verifiable();

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(foreignTypeAssembly))
                .Returns(true).Verifiable();

            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignTypeMock.Object,
                this.callingAssemblyWrapperMock.Object, this.attributeDecorator.GetSingleAttribute<TableAttribute>,
                true)).Returns((TypeInfoWrapper) null).Verifiable();

            // Act
            // Assert

            Helpers.ExceptionTest(
                () => this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignTypeMock.Object),
                typeof(AttributeDecoratorException),
                string.Format(Messages.CannotResolveForeignKey, foreignKeyAtribute, foreignTypeMock.Object));

            this.tableTypeCacheMock.Verify();

            this.tableTypeCacheMock.Verify(m => m.PopulateAssemblyCache(this.callingAssemblyWrapperMock.Object,
                this.attributeDecorator.GetSingleAttribute<TableAttribute>, this.schema.Value), Times.Never);

            this.tableTypeCacheMock.Verify(m => m.PopulateAssemblyCache(foreignTypeAssembly,
                this.attributeDecorator.GetSingleAttribute<TableAttribute>, this.schema.Value), Times.Never);

            this.tableTypeCacheMock.Verify(m => m.GetCachedTableType(foreignKeyAtribute, foreignTypeMock.Object,
                foreignTypeAssembly, this.attributeDecorator.GetSingleAttribute<TableAttribute>,
                false), Times.Never());
        }

        [TestMethod]
        public void GetTableType_Cannot_Find_ForeignKey_In_ForeignKeyType_Assembly()
        {
            // Arrange

            var foreignType = new TypeInfoWrapper(typeof(ForeignClass));

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(new AssemblyWrapper())).Returns(false);

            // Act
            // Assert

            Helpers.ExceptionTest(() => this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignType),
                typeof(AttributeDecoratorException),
                string.Format(Messages.CannotResolveForeignKey, foreignKeyAtribute, foreignType));

            this.tableTypeCacheMock.Verify(m => m.PopulateAssemblyCache(It.IsAny<AssemblyWrapper>(),
                this.attributeDecorator.GetSingleAttribute<TableAttribute>, null), Times.Exactly(2));

            this.tableTypeCacheMock.Verify(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                    It.IsAny<AssemblyWrapper>(), this.attributeDecorator.GetSingleAttribute<TableAttribute>, true),
                Times.Once);

            this.tableTypeCacheMock.Verify(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                    It.IsAny<AssemblyWrapper>(), this.attributeDecorator.GetSingleAttribute<TableAttribute>, false),
                Times.Once);
        }

        [TestMethod]
        public void GetTableType_ForeignKey_Found_In_ForeignKeyType_Assembly()
        {
            // Arrange

            var foreignType = new TypeInfoWrapper();
            var typeInfoWrapperMock = new Mock<TypeInfoWrapper>();
            Type primaryKeyTableType = typeof(PrimaryTable);
            typeInfoWrapperMock.SetupGet(m => m.Type).Returns(primaryKeyTableType);

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(new AssemblyWrapper())).Returns(false);

            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                    It.IsAny<AssemblyWrapper>(), this.attributeDecorator.GetSingleAttribute<TableAttribute>, false))
                .Returns(typeInfoWrapperMock.Object);

            // Act

            Type result = this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignType);

            // Assert

            Assert.AreEqual(primaryKeyTableType, result);

            this.tableTypeCacheMock.Verify(m => m.PopulateAssemblyCache(It.IsAny<AssemblyWrapper>(),
                this.attributeDecorator.GetSingleAttribute<TableAttribute>, null), Times.Exactly(2));

            this.tableTypeCacheMock.Verify(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                    It.IsAny<AssemblyWrapper>(), this.attributeDecorator.GetSingleAttribute<TableAttribute>, true),
                Times.Once);

            this.tableTypeCacheMock.Verify(m => m.GetCachedTableType(foreignKeyAtribute, foreignType,
                    It.IsAny<AssemblyWrapper>(), this.attributeDecorator.GetSingleAttribute<TableAttribute>, false),
                Times.Once);
        }

        [TestMethod]
        public void GetTableType_AssemblyCacheIsPopulated_Test()
        {
            // Arrange

            Type foreignType = typeof(ForeignClass);
            var assembly = new AssemblyWrapper();
            var foreignTypeMock = new Mock<TypeInfoWrapper>();
            foreignTypeMock.SetupGet(m => m.Assembly).Returns(assembly);
            foreignTypeMock.SetupGet(m => m.Type).Returns(foreignType);

            var foreignKeyAtribute = new ForeignKeyAttribute("PrimaryClass", null);

            Type cachedTableType = typeof(PrimaryClass);
            var returnedTypeMock = new Mock<TypeInfoWrapper>();
            returnedTypeMock.SetupGet(m => m.Type).Returns(cachedTableType);

            this.tableTypeCacheMock.Setup(m => m.IsAssemblyCachePopulated(assembly)).Returns(true);

            this.tableTypeCacheMock.Setup(m => m.GetCachedTableType(foreignKeyAtribute, foreignTypeMock.Object,
                    this.callingAssemblyWrapperMock.Object, this.attributeDecorator.GetSingleAttribute<TableAttribute>,
                    true))
                .Returns(returnedTypeMock.Object);

            // Act

            Type result = this.attributeDecorator.GetTableType(foreignKeyAtribute, foreignTypeMock.Object);

            // Assert

            this.tableTypeCacheMock.Verify(
                m => m.PopulateAssemblyCache(assembly, this.attributeDecorator.GetSingleAttribute<TableAttribute>,
                    this.schema.Value), Times.Never);

            Assert.AreEqual(cachedTableType, result);
        }

        #endregion GetTableType tests

        #region Default schema handling

        [TestMethod]
        public void GetCustomAttributes_NonGeneric_InsertDefaultSchema_ForeignKeyAttribute_Test()
        {
            // Arrange

            const string defaultSchema = "defaultSchema123";

            var attributeDecorator =
                new StandardAttributeDecorator(null, new AssemblyWrapper(), new Schema {Value = defaultSchema});

            // Act

            IEnumerable<Attribute> resultSet =
                attributeDecorator.GetCustomAttributes(typeof(ForeignClass).GetPropertyInfoProxy("ForeignKey"));

            Attribute result = resultSet.First();

            // Assert

            Assert.AreEqual(defaultSchema, ((ForeignKeyAttribute) result).Schema);
        }

        [TestMethod]
        public void GetCustomAttributes_NonGeneric_InsertDefaultSchema_ExplicitSchema_Test()
        {
            // Arrange

            var attributeDecorator = new StandardAttributeDecorator(null, new AssemblyWrapper(), new Schema());

            // Act

            IEnumerable<Attribute> resultSet =
                attributeDecorator.GetCustomAttributes(typeof(ClassWithSchemaInForeignKey).GetPropertyInfoProxy("ForeignKey"));

            Attribute result = resultSet.First();

            // Assert

            Assert.AreEqual(ClassWithSchemaInForeignKey.Schema, ((ForeignKeyAttribute) result).Schema);
        }

        [TestMethod]
        public void GetCustomAttributes_Generic_InsertDefaultSchema_ForeignKeyAttribute_Test()
        {
            // Arrange

            const string defaultSchema = "defaultSchema123";

            var attributeDecorator =
                new StandardAttributeDecorator(null, new AssemblyWrapper(), new Schema {Value = defaultSchema});

            // Act

            IEnumerable<ForeignKeyAttribute> resultSet =
                attributeDecorator.GetCustomAttributes<ForeignKeyAttribute>(
                    typeof(ForeignClass).GetPropertyInfoProxy("ForeignKey"));

            ForeignKeyAttribute result = resultSet.First();

            // Assert

            Assert.AreEqual(defaultSchema, result.Schema);
        }

        [TestMethod]
        public void GetCustomAttributes_Generic_InsertDefaultSchema_ExplicitSchema_Test()
        {
            // Arrange

            var attributeDecorator = new StandardAttributeDecorator(null, new AssemblyWrapper(), new Schema());

            // Act

            IEnumerable<ForeignKeyAttribute> resultSet =
                attributeDecorator.GetCustomAttributes<ForeignKeyAttribute>(
                    typeof(ClassWithSchemaInForeignKey).GetPropertyInfoProxy("ForeignKey"));

            ForeignKeyAttribute result = resultSet.First();

            // Assert

            Assert.AreEqual(ClassWithSchemaInForeignKey.Schema, result.Schema);
        }

        #endregion Default schema handling
    }
}