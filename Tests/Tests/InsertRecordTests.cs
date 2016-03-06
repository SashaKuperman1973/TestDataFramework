/*
    Copyright 2016 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.RepositoryOperations.Operations.InsertRecord;
using TestDataFramework.WritePrimitives.Interfaces;
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

        private TableTypeCache tableTypeCache;
        private IAttributeDecorator attributeDecorator;

        private SubjectClass subject;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.tableTypeCache = new TableTypeCache();
            this.attributeDecorator = new StandardAttributeDecorator(this.tableTypeCache);

            this.subject = new SubjectClass();
            this.recordReferenceMock = new Mock<RecordReference>(null, this.attributeDecorator);
            this.peers = new List<AbstractRepositoryOperation>();
            this.serviceMock = new Mock<InsertRecordService>(this.recordReferenceMock.Object, this.attributeDecorator);
            this.insertRecord = new InsertRecord(this.serviceMock.Object, this.recordReferenceMock.Object, this.peers, this.attributeDecorator);

            this.breakerMock = new Mock<CircularReferenceBreaker>();
            this.writePrimitivesMock = new Mock<IWritePrimitives>();

            this.recordReferenceMock.Setup(m => m.RecordObject).Returns(this.subject);
            this.recordReferenceMock.Setup(m => m.RecordType).Returns(this.subject.GetType());
        }

        [TestMethod]
        public void Write_Test()
        {
            // Arrange

            var orderedOperations = new AbstractRepositoryOperation[1];
            var primaryKeyOperations = new List<InsertRecord>();
            var currentOrder = new Counter();
            var regularColumns = new List<Column>();
            var foreignKeyColumns = new List<ExtendedColumnSymbol>();
            var columnList = regularColumns.Concat(Helpers.ColumnSymbolToColumn(foreignKeyColumns));
            string tableName = typeof (SubjectClass).Name;

            this.serviceMock.Setup(m => m.GetPrimaryKeyOperations(this.peers)).Returns(primaryKeyOperations);
            this.serviceMock.Setup(m => m.GetRegularColumns(this.writePrimitivesMock.Object)).Returns(regularColumns);
            this.serviceMock.Setup(m => m.GetForeignKeyColumns(primaryKeyOperations)).Returns(foreignKeyColumns);

            // Act

            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, currentOrder, orderedOperations);

            // Assert

            this.breakerMock.Verify(m => m.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.insertRecord.Write), Times.Once);
            this.serviceMock.Verify(m => m.GetPrimaryKeyOperations(this.peers), Times.Once);
            this.serviceMock.Verify(
                m =>
                    m.WritePrimaryKeyOperations(this.writePrimitivesMock.Object, primaryKeyOperations,
                        this.breakerMock.Object, currentOrder, orderedOperations), Times.Once);

            this.serviceMock.Verify(m => m.GetRegularColumns(this.writePrimitivesMock.Object), Times.Once);
            this.serviceMock.Verify(m => m.GetForeignKeyColumns(primaryKeyOperations), Times.Once);

            Assert.AreEqual(this.insertRecord, orderedOperations[0]);

            this.serviceMock.Verify(
                m =>
                    m.WritePrimitives(this.writePrimitivesMock.Object, null, It.IsAny<string>(), tableName, columnList,
                        It.Is<List<ColumnSymbol>>(l => l.Count == 0)), Times.Once);

            this.serviceMock.Verify(m => m.CopyPrimaryToForeignKeyColumns(It.Is<IEnumerable<Column>>(c => !c.Any())), Times.Once());
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
            var secondObject = new SubjectClass();
            var secondrecordReferenceMock = new Mock<RecordReference>(null, this.attributeDecorator);
            secondrecordReferenceMock.Setup(m => m.RecordObject).Returns(secondObject);
            secondrecordReferenceMock.Setup(m => m.RecordType).Returns(secondObject.GetType());

            var secondInsertRecord = new InsertRecord(this.serviceMock.Object, secondrecordReferenceMock.Object, this.peers, this.attributeDecorator);
            var primaryKeyOperations = new List<InsertRecord> { secondInsertRecord };

            var regularColumns = new List<Column>();
            var foreignKeyColumns = new List<ExtendedColumnSymbol>();

            this.serviceMock.Setup(m => m.GetPrimaryKeyOperations(this.peers)).Returns(primaryKeyOperations);
            this.serviceMock.Setup(m => m.GetRegularColumns(this.writePrimitivesMock.Object)).Returns(regularColumns);
            this.serviceMock.Setup(m => m.GetForeignKeyColumns(primaryKeyOperations)).Returns(foreignKeyColumns);

            this.serviceMock.Setup(
                m =>
                    m.WritePrimaryKeyOperations(It.IsAny<IWritePrimitives>(), It.IsAny<IEnumerable<AbstractRepositoryOperation>>(),
                        It.IsAny<CircularReferenceBreaker>(), It.IsAny<Counter>(),
                        It.IsAny<AbstractRepositoryOperation[]>()))
                .Callback
                <IWritePrimitives, IEnumerable<AbstractRepositoryOperation>, CircularReferenceBreaker, Counter,
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
        public void WriteIsDone_FilterPasses_Test()
        {
            // Arrange

            var orderedOpertations = new AbstractRepositoryOperation[1];

            // Act

            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(), orderedOpertations);

            // Assert

            this.breakerMock.Verify(m => m.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.insertRecord.Write), Times.Once);
            Assert.AreEqual(this.insertRecord, orderedOpertations[0]);
        }

        [TestMethod]
        public void WriteIsDone_FilterFails_Test()
        {
            // Arrange

            var orderedOpertations = new AbstractRepositoryOperation[1];

            // Act

            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(), orderedOpertations);
            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(), orderedOpertations);

            // Assert

            this.breakerMock.Verify(m => m.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.insertRecord.Write), Times.Once);
        }

        [TestMethod]
        public void Read_AutoKey_Test()
        {
            // Arrange

            var record = new PrimaryTable();

            this.recordReferenceMock.Setup(m => m.RecordObject).Returns(record);
            this.recordReferenceMock.Setup(m => m.RecordType).Returns(record.GetType());

            var streamReadPointer = new Counter();

            const int expected = 8;

            var returnValue = new object[] { "Key", expected };

            // Act

            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(), new AbstractRepositoryOperation[1]);
            this.insertRecord.Read(streamReadPointer, returnValue);

            // Assert

            Assert.AreEqual(expected, record.Key);
            Assert.AreEqual(2, streamReadPointer.Value);
        }

        [TestMethod]
        public void Read_ManualKey_Test()
        {
            // Arrange

            var record = new ClassWithGuidKeys();

            this.recordReferenceMock.Setup(m => m.RecordObject).Returns(record);
            this.recordReferenceMock.Setup(m => m.RecordType).Returns(record.GetType());

            var streamReadPointer = new Counter();

            var returnValue = new object[] { "Key1", Guid.NewGuid(), "Key3", Guid.NewGuid(), "Key4", Guid.NewGuid() };

            // Act

            this.insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(), new AbstractRepositoryOperation[1]);
            this.insertRecord.Read(streamReadPointer, returnValue);

            // Assert

            Assert.AreEqual(returnValue[1], record.Key1);
            Assert.AreEqual(returnValue[3], record.Key3);
            Assert.AreEqual(6, streamReadPointer.Value);
        }
    }
}
