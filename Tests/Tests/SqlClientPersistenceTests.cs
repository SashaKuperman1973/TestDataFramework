using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Persistence;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class SqlClientPersistenceTests
    {
        private SqlClientPersistence persistence;
        private Mock<IWritePrimitives> writePrimitivesMock;
        private Mock<IDeferredValueGenerator<ulong>> deferredValueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.writePrimitivesMock = new Mock<IWritePrimitives>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<ulong>>();

            this.persistence = new SqlClientPersistence(this.writePrimitivesMock.Object, this.deferredValueGeneratorMock.Object);

            XmlConfigurator.Configure();
        }

        [TestMethod]
        public void Insert_Test()
        {
            // Arrange

            var primaryTable = new PrimaryTable { Integer = 5, Text = "Text"};
            var primaryRecordReference = new RecordReference<PrimaryTable>(primaryTable);
            string tableName = typeof(PrimaryTable).Name;

            List<Column> primaryTableColumns = null;

            this.writePrimitivesMock.Setup(m => m.Insert(tableName, It.IsAny<IEnumerable<Column>>()))
                .Callback<string, IEnumerable<Column>>((s, c) => primaryTableColumns = c.ToList());

            this.writePrimitivesMock.Setup(m => m.Execute()).Returns(new object[] {0});

            // Act

            var recordReferenceArray = new RecordReference[] {primaryRecordReference};

            this.persistence.Persist(recordReferenceArray);

            // Assert

            this.writePrimitivesMock.Verify(m => m.Insert(tableName, It.IsAny<IEnumerable<Column>>()), Times.Once());

            this.deferredValueGeneratorMock.Verify(
                m => m.Execute(It.Is<IEnumerable<object>>(e => e.First() == recordReferenceArray[0].RecordObject)),
                Times.Once);

            Assert.IsNotNull(primaryTableColumns);
            Assert.AreEqual(2, primaryTableColumns.Count);

            Assert.AreEqual("Text", primaryTableColumns[0].Name);
            Assert.AreEqual(primaryTable.Text, primaryTableColumns[0].Value);

            Assert.AreEqual("Integer", primaryTableColumns[1].Name);
            Assert.AreEqual(primaryTable.Integer, primaryTableColumns[1].Value);
        }

        [TestMethod]
        public void InsertsInProperOrder_Test()
        {
            // Arrange

            var primaryTable = new PrimaryTable { Integer = 1};
            var primaryRecordReference = new RecordReference<PrimaryTable>(primaryTable);

            var foreignTable = new ForeignTable {Integer = 1};
            var foreignRecordReference = new RecordReference<ForeignTable>(foreignTable);

            foreignRecordReference.AddPrimaryRecordReference(primaryRecordReference);

            var columns = new List<List<Column>>();

            this.writePrimitivesMock.Setup(m => m.Insert(It.IsAny<string>(), It.IsAny<IEnumerable<Column>>()))
                .Callback<string, IEnumerable<Column>>((s, c) => columns.Add(c.ToList()));

            this.writePrimitivesMock.Setup(m => m.SelectIdentity()).Returns(new Variable(null));

            this.writePrimitivesMock.Setup(m => m.Execute()).Returns(new object[] {0, 0});

            // Act

            // Note the foreign key record is being passed in before the primary key record 
            // to test that the primary key record writes first regardless which insert operation's
            // Write method is called.

            this.persistence.Persist(new RecordReference[] { foreignRecordReference, primaryRecordReference});

            // Assert

            Assert.AreEqual(primaryTable.Integer, columns[0].First(c => c.Name == "Integer").Value);
            Assert.AreEqual(foreignTable.Integer, columns[1].First(c => c.Name == "Integer").Value);
        }

        [TestMethod]
        public void ForeignKeysCopiedFromManualPrimaryKeys_Test()
        {
            // Arrange

            var primaryTable = new ManualKeyPrimaryTable {Key1 = "A", Key2 = 7};
            var foreignTable = new ManualKeyForeignTable();

            var primaryRecordReference = new RecordReference<ManualKeyPrimaryTable>(primaryTable);
            var foreignRecordReference = new RecordReference<ManualKeyForeignTable>(foreignTable);

            foreignRecordReference.AddPrimaryRecordReference(primaryRecordReference);
            const string tableName = "ABCD";

            var columns = new List<List<Column>>();

            this.writePrimitivesMock.Setup(m => m.Insert(tableName, It.IsAny<IEnumerable<Column>>()))
                .Callback<string, IEnumerable<Column>>((s,c) => columns.Add(c.ToList()));

            // Act

            this.persistence.Persist(new RecordReference[] { foreignRecordReference, primaryRecordReference });

            // Assert

            Assert.AreEqual(primaryTable.Key1, foreignTable.ForeignKey1);
            Assert.AreEqual(primaryTable.Key2, foreignTable.ForeignKey2);
        }

        [TestMethod]
        public void ForeignKeyCopiedFromAutoPrimaryKey_InCorrectOrder_Test()
        {
            // Arrange

            var primaryTable = new PrimaryTable();
            var primaryRecordReference = new RecordReference<PrimaryTable>(primaryTable);

            var foreignTable = new ForeignTable();
            var foreignRecordReference = new RecordReference<ForeignTable>(foreignTable);

            foreignRecordReference.AddPrimaryRecordReference(primaryRecordReference);

            var expected = new object[] {1, 2};

            this.writePrimitivesMock.Setup(m => m.Execute()).Returns(expected);
            
            // Act

            // Note the foreign key record is being passed in before the primary key record. 
            // This is to test that the primary key record that wrote first gets the first return
            // data element and the foreign key record gets the subsequent one.

            this.persistence.Persist(new RecordReference[] { foreignRecordReference, primaryRecordReference });

            // Assert

            Assert.AreEqual(expected[0], primaryTable.Key);
            Assert.AreEqual(expected[1], foreignTable.Key);
        }
    }
}
