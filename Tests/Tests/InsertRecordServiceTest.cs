using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.RepositoryOperations.Operations.InsertRecord;
using TestDataFramework.WritePrimitives;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class InsertRecordServiceTest
    {
        private InsertRecordService insertRecordService;
        private RecordReference recordReference;
        private Mock<IWritePrimitives> writerMock;

        private ForeignTable mainTable;

        [TestInitialize]
        public void Initialize()
        {
            this.mainTable = new ForeignTable();

            this.recordReference = new RecordReference<ForeignTable>(this.mainTable);
            this.insertRecordService = new InsertRecordService(this.recordReference);
            this.writerMock = new Mock<IWritePrimitives>();
        }

        [TestMethod]
        public void GetPrimaryKeyOperations_Test()
        {
            // Arrange

            var peerRecordreferences = new RecordReference[]
            {
                new RecordReference<SubjectClass>(new SubjectClass()),
                new RecordReference<PrimaryTable>(new PrimaryTable()),
                new RecordReference<SubjectClass>(new SubjectClass()),
                new RecordReference<PrimaryTable>(new PrimaryTable()),
            };

            InsertRecord[] peerOperations = peerRecordreferences.Select(r => new InsertRecord(null, r, null)).ToArray();

            // Act

            this.recordReference.AddPrimaryRecordReference(peerRecordreferences[1], peerRecordreferences[3]);

            InsertRecord[] primaryKeyOperations = this.insertRecordService.GetPrimaryKeyOperations(peerOperations).ToArray();

            // Assert

            Assert.AreEqual(2, primaryKeyOperations.Length);
            Assert.AreEqual(peerOperations[1], primaryKeyOperations[0]);
            Assert.AreEqual(peerOperations[3], primaryKeyOperations[1]);
        }

        [TestMethod]
        public void WritePrimaryKeyOperations_Test()
        {
            // Arrange

            var primaryKeyOperations = new[]
            {
                new Mock<InsertRecord>(null, null, null),
                new Mock<InsertRecord>(null, null, null),
            };

            // Act

            var breaker = new CircularReferenceBreaker();
            var currentOrder = new CurrentOrder();
            var orderedOperations = new AbstractRepositoryOperation[0];

            this.insertRecordService.WritePrimaryKeyOperations(this.writerMock.Object, primaryKeyOperations.Select(m => m.Object), breaker, currentOrder, orderedOperations);

            // Assert

            primaryKeyOperations.ToList().ForEach(o => o.Verify(m => m.Write(breaker, this.writerMock.Object, currentOrder, orderedOperations), Times.Once));
        }

        [TestMethod]
        public void GetColumnData_Test()
        {
            // Arrange

            var primaryKeyOperations = new[]
            {
                new Mock<InsertRecord>(null, null, null),
                new Mock<InsertRecord>(null, null, null),
            };

            primaryKeyOperations[0].Setup(m => m.GetPrimaryKeySymbols())
                .Returns(new[] {new ColumnSymbol {ColumnName = "Key", TableType = typeof (PrimaryTable), Value = 10}});

            primaryKeyOperations[1].Setup(m => m.GetPrimaryKeySymbols()).Returns(new[] {new ColumnSymbol()});

            this.mainTable.Text = "Value1";
            this.mainTable.Integer = 5;

            // Act

            Columns columns = this.insertRecordService.GetColumnData(primaryKeyOperations.Select(o => o.Object));

            // Assert

            Assert.AreEqual(2, columns.RegularColumns.Count());
            Assert.AreEqual(1, columns.ForeignKeyColumns.Count());

            Assert.AreEqual("Text", columns.RegularColumns.ElementAt(0).Name);
            Assert.AreEqual(this.mainTable.Text, columns.RegularColumns.ElementAt(0).Value);

            Assert.AreEqual("Integer", columns.RegularColumns.ElementAt(1).Name);
            Assert.AreEqual(this.mainTable.Integer, columns.RegularColumns.ElementAt(1).Value);

            Assert.AreEqual("ForeignKey", columns.ForeignKeyColumns.ElementAt(0).Name);
            Assert.AreEqual(10, columns.ForeignKeyColumns.ElementAt(0).Value);
        }

        [TestMethod]
        public void WritePrimitives_Insert_Test()
        {
            var columns = new Column[0];
            var primaryKeyValues = new List<ColumnSymbol>();

            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, columns, primaryKeyValues);

            // Assert

            this.writerMock.Verify(m => m.Insert(columns));
        }

        [TestMethod]
        public void WritePrimitives_AddPrimaryKeyValue_AutoKey_Test()
        {
            var columns = new Column[0];
            var primaryKeyValues = new List<ColumnSymbol>();

            this.writerMock.Setup(m => m.SelectIdentity()).Returns("ABCD");

            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, columns, primaryKeyValues);

            // Assert

            this.writerMock.Verify(m => m.SelectIdentity(), Times.Once);

            Assert.AreEqual(1, primaryKeyValues.Count);

            Assert.AreEqual("Key", primaryKeyValues[0].ColumnName);
            Assert.AreEqual(typeof(ForeignTable), primaryKeyValues[0].TableType);
            Assert.AreEqual("ABCD", primaryKeyValues[0].Value);
        }

        [TestMethod]
        public void WritePrimitives_AddPrimaryKeyValue_ManualKeys_Test()
        {
            // Arrange

            var columns = new Column[0];
            var primaryKeyValues = new List<ColumnSymbol>();

            this.insertRecordService =
                new InsertRecordService(
                    new RecordReference<ManualKeyPrimaryTable>(new ManualKeyPrimaryTable {Key1 = "ABCD", Key2 = 5}));

            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, columns, primaryKeyValues);

            // Assert

            Assert.AreEqual(2, primaryKeyValues.Count);

            Assert.AreEqual(typeof(ManualKeyPrimaryTable), primaryKeyValues[0].TableType);
            Assert.AreEqual("ABCD", primaryKeyValues[0].Value);
            Assert.AreEqual("Key1", primaryKeyValues[0].ColumnName);

            Assert.AreEqual(typeof(ManualKeyPrimaryTable), primaryKeyValues[1].TableType);
            Assert.AreEqual(5, primaryKeyValues[1].Value);
            Assert.AreEqual("Key2", primaryKeyValues[1].ColumnName);
        }

        [TestMethod]
        public void WritePrimitives_AddPrimaryKeyValue_KeysNone_Test()
        {
            // Arrange

            var columns = new Column[0];
            var primaryKeyValues = new List<ColumnSymbol>();

            this.insertRecordService =
                new InsertRecordService(
                    new RecordReference<KeyNoneTable>(new KeyNoneTable()));

            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, columns, primaryKeyValues);

            // Assert

            Assert.IsFalse(primaryKeyValues.Any());
        }
    }
}
