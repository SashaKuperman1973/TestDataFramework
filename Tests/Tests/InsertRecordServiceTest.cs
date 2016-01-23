using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using log4net.Config;
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
            XmlConfigurator.Configure();

            this.mainTable = new ForeignTable();

            this.recordReference = new RecordReference<ForeignTable>(this.mainTable, null);
            this.insertRecordService = new InsertRecordService(this.recordReference);
            this.writerMock = new Mock<IWritePrimitives>();
        }

        [TestMethod]
        public void GetPrimaryKeyOperations_Test()
        {
            // Arrange

            var peerRecordreferences = new RecordReference[]
            {
                new RecordReference<SubjectClass>(new SubjectClass(), null),
                new RecordReference<PrimaryTable>(new PrimaryTable(), null),
                new RecordReference<SubjectClass>(new SubjectClass(), null),
                new RecordReference<PrimaryTable>(new PrimaryTable(), null),
            };

            InsertRecord[] peerOperations = peerRecordreferences.Select(r => new InsertRecord(null, r, null)).ToArray();

            // Act

            this.recordReference.AddPrimaryRecordReference(peerRecordreferences[1], peerRecordreferences[3]);

            InsertRecord[] primaryKeyOperations =
                this.insertRecordService.GetPrimaryKeyOperations(peerOperations).ToArray();

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
            var currentOrder = new Counter();
            var orderedOperations = new AbstractRepositoryOperation[0];

            this.insertRecordService.WritePrimaryKeyOperations(this.writerMock.Object,
                primaryKeyOperations.Select(m => m.Object), breaker, currentOrder, orderedOperations);

            // Assert

            primaryKeyOperations.ToList()
                .ForEach(
                    o =>
                        o.Verify(m => m.Write(breaker, this.writerMock.Object, currentOrder, orderedOperations),
                            Times.Once));
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

            const int primaryKeyValue = 10;

            primaryKeyOperations[0].Setup(m => m.GetPrimaryKeySymbols())
                .Returns(new[]
                {new ColumnSymbol {ColumnName = "Key", TableType = typeof (PrimaryTable), Value = primaryKeyValue}});

            primaryKeyOperations[1].Setup(m => m.GetPrimaryKeySymbols()).Returns(new[] {new ColumnSymbol()});

            this.mainTable.Text = "Value1";
            this.mainTable.Integer = 5;

            // Act

            Columns columns = this.insertRecordService.GetColumnData(primaryKeyOperations.Select(o => o.Object),
                this.writerMock.Object);

            // Assert

            Assert.AreEqual(2, columns.RegularColumns.Count());
            Assert.AreEqual(1, columns.ForeignKeyColumns.Count());

            Assert.AreEqual("Text", columns.RegularColumns.ElementAt(0).Name);
            Assert.AreEqual(this.mainTable.Text, columns.RegularColumns.ElementAt(0).Value);

            Assert.AreEqual("Integer", columns.RegularColumns.ElementAt(1).Name);
            Assert.AreEqual(this.mainTable.Integer, columns.RegularColumns.ElementAt(1).Value);

            Assert.AreEqual("ForeignKey", columns.ForeignKeyColumns.ElementAt(0).Name);
            Assert.AreEqual(primaryKeyValue, columns.ForeignKeyColumns.ElementAt(0).Value);
        }

        [TestMethod]
        public void WritePrimitives_Insert_Test()
        {
            var columns = new Column[0];
            var primaryKeyValues = new List<ColumnSymbol>();
            const string tableName = "ABCD";

            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, tableName, columns, primaryKeyValues);

            // Assert

            this.writerMock.Verify(m => m.Insert(tableName, columns));
        }

        [TestMethod]
        public void WritePrimitives_AddPrimaryKeyValue_AutoKey_Test()
        {
            var columns = new Column[0];
            var primaryKeyValues = new List<ColumnSymbol>();

            const string identityVariableSymbol = "ABCD";
            string tableName = this.mainTable.GetType().Name;

            this.writerMock.Setup(m => m.SelectIdentity(It.IsAny<string>())).Returns(identityVariableSymbol);

            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, tableName, columns, primaryKeyValues);

            // Assert

            this.writerMock.Verify(m => m.SelectIdentity(It.IsAny<string>()), Times.Once);

            Assert.AreEqual(1, primaryKeyValues.Count);

            Assert.AreEqual("Key", primaryKeyValues[0].ColumnName);
            Assert.AreEqual(typeof (ForeignTable), primaryKeyValues[0].TableType);
            Assert.AreEqual(identityVariableSymbol, primaryKeyValues[0].Value);
        }

        [TestMethod]
        public void WritePrimitives_AddPrimaryKeyValue_ManualKeys_Test()
        {
            // Arrange

            var columns = new Column[0];
            var primaryKeyValues = new List<ColumnSymbol>();

            const string keyValue1 = "ABCD";
            const int keyValue2 = 5;
            const string tableName = "XYZ";

            this.insertRecordService =
                new InsertRecordService(
                    new RecordReference<ManualKeyPrimaryTable>(new ManualKeyPrimaryTable
                    {
                        Key1 = keyValue1,
                        Key2 = keyValue2
                    }, null));

            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, tableName, columns, primaryKeyValues);

            // Assert

            Assert.AreEqual(2, primaryKeyValues.Count);

            Assert.AreEqual(typeof (ManualKeyPrimaryTable), primaryKeyValues[0].TableType);
            Assert.AreEqual(keyValue1, primaryKeyValues[0].Value);
            Assert.AreEqual("Key1", primaryKeyValues[0].ColumnName);

            Assert.AreEqual(typeof (ManualKeyPrimaryTable), primaryKeyValues[1].TableType);
            Assert.AreEqual(keyValue2, primaryKeyValues[1].Value);
            Assert.AreEqual("Key2", primaryKeyValues[1].ColumnName);
        }

        [TestMethod]
        public void WritePrimitives_AddPrimaryKeyValue_ManualGuids_Test()
        {
            // Arrange

            var columns = new Column[0];
            var primaryKeyValues = new List<ColumnSymbol>();

            const string tableName = "XYZ";
            const string symbol1 = "Symbol1";
            const string symbol2 = "Symbol2";

            this.insertRecordService =
                new InsertRecordService(
                    new RecordReference<ClassWithGuidKeys>(new ClassWithGuidKeys(), null));

            this.writerMock.Setup(m => m.WriteGuid("Key1")).Returns(symbol1);
            this.writerMock.Setup(m => m.WriteGuid("Key3")).Returns(symbol2);

            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, tableName, columns, primaryKeyValues);

            // Assert

            Assert.AreEqual(3, primaryKeyValues.Count);

            Assert.AreEqual(typeof (ClassWithGuidKeys), primaryKeyValues[0].TableType);
            Assert.AreEqual(symbol1, primaryKeyValues[0].Value);
            Assert.AreEqual("Key1", primaryKeyValues[0].ColumnName);

            Assert.AreEqual(typeof (ClassWithGuidKeys), primaryKeyValues[1].TableType);
            Assert.AreEqual(symbol2, primaryKeyValues[2].Value);
            Assert.AreEqual("Key3", primaryKeyValues[2].ColumnName);
        }

        [TestMethod]
        public void WritePrimitives_NonKeyGuids_Test()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void WritePrimitives_AddPrimaryKeyValue_KeysNone_Test()
        {
            // Arrange

            var columns = new Column[0];
            var primaryKeyValues = new List<ColumnSymbol>();

            this.insertRecordService =
                new InsertRecordService(
                    new RecordReference<KeyNoneTable>(new KeyNoneTable(), null));

            const string tableName = "ABCD";

            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, tableName, columns, primaryKeyValues);

            // Assert

            Assert.IsFalse(primaryKeyValues.Any());
        }

        [TestMethod]
        public void CopyForeignKeyColumns_Test()
        {
            // Arrange

            var target = new ManualKeyForeignTable();

            this.insertRecordService = new InsertRecordService(new RecordReference<ManualKeyForeignTable>(target, null));

            var columns = new[]
            {
                new Column { Name = "ForeignKey1", Value = "ABCD" },
                new Column { Name = "Two", Value = new Variable(null) },
                new Column { Name = "ForeignKey2", Value = 3 },
            };

            // Act

            this.insertRecordService.CopyForeignKeyColumns(columns);

            // Assert

            Assert.AreEqual(columns[0].Value, target.ForeignKey1);
            Assert.AreEqual(columns[2].Value, target.ForeignKey2);
        }

        [TestMethod]
        public void CopyForeignKeyColumns_UnknownColumnThrows_Test()
        {
            // Arrange

            this.insertRecordService = new InsertRecordService(new RecordReference<ManualKeyForeignTable>(new ManualKeyForeignTable(), null));

            var columns = new[]
            {
                new Column { Name = "ForeignKey1", Value = "ABCD" },
                new Column { Name = "Two", Value = new object() },
            };

            // Act
            // Assert

            Helpers.ExceptionTest(
                () => this.insertRecordService.CopyForeignKeyColumns(columns),
                typeof(InvalidOperationException),
                "Sequence contains no matching element");            
        }
    }
}
