using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using Tests.TestModels;

namespace Tests.Tests.TableTypeCacheServiceTests
{
    [TestClass]
    public class StandardTableTypeCacheServiceTests
    {
        private AssemblyLookupContext assemblyLookupContext;

        private ForeignKeyAttribute foreignKeyAttribute;

        private Table foreignKeyAttributeTable;
        private StandardTableTypeCacheService service;
        private TableAttribute tableAttribute;
        private Mock<TableTypeLookup> tableTypeLookupMock;

        [TestInitialize]
        public void Initialize()
        {
            this.assemblyLookupContext = new AssemblyLookupContext();

            this.tableTypeLookupMock = new Mock<TableTypeLookup>(null);
            this.service = new StandardTableTypeCacheService(this.tableTypeLookupMock.Object);

            this.foreignKeyAttribute = new ForeignKeyAttribute("schema", "primaryTableName", "primaryKeyName");
            this.tableAttribute = new TableAttribute("catalogueName", "schema", "tableName");
            this.foreignKeyAttributeTable = new Table(this.foreignKeyAttribute, this.tableAttribute);
        }

        private void VerifyGetTableTypeByCriteria(TypeDictionaryEqualityComparer.EqualsCriteriaDelegate matchCriteria)
        {
            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeByCriteria(
                    It.Is<Table>(table => table.CatalogueName == this.foreignKeyAttributeTable.CatalogueName &&
                                          table.TableName == this.foreignKeyAttributeTable.TableName && table.Schema ==
                                          this.foreignKeyAttributeTable.Schema),
                    matchCriteria, this.assemblyLookupContext), Times.Once);
        }

        [TestMethod]
        public void CompleteMatch_IsFirst_Test()
        {
            // Arrange

            this.tableTypeLookupMock.Setup(m => m.GetTableTypeByCriteria(It.IsAny<Table>(),
                    It.Is<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate>(
                        func => func == TableTypeCriteria.CompleteCatalogueMatchCriteria), this.assemblyLookupContext))
                .Returns(new TypeInfoWrapper(typeof(SubjectClass)));

            // Act

            TypeInfoWrapper result = this.service.GetCachedTableType(this.foreignKeyAttribute, this.tableAttribute,
                this.assemblyLookupContext);

            // Assert

            Assert.AreEqual(new TypeInfoWrapper(typeof(SubjectClass)), result);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.CompleteCatalogueMatchCriteria);

            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeWithCatalogue(It.IsAny<Table>(), It.IsAny<AssemblyLookupContext>()), Times.Never);

            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeByCriteria(It.IsAny<Table>(), TableTypeCriteria.MatchOnWhatIsDecorated,
                    It.IsAny<AssemblyLookupContext>()), Times.Never);

            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeByCriteria(It.IsAny<Table>(), TableTypeCriteria.MatchOnEverythingNotAlreadyTried,
                    It.IsAny<AssemblyLookupContext>()), Times.Never);
        }

        [TestMethod]
        public void GetTableTypeWithCatalogue_IsSecond_Test()
        {
            // Arrange

            this.tableTypeLookupMock.Setup(m => m.GetTableTypeByCriteria(It.IsAny<Table>(),
                    It.Is<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate>(
                        func => func == TableTypeCriteria.CompleteCatalogueMatchCriteria), this.assemblyLookupContext))
                .Returns((TypeInfoWrapper) null);

            this.tableTypeLookupMock
                .Setup(m => m.GetTableTypeWithCatalogue(It.IsAny<Table>(), this.assemblyLookupContext))
                .Returns(new TypeInfoWrapper(typeof(SubjectClass)));

            // Act

            TypeInfoWrapper result = this.service.GetCachedTableType(this.foreignKeyAttribute, this.tableAttribute,
                this.assemblyLookupContext);


            // Assert

            Assert.AreEqual(new TypeInfoWrapper(typeof(SubjectClass)), result);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.CompleteCatalogueMatchCriteria);

            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeWithCatalogue(It.IsAny<Table>(), It.IsAny<AssemblyLookupContext>()), Times.Once);

            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeByCriteria(It.IsAny<Table>(), TableTypeCriteria.MatchOnWhatIsDecorated,
                    It.IsAny<AssemblyLookupContext>()), Times.Never);

            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeByCriteria(It.IsAny<Table>(), TableTypeCriteria.MatchOnEverythingNotAlreadyTried,
                    It.IsAny<AssemblyLookupContext>()), Times.Never);
        }

        [TestMethod]
        public void MatchWhatIsDecorated_IsThird_Test()
        {
            // Arrange

            this.tableTypeLookupMock.Setup(m => m.GetTableTypeByCriteria(It.IsAny<Table>(),
                    It.Is<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate>(
                        func => func == TableTypeCriteria.CompleteCatalogueMatchCriteria), this.assemblyLookupContext))
                .Returns((TypeInfoWrapper) null);

            this.tableTypeLookupMock
                .Setup(m => m.GetTableTypeWithCatalogue(It.IsAny<Table>(), this.assemblyLookupContext))
                .Returns((TypeInfoWrapper) null);

            this.tableTypeLookupMock.Setup(m => m.GetTableTypeByCriteria(It.IsAny<Table>(),
                    It.Is<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate>(
                        func => func == TableTypeCriteria.MatchOnWhatIsDecorated), this.assemblyLookupContext))
                .Returns(new TypeInfoWrapper(typeof(SubjectClass)));

            // Act

            TypeInfoWrapper result = this.service.GetCachedTableType(this.foreignKeyAttribute, this.tableAttribute,
                this.assemblyLookupContext);

            // Assert

            Assert.AreEqual(new TypeInfoWrapper(typeof(SubjectClass)), result);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.CompleteCatalogueMatchCriteria);

            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeWithCatalogue(It.IsAny<Table>(), It.IsAny<AssemblyLookupContext>()), Times.Once);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.MatchOnWhatIsDecorated);

            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeByCriteria(It.IsAny<Table>(), TableTypeCriteria.MatchOnEverythingNotAlreadyTried,
                    It.IsAny<AssemblyLookupContext>()), Times.Never);
        }

        [TestMethod]
        public void MatchOnEverythingNotAlreadyTried_IsFourth_Test()
        {
            // Arrange

            this.tableTypeLookupMock.Setup(m => m.GetTableTypeByCriteria(It.IsAny<Table>(),
                    It.Is<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate>(
                        func => func == TableTypeCriteria.CompleteCatalogueMatchCriteria), this.assemblyLookupContext))
                .Returns((TypeInfoWrapper) null);

            this.tableTypeLookupMock
                .Setup(m => m.GetTableTypeWithCatalogue(It.IsAny<Table>(), this.assemblyLookupContext))
                .Returns((TypeInfoWrapper) null);

            this.tableTypeLookupMock.Setup(m => m.GetTableTypeByCriteria(It.IsAny<Table>(),
                    It.Is<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate>(
                        func => func == TableTypeCriteria.MatchOnWhatIsDecorated), this.assemblyLookupContext))
                .Returns((TypeInfoWrapper) null);

            this.tableTypeLookupMock.Setup(m => m.GetTableTypeByCriteria(It.IsAny<Table>(),
                    It.Is<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate>(
                        func => func == TableTypeCriteria.MatchOnEverythingNotAlreadyTried),
                    this.assemblyLookupContext))
                .Returns(new TypeInfoWrapper(typeof(SubjectClass)));

            // Act

            TypeInfoWrapper result = this.service.GetCachedTableType(this.foreignKeyAttribute, this.tableAttribute,
                this.assemblyLookupContext);

            // Assert

            Assert.AreEqual(new TypeInfoWrapper(typeof(SubjectClass)), result);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.CompleteCatalogueMatchCriteria);

            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeWithCatalogue(It.IsAny<Table>(), It.IsAny<AssemblyLookupContext>()), Times.Once);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.MatchOnWhatIsDecorated);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.MatchOnEverythingNotAlreadyTried);
        }

        [TestMethod]
        public void NoMatch_Test()
        {
            // Arrange

            this.tableTypeLookupMock.Setup(m => m.GetTableTypeByCriteria(It.IsAny<Table>(),
                    It.Is<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate>(
                        func => func == TableTypeCriteria.CompleteCatalogueMatchCriteria), this.assemblyLookupContext))
                .Returns((TypeInfoWrapper) null);

            this.tableTypeLookupMock
                .Setup(m => m.GetTableTypeWithCatalogue(It.IsAny<Table>(), this.assemblyLookupContext))
                .Returns((TypeInfoWrapper) null);

            this.tableTypeLookupMock.Setup(m => m.GetTableTypeByCriteria(It.IsAny<Table>(),
                    It.Is<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate>(
                        func => func == TableTypeCriteria.MatchOnWhatIsDecorated), this.assemblyLookupContext))
                .Returns((TypeInfoWrapper) null);

            this.tableTypeLookupMock.Setup(m => m.GetTableTypeByCriteria(It.IsAny<Table>(),
                    It.Is<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate>(
                        func => func == TableTypeCriteria.MatchOnEverythingNotAlreadyTried),
                    this.assemblyLookupContext))
                .Returns((TypeInfoWrapper) null);

            // Act

            TypeInfoWrapper result = this.service.GetCachedTableType(this.foreignKeyAttribute, this.tableAttribute,
                this.assemblyLookupContext);

            // Assert

            Assert.IsNull(result);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.CompleteCatalogueMatchCriteria);

            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeWithCatalogue(It.IsAny<Table>(), It.IsAny<AssemblyLookupContext>()), Times.Once);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.MatchOnWhatIsDecorated);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.MatchOnEverythingNotAlreadyTried);
        }

        [TestMethod]
        public void TryAssociateTypeToTable_NoColision_AddsTo_TypeDictionary_Test()
        {
            // Arrange

            var definedType = new TypeInfoWrapper(typeof(SubjectClass));

            TableAttribute GetTableAttibute(TypeInfoWrapper type)
            {
                return null;
            }

            // Act

            this.service.TryAssociateTypeToTable(definedType, this.assemblyLookupContext, GetTableAttibute,
                null);

            // Assert

            bool containsTheEntry = this.assemblyLookupContext.TypeDictionary.Contains(
                new KeyValuePair<Table, TypeInfoWrapper>(new Table(definedType, null), definedType));

            Assert.IsTrue(containsTheEntry);

            Assert.IsFalse(this.assemblyLookupContext.CollisionDictionary.Any());
        }

        [TestMethod]
        public void TryAssociateTypeToTable_NoMatchingEntriesIn_CollisionDictionary_AddsTo_CollisionDictionary_Test()
        {
            // Arrange

            var definedType = new TypeInfoWrapper(typeof(SubjectClass));

            TableAttribute GetTableAttibute(TypeInfoWrapper type)
            {
                return null;
            }

            this.assemblyLookupContext.TypeDictionary.TryAdd(new Table(definedType, null), definedType);

            // Act

            this.service.TryAssociateTypeToTable(definedType, this.assemblyLookupContext, GetTableAttibute,
                null);

            // Assert

            bool containsTheEntry = this.assemblyLookupContext.TypeDictionary.Contains(
                new KeyValuePair<Table, TypeInfoWrapper>(new Table(definedType, null), definedType));

            Assert.IsTrue(containsTheEntry);
            Assert.AreEqual(1, this.assemblyLookupContext.TypeDictionary.Count);

            int collisionCount = this.assemblyLookupContext.CollisionDictionary.Count;
            Assert.AreEqual(1, collisionCount);

            var table = new Table(definedType, null);

            this.assemblyLookupContext.CollisionDictionary.TryGetValue(table,
                out List<TypeInfoWrapper> collisionTypes);

            Assert.IsNotNull(collisionTypes);
            Assert.AreEqual(2, collisionTypes.Count);

            Assert.AreEqual(definedType, collisionTypes[0]);
            Assert.AreEqual(definedType, collisionTypes[1]);
        }

        [TestMethod]
        public void TryAssociateTypeToTable_MatchingEntry_InCollisionDictionary_UpdatesTheDictionary_Test()
        {
            // Arrange

            var definedType = new TypeInfoWrapper(typeof(SubjectClass));

            TableAttribute GetTableAttibute(TypeInfoWrapper type)
            {
                return null;
            }

            var table = new Table(definedType, null);
            this.assemblyLookupContext.TypeDictionary.TryAdd(table, definedType);
            var collidedType = new TypeInfoWrapper(typeof(SecondClass));
            this.assemblyLookupContext.CollisionDictionary.TryAdd(table, new List<TypeInfoWrapper> {collidedType});

            // Act

            this.service.TryAssociateTypeToTable(definedType, this.assemblyLookupContext, GetTableAttibute,
                null);

            // Assert

            KeyValuePair<Table, List<TypeInfoWrapper>> collisionDictionaryEntry =
                this.assemblyLookupContext.CollisionDictionary.Single();

            Assert.AreEqual(table, collisionDictionaryEntry.Key);
            Assert.AreEqual(2, collisionDictionaryEntry.Value.Count);
            Assert.IsTrue(collisionDictionaryEntry.Value.Contains(definedType));
            Assert.IsTrue(collisionDictionaryEntry.Value.Contains(collidedType));
        }

        [TestMethod]
        public void GetCachedTableTypeUsingAllAssemblies_ReturnsType_Test()
        {
            // Arrange

            var getCachedTableTypeMock = new Mock<GetCachedTableType>();
            var assemblyLookupContext1 = new AssemblyLookupContext();
            var assemblyLookupContext2 = new AssemblyLookupContext();
            var type = new TypeInfoWrapper();

            // Act

            TypeInfoWrapper result = this.GetCachedTableTypeUsingAllAssemblies_Test(getCachedTableTypeMock,
                assemblyLookupContext1, assemblyLookupContext2, type);

            // Assert

            getCachedTableTypeMock.Verify(m => m(this.foreignKeyAttribute, this.tableAttribute,
                It.IsAny<AssemblyLookupContext>()), Times.Once());

            Assert.AreEqual(type, result);
        }

        [TestMethod]
        public void GetCachedTableTypeUsingAllAssemblies_NoCacheHits_ReturnsNull_Test()
        {
            // Arrange

            var getCachedTableTypeMock = new Mock<GetCachedTableType>();
            var assemblyLookupContext1 = new AssemblyLookupContext();
            var assemblyLookupContext2 = new AssemblyLookupContext();

            // Act

            TypeInfoWrapper result = this.GetCachedTableTypeUsingAllAssemblies_Test(getCachedTableTypeMock,
                assemblyLookupContext1, assemblyLookupContext2, null);

            // Assert

            getCachedTableTypeMock.Verify(m => m(this.foreignKeyAttribute, this.tableAttribute,
                assemblyLookupContext1), Times.Once());

            getCachedTableTypeMock.Verify(m => m(this.foreignKeyAttribute, this.tableAttribute,
                assemblyLookupContext2), Times.Once());

            getCachedTableTypeMock.VerifyNoOtherCalls();

            Assert.IsNull(result);
        }

        private TypeInfoWrapper GetCachedTableTypeUsingAllAssemblies_Test(
            Mock<GetCachedTableType> getCachedTableTypeMock,
            AssemblyLookupContext assemblyLookupContext1,
            AssemblyLookupContext assemblyLookupContext2,
            TypeInfoWrapper type)
        {
            // Arrange

            var tableTypeDictionary = new ConcurrentDictionary<AssemblyWrapper, AssemblyLookupContext>();

            tableTypeDictionary.TryAdd(new AssemblyWrapper(), assemblyLookupContext1);
            tableTypeDictionary.TryAdd(new AssemblyWrapper(), assemblyLookupContext2);

            getCachedTableTypeMock.Setup(m => m(this.foreignKeyAttribute, this.tableAttribute,
                    It.IsAny<AssemblyLookupContext>()))
                .Returns(type);

            // Act

            TypeInfoWrapper result = this.service.GetCachedTableTypeUsingAllAssemblies(this.foreignKeyAttribute,
                this.tableAttribute, getCachedTableTypeMock.Object, tableTypeDictionary);

            return result;
        }

        [TestMethod]
        public void EqualsCriteria_Prevents_Adding_Type_Test()
        {
            var definedType = new TypeInfoWrapper(typeof(SubjectClass));

            GetTableAttribute getTableAttribute;
            Table tableOfSet;

            TableAttribute GetTableAttributeWithCatalogue()
            {
                return new TableAttribute("catalogueName", "schema", "name");
            }

            TableAttribute GetTableAttributeWithoutCatalogue()
            {
                return new TableAttribute(null, "schema", "name");
            }

            getTableAttribute = type => GetTableAttributeWithCatalogue();
            tableOfSet = new Table(GetTableAttributeWithCatalogue());
            // Go
            this.EqualsCriteria_Prevents_Adding_Type(getTableAttribute, tableOfSet);

            getTableAttribute = type => GetTableAttributeWithoutCatalogue();
            tableOfSet = new Table(GetTableAttributeWithoutCatalogue());
            // Go
            this.EqualsCriteria_Prevents_Adding_Type(getTableAttribute, tableOfSet);

            getTableAttribute = type => null;
            tableOfSet = new Table(definedType, "schema");
            // Go
            this.EqualsCriteria_Prevents_Adding_Type(getTableAttribute, tableOfSet);
        }

        private void EqualsCriteria_Prevents_Adding_Type(GetTableAttribute getTableAttribute, Table tableOfSet)
        {
            // Arrange

            this.assemblyLookupContext = new AssemblyLookupContext();

            var definedType = new TypeInfoWrapper(typeof(SubjectClass));

            this.assemblyLookupContext.TypeDictionary.TryAdd(tableOfSet, definedType);

            // Act

            this.service.TryAssociateTypeToTable(definedType, this.assemblyLookupContext, getTableAttribute,
                "schema");

            // Assert

            Assert.AreEqual(1, this.assemblyLookupContext.TypeDictionary.Count);

            Assert.AreEqual(1, this.assemblyLookupContext.CollisionDictionary.Count);
            Assert.AreEqual(2, this.assemblyLookupContext.CollisionDictionary.First().Value.Count);
        }

        [TestMethod]
        public void PopulateAssemblyCache_Test()
        {
            // Arrange

            var appDomainWrapperMock = new Mock<AppDomainWrapper>();
            var assemblyName = new AssemblyNameWrapper();
            GetTableAttribute getTableAttribute = type => null;

            var tryAssociateTypeToTableMock = new Mock<TryAssociateTypeToTable>();
            var assemblyWrapperMock = new Mock<AssemblyWrapper>();
            appDomainWrapperMock.Setup(m => m.Load(assemblyName)).Returns(assemblyWrapperMock.Object);

            assemblyWrapperMock.SetupGet(m => m.DefinedTypes)
                .Returns(new[] {new TypeInfoWrapper(), new TypeInfoWrapper()});

            // Act

            this.service.PopulateAssemblyCache(appDomainWrapperMock.Object, assemblyName, getTableAttribute, "schema",
                tryAssociateTypeToTableMock.Object, this.assemblyLookupContext);

            // Assert

            appDomainWrapperMock.Verify(m => m.Load(assemblyName), Times.Once);
            assemblyWrapperMock.VerifyGet(m => m.DefinedTypes, Times.Once);

            tryAssociateTypeToTableMock.Verify(m => m(It.IsAny<TypeInfoWrapper>(), this.assemblyLookupContext,
                getTableAttribute, "schema"), Times.Exactly(2));
        }

        [TestMethod]
        public void PopulateAssemblyCache_AssemblyFileNotFound_Throws_Test()
        {
            var appDomainWrapperMock = new Mock<AppDomainWrapper>();
            var assemblyName = new AssemblyNameWrapper();

            appDomainWrapperMock.Setup(m => m.Load(assemblyName)).Throws<FileNotFoundException>();
            var tryAssociateTypeToTableMock = new Mock<TryAssociateTypeToTable>();

            // Act

            this.service.PopulateAssemblyCache(appDomainWrapperMock.Object, assemblyName, null, null,
                tryAssociateTypeToTableMock.Object, null);

            // Assert

            tryAssociateTypeToTableMock.Verify(m => m(It.IsAny<TypeInfoWrapper>(), It.IsAny<AssemblyLookupContext>(),
                It.IsAny<GetTableAttribute>(), It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public void PopulateAssemblyCache_TypeLoadException_Throws_Test()
        {
            // Arrange

            var appDomainWrapperMock = new Mock<AppDomainWrapper>();
            var assemblyName = new AssemblyNameWrapper();
            GetTableAttribute getTableAttribute = type => null;

            var tryAssociateTypeToTableMock = new Mock<TryAssociateTypeToTable>();
            var assemblyWrapperMock = new Mock<AssemblyWrapper>();
            appDomainWrapperMock.Setup(m => m.Load(assemblyName)).Returns(assemblyWrapperMock.Object);

            assemblyWrapperMock.SetupGet(m => m.DefinedTypes).Throws(new ReflectionTypeLoadException(null, null));

            // Act

            this.service.PopulateAssemblyCache(appDomainWrapperMock.Object, assemblyName, getTableAttribute, "schema",
                tryAssociateTypeToTableMock.Object, this.assemblyLookupContext);

            // Assert

            appDomainWrapperMock.Verify(m => m.Load(assemblyName), Times.Once);
            assemblyWrapperMock.VerifyGet(m => m.DefinedTypes, Times.Once);

            tryAssociateTypeToTableMock.Verify(m => m(It.IsAny<TypeInfoWrapper>(), It.IsAny<AssemblyLookupContext>(),
                It.IsAny<GetTableAttribute>(), It.IsAny<string>()), Times.Never());
        }
    }
}