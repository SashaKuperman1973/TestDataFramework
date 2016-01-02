using System.Collections.Generic;
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
    public class InsertRecordTests
    {
        private InsertRecord insertRecord;
        private Mock<InsertRecordService> serviceMock;
        private Mock<RecordReference> recordReferenceMock;
        private List<AbstractRepositoryOperation> peers;

        private Mock<CircularReferenceBreaker> breakerMock;
        private Mock<IWritePrimitives> writePrimitivesMock;

        private SubjectClass subject;

        [TestInitialize]
        public void Initialize()
        {
            this.subject = new SubjectClass();
            this.recordReferenceMock = new Mock<RecordReference>(subject);
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
            var currentOrder = new Counter();
            var columns = new Columns { ForeignKeyColumns = new List<Column>(), RegularColumns = new List<Column>()};
            var columnList = new List<Column>();
            string tableName = typeof (SubjectClass).Name;

            this.serviceMock.Setup(m => m.GetPrimaryKeyOperations(this.peers)).Returns(primaryKeyOperations);
            this.serviceMock.Setup(m => m.GetColumnData(primaryKeyOperations)).Returns(columns);

            // Act

            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, currentOrder, orderedOperations);

            // Assert

            this.breakerMock.Verify(m => m.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.insertRecord.Write), Times.Once);
            this.serviceMock.Verify(m => m.GetPrimaryKeyOperations(this.peers), Times.Once);
            this.serviceMock.Verify(
                m =>
                    m.WritePrimaryKeyOperations(this.writePrimitivesMock.Object, primaryKeyOperations,
                        this.breakerMock.Object, currentOrder, orderedOperations), Times.Once);

            this.serviceMock.Verify(m => m.GetColumnData(primaryKeyOperations), Times.Once);

            Assert.AreEqual(this.insertRecord, orderedOperations[0]);

            this.serviceMock.Verify(m => m.WritePrimitives(this.writePrimitivesMock.Object, tableName, columnList, It.Is<List<ColumnSymbol>>(l => l.Count == 0)), Times.Once);

            this.serviceMock.Verify(m => m.CopyForeignKeyColumns(columns.ForeignKeyColumns), Times.Once());
        }

        [TestMethod]
        public void WriteIsVisited_Test()
        {
            // Arrange

            this.breakerMock.Setup(m => m.IsVisited<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.insertRecord.Write)).Returns(true);

            // Act

            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(), new AbstractRepositoryOperation[1]);

            // Assert

            this.breakerMock.Verify(m => m.IsVisited<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.insertRecord.Write), Times.Once);
            this.breakerMock.Verify(m => m.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.insertRecord.Write), Times.Never);
        }

        [TestMethod]
        public void WriteOperationOrder_Test()
        {
            // Arrange

            var orderedOpertations = new AbstractRepositoryOperation[2];
            var secondrecordReferenceMock = new Mock<RecordReference>(new SubjectClass());

            var secondInsertRecord = new InsertRecord(this.serviceMock.Object, secondrecordReferenceMock.Object, this.peers);
            var primaryKeyOperations = new List<InsertRecord> { secondInsertRecord };

            var columns = new Columns { ForeignKeyColumns = new List<Column>(), RegularColumns = new List<Column>() };

            this.serviceMock.Setup(m => m.GetPrimaryKeyOperations(this.peers)).Returns(primaryKeyOperations);
            this.serviceMock.Setup(m => m.GetColumnData(primaryKeyOperations)).Returns(columns);

            this.serviceMock.Setup(
                m =>
                    m.WritePrimaryKeyOperations(It.IsAny<IWritePrimitives>(), It.IsAny<IEnumerable<InsertRecord>>(),
                        It.IsAny<CircularReferenceBreaker>(), It.IsAny<Counter>(),
                        It.IsAny<AbstractRepositoryOperation[]>()))
                .Callback
                <IWritePrimitives, IEnumerable<InsertRecord>, CircularReferenceBreaker, Counter,
                    AbstractRepositoryOperation[]>(
                        (writer, secondPrimaryKeyOperations, breaker, secondCurrentOrder, secondOrderedOperations) =>
                        {
                            this.serviceMock.Setup(
                                n =>
                                    n.WritePrimaryKeyOperations(It.IsAny<IWritePrimitives>(),
                                        It.IsAny<IEnumerable<InsertRecord>>(),
                                        It.IsAny<CircularReferenceBreaker>(), It.IsAny<Counter>(),
                                        It.IsAny<AbstractRepositoryOperation[]>()));

                            secondInsertRecord.Write(breaker, writer, secondCurrentOrder, secondOrderedOperations);
                        });

            // Act

            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(), orderedOpertations);

            // Assert

            Assert.AreEqual(secondInsertRecord, orderedOpertations[0]);
            Assert.AreEqual(this.insertRecord, orderedOpertations[1]);
        }

        [TestMethod]
        public void WriteIsDone_Test()
        {
            // Arrange

            var orderedOpertations = new AbstractRepositoryOperation[1];
            this.serviceMock.Setup(m => m.GetColumnData(It.IsAny<IEnumerable<InsertRecord>>()))
                .Returns(new Columns {ForeignKeyColumns = new List<Column>(), RegularColumns = new List<Column>()});


            // Act

            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(), orderedOpertations);

            // Assert

            this.breakerMock.Verify(m => m.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.insertRecord.Write), Times.Once);
            Assert.AreEqual(this.insertRecord, orderedOpertations[0]);
        }

        [TestMethod]
        public void Read_Test()
        {
            // Arrange

            var streamReadPointer = new Counter();

            const int expected = 8;

            var returnValue = new object[] {expected};

            // Act

            this.insertRecord.Read(streamReadPointer, returnValue);

            // Assert

            Assert.AreEqual(expected, this.subject.Key);
        }
    }
}
