using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.RepositoryOperations.Operations.InsertRecord;
using TestDataFramework.WritePrimitives;

namespace Tests
{
    [TestClass]
    public class InsertRecordTests
    {
        private InsertRecord insertRecord;
        private Mock<InsertRecordService> serviceMock;
        private Mock<RecordReference> recordReferenceMock;
        private List<AbstractRepositoryOperation> peers;

        private Mock<CircularReferenceBreaker> breakerMock;
        private Mock<IWritePrimitives> writePrimitivesMock;

        [TestInitialize]
        public void Initialize()
        {
            this.recordReferenceMock = new Mock<RecordReference>(new object());
            this.peers = new List<AbstractRepositoryOperation>();
            this.serviceMock = new Mock<InsertRecordService>(this.recordReferenceMock.Object);
            this.insertRecord = new InsertRecord(this.serviceMock.Object, this.recordReferenceMock.Object, this.peers);

            this.breakerMock = new Mock<CircularReferenceBreaker>();
            this.writePrimitivesMock = new Mock<IWritePrimitives>();
        }

        [TestMethod]
        public void Write_Test()
        {
            // Arrange

            var orderedOperations = new AbstractRepositoryOperation[1];
            var primaryKeyOperations = new List<InsertRecord>();
            var currentOrder = new CurrentOrder();
            var columns = new Columns { ForeignKeyColumns = new List<Column>(), RegularColumns = new List<Column>()};
            var columnList = new List<Column>();

            this.serviceMock.Setup(m => m.GetPrimaryKeyOperations(this.peers)).Returns(primaryKeyOperations);
            this.serviceMock.Setup(m => m.GetColumnData(primaryKeyOperations)).Returns(columns);

            // Act

            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, currentOrder, orderedOperations);

            // Assert

            this.breakerMock.Verify(m => m.Push<IWritePrimitives, CurrentOrder, AbstractRepositoryOperation[]>(this.insertRecord.Write), Times.Once);
            this.serviceMock.Verify(m => m.GetPrimaryKeyOperations(this.peers), Times.Once);
            this.serviceMock.Verify(
                m =>
                    m.WritePrimaryKeyOperations(this.writePrimitivesMock.Object, primaryKeyOperations,
                        this.breakerMock.Object, currentOrder, orderedOperations), Times.Once);

            this.serviceMock.Verify(m => m.GetColumnData(primaryKeyOperations), Times.Once);

            Assert.AreEqual(this.insertRecord, orderedOperations[0]);

            this.serviceMock.Verify(m => m.WritePrimitives(this.writePrimitivesMock.Object, columnList, It.Is<List<ColumnSymbol>>(l => l.Count == 0)), Times.Once);

            this.serviceMock.Verify(m => m.CopyForeignKeyColumns(columns.ForeignKeyColumns), Times.Once());
        }

        [TestMethod]
        public void WriteIsVisited_Test()
        {
            // Arrange

            this.breakerMock.Setup(m => m.IsVisited<IWritePrimitives, CurrentOrder, AbstractRepositoryOperation[]>(this.insertRecord.Write)).Returns(true);

            // Act

            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new CurrentOrder(), new AbstractRepositoryOperation[1]);

            // Assert

            this.breakerMock.Verify(m => m.IsVisited<IWritePrimitives, CurrentOrder, AbstractRepositoryOperation[]>(this.insertRecord.Write), Times.Once);
            this.breakerMock.Verify(m => m.Push<IWritePrimitives, CurrentOrder, AbstractRepositoryOperation[]>(this.insertRecord.Write), Times.Never);
        }
    }
}
