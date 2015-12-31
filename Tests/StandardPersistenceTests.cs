using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
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

            this.writePrimitivesMock.Setup(m => m.Insert(It.IsAny<IEnumerable<Column>>()))
                .Callback<IEnumerable<Column>>(c => primaryTableColumns = c.ToList());

            // Act

            this.persistence.Persist(new RecordReference[] { primaryRecordReference });

            // Assert

            this.writePrimitivesMock.Verify(m => m.Insert(It.IsAny<IEnumerable<Column>>()), Times.Once());
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

            this.writePrimitivesMock.Setup(m => m.Insert(It.IsAny<IEnumerable<Column>>()))
                .Callback<IEnumerable<Column>>(c => columns.Add(c.ToList()));

            this.writePrimitivesMock.Setup(m => m.SelectIdentity()).Returns(new Variable(null));

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

            var columns = new List<List<Column>>();

            this.writePrimitivesMock.Setup(m => m.Insert(It.IsAny<IEnumerable<Column>>()))
                .Callback<IEnumerable<Column>>(c => columns.Add(c.ToList()));

            // Act

            this.persistence.Persist(new RecordReference[] { foreignRecordReference, primaryRecordReference });

            // Assert

            Assert.AreEqual(primaryTable.Key1, foreignTable.ForeignKey1);
            Assert.AreEqual(primaryTable.Key2, foreignTable.ForeignKey2);
        }

        [TestMethod]
        public void ForeignKeyCopiedFromAutoPrimaryKey_Test()
        {
            throw new NotImplementedException();
        }
    }
}
