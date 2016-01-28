using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.RepositoryOperations.Operations.InsertRecord;
using TestDataFramework.TypeGenerator;
using TestDataFramework.WritePrimitives;
using Tests.TestModels;
using TestDataFramework.Helpers;

namespace Tests.Tests
{
    [TestClass]
    public class InsertRecordServiceTest
    {
        private InsertRecordService insertRecordService;
        private RecordReference<ForeignTable> recordReference;
        private Mock<IWritePrimitives> writerMock;
        private Mock<ITypeGenerator> typeGeneratorMock;

        private ForeignTable foreignKeyTable;
        private List<Column> mainTableColumns;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.foreignKeyTable = new ForeignTable();

            this.typeGeneratorMock = Helpers.GetTypeGeneratorMock(this.foreignKeyTable);
            this.recordReference = new RecordReference<ForeignTable>(this.typeGeneratorMock.Object);
            this.recordReference.Populate();
            this.insertRecordService = new InsertRecordService(this.recordReference);
            this.writerMock = new Mock<IWritePrimitives>();

            this.mainTableColumns = Helpers.GetColumns(this.foreignKeyTable);

        }

        [TestMethod]
        public void GetPrimaryKeyOperations_Test()
        {
            // Arrange

            Mock<ITypeGenerator> subjectTypeGeneratorMock = Helpers.GetTypeGeneratorMock(new SubjectClass());
            Mock<ITypeGenerator> primaryTableTypeGeneratorMock = Helpers.GetTypeGeneratorMock(new PrimaryTable());

            var peerRecordreferences = new RecordReference[]
            {
                new RecordReference<SubjectClass>(subjectTypeGeneratorMock.Object),
                new RecordReference<PrimaryTable>(primaryTableTypeGeneratorMock.Object),
                new RecordReference<SubjectClass>(subjectTypeGeneratorMock.Object),
                new RecordReference<PrimaryTable>(primaryTableTypeGeneratorMock.Object),
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
        public void WritePrimitives_Insert_Test()
        {
            var primaryKeyValues = new List<ColumnSymbol>();
            const string tableName = "ABCD";

            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, tableName, this.mainTableColumns, primaryKeyValues);

            // Assert

            this.writerMock.Verify(m => m.Insert(tableName, this.mainTableColumns));
        }

        [TestMethod]
        public void WritePrimitives_AddPrimaryKeyValue_AutoKey_Test()
        {
            var primaryKeyValues = new List<ColumnSymbol>();

            const string identityVariableSymbol = "ABCD";
            string tableName = this.foreignKeyTable.GetType().Name;

            this.writerMock.Setup(m => m.SelectIdentity(It.IsAny<string>())).Returns(identityVariableSymbol);

            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, tableName, this.mainTableColumns, primaryKeyValues);

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

            var primaryKeyValues = new List<ColumnSymbol>();

            const string keyValue1 = "ABCD";
            const int keyValue2 = 5;
            const string tableName = "XYZ";

            ManualKeyPrimaryTable table;

            this.insertRecordService =
                new InsertRecordService(
                    new RecordReference<ManualKeyPrimaryTable>(
                        Helpers.GetTypeGeneratorMock(
                            table = new ManualKeyPrimaryTable
                            {
                                Key1 = keyValue1,
                                Key2 = keyValue2
                            }).Object));
            // Act

            this.insertRecordService.WritePrimitives(this.writerMock.Object, tableName, Helpers.GetColumns(table), primaryKeyValues);

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
        public void WritePrimitives_AddPrimaryKeyValue_KeysNone_Test()
        {
            // Arrange

            var columns = new Column[0];
            var primaryKeyValues = new List<ColumnSymbol>();

            this.insertRecordService =
                new InsertRecordService(
                    new RecordReference<KeyNoneTable>(
                        Helpers.GetTypeGeneratorMock(
                            new KeyNoneTable()).Object));

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

            var recordReference = new RecordReference<ManualKeyForeignTable>(Helpers.GetTypeGeneratorMock(target).Object);
            recordReference.Populate();

            this.insertRecordService = new InsertRecordService(recordReference);

            var columns = new[]
            {
                new Column { Name = "ForeignKey1", Value = "ABCD" },
                new Column { Name = "Two", Value = new Variable(null) },
                new Column { Name = "ForeignKey2", Value = 3 },
            };

            // Act

            this.insertRecordService.CopyPrimaryToForeignKeyColumns(columns);

            // Assert

            Assert.AreEqual(columns[0].Value, target.ForeignKey1);
            Assert.AreEqual(columns[2].Value, target.ForeignKey2);
        }

        [TestMethod]
        public void CopyForeignKeyColumns_UnknownColumnThrows_Test()
        {
            // Arrange

            var target = new ManualKeyForeignTable();

            var recordReference = new RecordReference<ManualKeyForeignTable>(Helpers.GetTypeGeneratorMock(target).Object);
            recordReference.Populate();

            this.insertRecordService = new InsertRecordService(recordReference);

            var columns = new[]
            {
                new Column { Name = "ForeignKey1", Value = "ABCD" },
                new Column { Name = "Two", Value = new object() },
            };

            // Act
            // Assert

            Helpers.ExceptionTest(
                () => this.insertRecordService.CopyPrimaryToForeignKeyColumns(columns),
                typeof(InvalidOperationException),
                "Sequence contains no matching element");            
        }

        [TestMethod]
        public void GetRegularColumns_NotAutoPrimaryNotForeign_Test()
        {
            // Arrange

            this.foreignKeyTable.Text = "ABCD";
            this.foreignKeyTable.Integer = 7;

            this.recordReference.Populate();

            // Act

            List<Column> regularColumns = this.insertRecordService.GetRegularColumns(null).ToList();

            // Assert

            Assert.AreEqual(2, regularColumns.Count);

            Column textColumn = regularColumns.First(c => c.Name == "Text");
            Column integerColumn = regularColumns.First(c => c.Name == "Integer");

            Assert.AreEqual(this.foreignKeyTable.Text, textColumn.Value);
            Assert.AreEqual(this.foreignKeyTable.Integer, integerColumn.Value);
        }

        [TestMethod]
        public void GetRegularColumns_NonAutoPrimaryKey_Test()
        {
            // Arrange

            var table = new ManualKeyPrimaryTable {Key1 = "ABCD", Key2 = 7};

            var typeGeneratorMock = Helpers.GetTypeGeneratorMock(table);
            var recordReference = new RecordReference<ManualKeyPrimaryTable>(typeGeneratorMock.Object);
            recordReference.Populate();
            var insertRecordService = new InsertRecordService(recordReference);

            // Act

            List<Column> regularColumns = insertRecordService.GetRegularColumns(null).ToList();

            // Assert

            Assert.AreEqual(2, regularColumns.Count);

            Column stringKey = regularColumns.First(c => c.Name == "Key1");
            Column intKey = regularColumns.First(c => c.Name == "Key2");

            Assert.AreEqual(table.Key1, stringKey.Value);
            Assert.AreEqual(table.Key2, intKey.Value);
        }

        [TestMethod]
        public void GetRegularColumns_WriteGuid_Test()
        {
            // Arrange

            var table = new ClassWithGuidKeys();

            Mock<ITypeGenerator> typeGeneratorMock = Helpers.GetTypeGeneratorMock(table);

            var recordReference = new RecordReference<ClassWithGuidKeys>(typeGeneratorMock.Object);
            recordReference.Populate();

            var insertRecordService = new InsertRecordService(recordReference);

            Variable v1, v2, v3;
            this.writerMock.Setup(m => m.WriteGuid("Key1")).Returns(v1 = new Variable("x"));
            this.writerMock.Setup(m => m.WriteGuid("Key3")).Returns(v2 = new Variable("y"));
            this.writerMock.Setup(m => m.WriteGuid("Key4")).Returns(v3 = new Variable("z"));

            // Act

            List<Column> regularColumns = insertRecordService.GetRegularColumns(this.writerMock.Object).ToList();

            // Assert

            this.writerMock.Verify(m => m.WriteGuid(It.IsAny<string>()), Times.Exactly(3));

            Column key1 = regularColumns.First(c => c.Name == "Key1");
            Column key3 = regularColumns.First(c => c.Name == "Key3");
            Column key4 = regularColumns.First(c => c.Name == "Key4");

            Assert.AreEqual(v1, key1.Value);
            Assert.AreEqual(v2, key3.Value);
            Assert.AreEqual(v3, key4.Value);
        }

        [TestMethod]
        public void GetRegularColumns_ExplicitlySetGuid_Test()
        {
            // Arrange

            var table = new ClassWithGuidKeys {Key1 = Guid.NewGuid(), Key3 = Guid.NewGuid(), Key4 = null};

            Mock<ITypeGenerator> typeGeneratorMock = Helpers.GetTypeGeneratorMock(table);
            var recordReference = new RecordReference<ClassWithGuidKeys>(typeGeneratorMock.Object);

            recordReference.Set(r => r.Key1, Guid.Empty).Set(r => r.Key3, Guid.Empty).Set(r => r.Key4, Guid.Empty);
            recordReference.Populate();

            var insertRecordService = new InsertRecordService(recordReference);

            // Act

            List<Column> regularColumns = insertRecordService.GetRegularColumns(null).ToList();

            // Assert

            Column key1 = regularColumns.First(c => c.Name == "Key1");
            Column key3 = regularColumns.First(c => c.Name == "Key3");
            Column key4 = regularColumns.First(c => c.Name == "Key4");

            Assert.AreEqual(table.Key1, key1.Value);
            Assert.AreEqual(table.Key3, key3.Value);
            Assert.AreEqual(null, key4.Value);
        }

        [TestMethod]
        public void GetForeignKeyColumns_ForeignKeyPrimaryKeyMatch_Test()
        {
            // Arrange

            const int primaryKeyValue = 5;

            var primaryKeySymbols = new List<ColumnSymbol>
            {
                new ColumnSymbol {ColumnName = "Key", TableType = typeof (PrimaryTable), Value = primaryKeyValue}
            };

            var primaryKeyInsertRecordMock = new Mock<InsertRecord>(null,
                new RecordReference<PrimaryTable>(Helpers.GetTypeGeneratorMock(new PrimaryTable()).Object),
                null);

            primaryKeyInsertRecordMock.Setup(m => m.GetPrimaryKeySymbols()).Returns(primaryKeySymbols);

            // Act

            List<Column> fkColumns =
                this.insertRecordService.GetForeignKeyColumns(new[] {primaryKeyInsertRecordMock.Object}).ToList();

            // Assert

            Assert.AreEqual(1, fkColumns.Count);
            Assert.AreEqual("ForeignKey", fkColumns[0].Name);
            Assert.AreEqual(primaryKeyValue, fkColumns[0].Value);
        }

        [TestMethod]
        public void GetForeignKeyColumns_NoForeignKeyPrimaryKeyMatch_Test()
        {
            // Arrange

            var primaryKeySymbols = new List<ColumnSymbol>
            {
                new ColumnSymbol {ColumnName = "Key1", TableType = typeof (ManualKeyPrimaryTable), Value = "ABCD"},
                new ColumnSymbol {ColumnName = "Key2", TableType = typeof (ManualKeyPrimaryTable), Value = 5},
            };

            var primaryKeyInsertRecordMock = new Mock<InsertRecord>(null,
                new RecordReference<ManualKeyPrimaryTable>(Helpers.GetTypeGeneratorMock(new ManualKeyPrimaryTable()).Object),
                null);

            primaryKeyInsertRecordMock.Setup(m => m.GetPrimaryKeySymbols()).Returns(primaryKeySymbols);

            // Act

            List<Column> fkColumns =
                this.insertRecordService.GetForeignKeyColumns(new[] { primaryKeyInsertRecordMock.Object }).ToList();

            // Assert

            Assert.AreEqual(1, fkColumns.Count);
            Assert.AreEqual("ForeignKey", fkColumns[0].Name);
            Assert.IsNull(fkColumns[0].Value);
        }

        [TestMethod]
        public void GetForeignKeyColumns_PropertyIsExplicitlySet_Test()
        {
            // Arrange

            const int explicitValue = 7;

            this.foreignKeyTable.ForeignKey = explicitValue;  // Simulate getting expicitly set value from type generator

            var primaryKeySymbols = new List<ColumnSymbol>
            {
                new ColumnSymbol {ColumnName = "Key", TableType = typeof (PrimaryTable), Value = 5}
            };

            var recordReference =
                new RecordReference<PrimaryTable>(Helpers.GetTypeGeneratorMock(new PrimaryTable()).Object);

            var primaryKeyInsertRecordMock = new Mock<InsertRecord>(null, recordReference, null);

            primaryKeyInsertRecordMock.Setup(m => m.GetPrimaryKeySymbols()).Returns(primaryKeySymbols);

            // Just record the fact that "ForeignKey" is explicitly set. Actual value comes from explicit value above.
            this.recordReference.Set(r => r.ForeignKey, default(int));  

            // Act

            List<Column> fkColumns =
                this.insertRecordService.GetForeignKeyColumns(new[] { primaryKeyInsertRecordMock.Object }).ToList();

            // Assert

            Assert.AreEqual(1, fkColumns.Count);
            Assert.AreEqual("ForeignKey", fkColumns[0].Name);
            Assert.AreEqual(explicitValue, fkColumns[0].Value);
        }
    }
}
