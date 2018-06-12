using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests.TableTypeCacheServiceTests
{
    [TestClass]
    public class StandardTableTypeCacheServiceTests
    {
        private readonly AssemblyLookupContext assemblyLookupContext = new AssemblyLookupContext();

        private ForeignKeyAttribute foreignKeyAttribute;
        private StandardTableTypeCacheService service;

        private Table table;
        private TableAttribute tableAttribute;
        private Mock<TableTypeLookup> tableTypeLookupMock;

        [TestInitialize]
        public void Initialize()
        {
            this.tableTypeLookupMock = new Mock<TableTypeLookup>(null);
            this.service = new StandardTableTypeCacheService(this.tableTypeLookupMock.Object);

            this.foreignKeyAttribute = new ForeignKeyAttribute("schema", "primaryTableName", "primaryKeyName");
            this.tableAttribute = new TableAttribute("catalogueName", "schema", "tableName");
            this.table = new Table(this.foreignKeyAttribute, this.tableAttribute);
        }

        private void VerifyGetTableTypeByCriteria(TypeDictionaryEqualityComparer.EqualsCriteriaDelegate matchCriteria)
        {
            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeByCriteria(
                    It.Is<Table>(table => table.CatalogueName == this.table.CatalogueName &&
                                          table.TableName == this.table.TableName && table.Schema == this.table.Schema),
                    matchCriteria, this.assemblyLookupContext), Times.Once);
        }

        [TestMethod]
        public void CompleteMatch_IsFirst_Test()
        {
            // Arrange

            this.tableTypeLookupMock.Setup(m => m.GetTableTypeByCriteria(It.IsAny<Table>(),
                    It.Is<TypeDictionaryEqualityComparer.EqualsCriteriaDelegate>(
                        func => func == TableTypeCriteria.CompleteMatchCriteria), this.assemblyLookupContext))
                .Returns(new TypeInfoWrapper(typeof(SubjectClass)));

            // Act

            TypeInfoWrapper result = this.service.GetCachedTableType(this.foreignKeyAttribute, this.tableAttribute,
                this.assemblyLookupContext);

            // Assert

            Assert.AreEqual(new TypeInfoWrapper(typeof(SubjectClass)), result);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.CompleteMatchCriteria);

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
                        func => func == TableTypeCriteria.CompleteMatchCriteria), this.assemblyLookupContext))
                .Returns((TypeInfoWrapper) null);

            this.tableTypeLookupMock
                .Setup(m => m.GetTableTypeWithCatalogue(It.IsAny<Table>(), this.assemblyLookupContext))
                .Returns(new TypeInfoWrapper(typeof(SubjectClass)));

            // Act

            TypeInfoWrapper result = this.service.GetCachedTableType(this.foreignKeyAttribute, this.tableAttribute,
                this.assemblyLookupContext);


            // Assert

            Assert.AreEqual(new TypeInfoWrapper(typeof(SubjectClass)), result);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.CompleteMatchCriteria);

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
                        func => func == TableTypeCriteria.CompleteMatchCriteria), this.assemblyLookupContext))
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

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.CompleteMatchCriteria);

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
                        func => func == TableTypeCriteria.CompleteMatchCriteria), this.assemblyLookupContext))
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

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.CompleteMatchCriteria);

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
                        func => func == TableTypeCriteria.CompleteMatchCriteria), this.assemblyLookupContext))
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

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.CompleteMatchCriteria);

            this.tableTypeLookupMock.Verify(
                m => m.GetTableTypeWithCatalogue(It.IsAny<Table>(), It.IsAny<AssemblyLookupContext>()), Times.Once);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.MatchOnWhatIsDecorated);

            this.VerifyGetTableTypeByCriteria(TableTypeCriteria.MatchOnEverythingNotAlreadyTried);
        }
    }
}