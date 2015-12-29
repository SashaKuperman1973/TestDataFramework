using System;
using System.Collections.Generic;
using System.Configuration;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Persistence;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;
using Tests.TestModels;

namespace Tests
{
    [TestClass]
    public class StandardPersistenceTests
    {
        private StandardPersistence persistence;
        private Mock<IWritePrimitives> writePrimitivesMock;

        [TestInitialize]
        public void Initialize()
        {
            this.writePrimitivesMock = new Mock<IWritePrimitives>();
            this.persistence = new StandardPersistence(this.writePrimitivesMock.Object);

            XmlConfigurator.Configure();
        }

        [TestMethod]
        public void Insert_Test()
        {
            // Arrange

            var primaryTable = new PrimaryTable { Integer = 5, Text = "Text"};
            var primaryRecordReference = new RecordReference<PrimaryTable>(primaryTable);

            List<Column> primaryTableColumns = null;

            this.writePrimitivesMock.Setup(m => m.Insert(It.IsAny<List<Column>>()))
                .Callback<List<Column>>(l => primaryTableColumns = l);

            // Act

            this.persistence.Persist(new RecordReference[] { primaryRecordReference });

            // Assert

            this.writePrimitivesMock.Verify(m => m.Insert(It.IsAny<List<Column>>()), Times.Once());
            Assert.IsNotNull(primaryTableColumns);
            Assert.AreEqual(2, primaryTableColumns.Count);

            Assert.AreEqual(primaryTable.Key, 0);

            Assert.AreEqual("Integer", primaryTableColumns[0].Name);
            Assert.AreEqual(primaryTable.Integer, primaryTableColumns[0].Value);

            Assert.AreEqual("Text", primaryTableColumns[1].Name);
            Assert.AreEqual(primaryTable.Text, primaryTableColumns[1].Value);
        }

        [TestMethod]
        public void ForeignKeyBinding_Test()
        {
            throw new NotImplementedException();

            var primaryTable = new PrimaryTable();
            var primaryRecordReference = new RecordReference<PrimaryTable>(primaryTable);

            var foreignTable = new ForeignTable();
            var foreignRecordReference = new RecordReference<ForeignTable>(foreignTable);

            primaryRecordReference.AddPrimaryRecordReference(foreignRecordReference);

            // Act

            this.persistence.Persist(new RecordReference[] { primaryRecordReference, foreignRecordReference });

            // Assert
        }

        [TestMethod]
        public void AutoPrimaryKeyGeneration_Test()
        {
            throw new NotImplementedException();

            RecordReference[] recordReferenceList =
            {
                new RecordReference<PrimaryTable>(new PrimaryTable()),

                new RecordReference<ForeignTable>(new ForeignTable()),

                new RecordReference<PrimaryTable>(new PrimaryTable()),

                new RecordReference<ForeignTable>(new ForeignTable()),
            };


            // Act

            this.persistence.Persist(recordReferenceList);

            // Assert

            int initialPrimaryTableKey = ((PrimaryTable) recordReferenceList[0].RecordObject).Key;
            int subsequentPrimaryTableKey = ((PrimaryTable)recordReferenceList[2].RecordObject).Key;

            Assert.IsTrue(subsequentPrimaryTableKey == initialPrimaryTableKey + 1);

            int initialForeignTableKey = ((ForeignTable)recordReferenceList[1].RecordObject).Key;
            int subsequentForeignTableKey = ((ForeignTable)recordReferenceList[3].RecordObject).Key;

            Assert.IsTrue(subsequentForeignTableKey == initialForeignTableKey + 1);
        }
    }
}
