using System;
using System.Collections.Generic;
using System.Security.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests.TableTypeCacheServiceTests
{
    [TestClass]
    public class TableTypeLookupTests
    {
        private TableTypeLookup tableTypeLookup;

        private Mock<IAttributeDecoratorBase> attributeDecoratorBaseMock;

        [TestInitialize]
        public void Initialize()
        {
            this.attributeDecoratorBaseMock = new Mock<IAttributeDecoratorBase>();
            this.tableTypeLookup = new TableTypeLookup(this.attributeDecoratorBaseMock.Object);
        }

        [TestMethod]
        public void GetTableTypeByCriteria_Criteria_And_BasicTableFieldsCheck_ShouldPass_Test()
        {
            // Arrange

            var tableOfSet = new Table(new TypeInfoWrapper(typeof(SubjectClass)), null);
            var inputTable = new Table(new TypeInfoWrapper(typeof(SubjectClass)), null);
            var typeInfoWrapper = new TypeInfoWrapper();

            var assemblyLookupContext = new AssemblyLookupContext();
            assemblyLookupContext.TypeDictionary.TryAdd(tableOfSet, typeInfoWrapper);

            bool EqualsCriteria(Table fromSet, Table input) => true;

            // Act

            TypeInfoWrapper result =
                this.tableTypeLookup.GetTableTypeByCriteria(inputTable, EqualsCriteria, assemblyLookupContext);

            // Assert

            Assert.AreEqual(typeInfoWrapper, result);
        }

        [TestMethod]
        public void GetTableTypeByCriteria_Criteria_And_BasicTableFieldsCheck_ShouldFail_Test()
        {
            Type matchesSet = typeof(SubjectClass);
            Type doesNotMatchSet = typeof(SecondClass);

            this.Criteria_And_BasicTableFieldsCheck_ShouldFail(inputType: matchesSet, criteriaCheckResult: false);
            this.Criteria_And_BasicTableFieldsCheck_ShouldFail(inputType: doesNotMatchSet, criteriaCheckResult: true);
            this.Criteria_And_BasicTableFieldsCheck_ShouldFail(inputType: doesNotMatchSet, criteriaCheckResult: false);
        }

        private void Criteria_And_BasicTableFieldsCheck_ShouldFail(Type inputType, bool criteriaCheckResult)
        {
            // Arrange

            var tableOfSet = new Table(new TypeInfoWrapper(typeof(SubjectClass)), null);
            var inputTable = new Table(new TypeInfoWrapper(inputType), null);
            var typeInfoWrapper = new TypeInfoWrapper();

            var assemblyLookupContext = new AssemblyLookupContext();
            assemblyLookupContext.TypeDictionary.TryAdd(tableOfSet, typeInfoWrapper);

            bool EqualsCriteria(Table fromSet, Table input) => criteriaCheckResult;

            // Act

            TypeInfoWrapper result =
                this.tableTypeLookup.GetTableTypeByCriteria(inputTable, EqualsCriteria, assemblyLookupContext);

            // Assert

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetTableTypeByCriteria_Criteria_And_BasicTableFieldsCheck_ShouldCauseCollision_Test()
        {
            // Arrange

            var tableOfSet = new Table(new TypeInfoWrapper(typeof(SubjectClass)), null);
            var inputTable = new Table(new TypeInfoWrapper(typeof(SubjectClass)), null);
            var typeInfoWrapper = new TypeInfoWrapper();

            var assemblyLookupContext = new AssemblyLookupContext();
            assemblyLookupContext.CollisionDictionary.TryAdd(tableOfSet, new[] {typeInfoWrapper});

            bool EqualsCriteria(Table fromSet, Table input) => true;

            // Act
            // Assert

            Helpers.ExceptionTest(() =>
                    this.tableTypeLookup.GetTableTypeByCriteria(inputTable, EqualsCriteria, assemblyLookupContext),
                typeof(TableTypeCacheException)
            );
        }

        [TestMethod]
        public void GetTableTypeByCriteria_Criteria_And_BasicTableFieldsCheck_ShouldNotCauseCollision_Test()
        {
            Type matchesSet = typeof(SubjectClass);
            Type doesNotMatchSet = typeof(SecondClass);

            this.GetTableTypeByCriteria_Criteria_And_BasicTableFieldsCheck_ShouldNotCauseCollision(
                inputType: matchesSet, criteriaCheckResult: false);
            this.GetTableTypeByCriteria_Criteria_And_BasicTableFieldsCheck_ShouldNotCauseCollision(
                inputType: doesNotMatchSet, criteriaCheckResult: true);
            this.GetTableTypeByCriteria_Criteria_And_BasicTableFieldsCheck_ShouldNotCauseCollision(
                inputType: doesNotMatchSet, criteriaCheckResult: false);
        }

        private void GetTableTypeByCriteria_Criteria_And_BasicTableFieldsCheck_ShouldNotCauseCollision(Type inputType,
            bool criteriaCheckResult)
        {
            // Arrange

            var tableOfSet = new Table(new TypeInfoWrapper(typeof(SubjectClass)), null);
            var inputTable = new Table(new TypeInfoWrapper(inputType), null);
            var typeInfoWrapper = new TypeInfoWrapper();

            var assemblyLookupContext = new AssemblyLookupContext();
            assemblyLookupContext.CollisionDictionary.TryAdd(tableOfSet, new[] {typeInfoWrapper});

            bool EqualsCriteria(Table fromSet, Table input) => criteriaCheckResult;

            // Act

            TypeInfoWrapper result =
                this.tableTypeLookup.GetTableTypeByCriteria(inputTable, EqualsCriteria, assemblyLookupContext);

            // Assert

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetTableTypeWithCatalogue_Test()
        {
            // Arrange

            var inputTable = new Table(new TableAttribute(null, "schema", "tableName"));
            var matchingTableFromSet = new Table(new TableAttribute("catalogueName", "schema", "tableName"));
            var matchingType = new TypeInfoWrapper();

            var assemblyLookupContext = new AssemblyLookupContext();
            assemblyLookupContext.TypeDictionary.TryAdd(matchingTableFromSet, matchingType);

            var matchingTableAtribute = new TableAttribute("catalogueName", "schema", "tableName");

            this.attributeDecoratorBaseMock.Setup(m => m.GetSingleAttribute<TableAttribute>(matchingType))
                .Returns(matchingTableAtribute);

            // Act

            TypeInfoWrapper result = this.tableTypeLookup.GetTableTypeWithCatalogue(inputTable, assemblyLookupContext);

            // Assert

            Assert.AreEqual(matchingType, result);
        }

        [TestMethod]
        public void GetTableTypeWithCatalogue_AmbigousTableObject_CausesCollision_Test()
        {
            // Arrange

            var inputTable = new Table(new TableAttribute(null, "schema", "tableName"));

            var matchingType1 = new TypeInfoWrapper();
            var matchingType2 = new TypeInfoWrapper();

            var matchingTableAtribute1 = new TableAttribute("catalogueName1", "schema", "tableName");
            var matchingTableAtribute2 = new TableAttribute("catalogueName2", "schema", "tableName");

            var matchingTableFromSet1 = new Table(matchingTableAtribute1);
            var matchingTableFromSet2 = new Table(matchingTableAtribute2);
            var assemblyLookupContext = new AssemblyLookupContext();

            // To add multiple keys a key should not be found during TryAdd check for existing key.
            assemblyLookupContext.TypeDictionaryEqualityComparer.SetEqualsCriteria((fromSet, input) => false);
            assemblyLookupContext.TypeDictionary.TryAdd(matchingTableFromSet1, matchingType1);
            assemblyLookupContext.TypeDictionary.TryAdd(matchingTableFromSet2, matchingType2);

            this.attributeDecoratorBaseMock.Setup(m => m.GetSingleAttribute<TableAttribute>(matchingType1))
                .Returns(matchingTableAtribute1);

            this.attributeDecoratorBaseMock.Setup(m => m.GetSingleAttribute<TableAttribute>(matchingType2))
                .Returns(matchingTableAtribute2);

            // Act

            Helpers.ExceptionTest(() =>
                    this.tableTypeLookup.GetTableTypeWithCatalogue(inputTable, assemblyLookupContext),
                typeof(TableTypeCacheException)
            );
        }

        [TestMethod]
        public void GetTableTypeWithCatalogue_RequestedTableNotFound_ReturnsNull_Test()
        {
            // Arrange

            var inputTable = new Table(new TableAttribute(null, "schema", "tableNameA"));

            var matchingType = new TypeInfoWrapper();
            var matchingTableFromSet = new Table(new TableAttribute("catalogueName", "schema", "tableNameB"));

            var assemblyLookupContext = new AssemblyLookupContext();
            assemblyLookupContext.TypeDictionary.TryAdd(matchingTableFromSet, matchingType);

            // Act

            TypeInfoWrapper result = this.tableTypeLookup.GetTableTypeWithCatalogue(inputTable, assemblyLookupContext);

            // Assert

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetTableTypeWithCatalogue_RequestedTable_HasCatalogue_ReturnsNull_Test()
        {
            // Arrange
            //
            var inputTable = new Table(new TableAttribute("catalogueName", "schema", "tableName"));

            // Act

            TypeInfoWrapper result = this.tableTypeLookup.GetTableTypeWithCatalogue(inputTable, null);

            // Assert

            Assert.IsNull(result);
        }
    }
}