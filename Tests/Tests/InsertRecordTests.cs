/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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
using TestDataFramework;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.RepositoryOperations.Operations.InsertRecord;
using TestDataFramework.WritePrimitives.Interfaces;
using Tests.TestModels;
using TestDataFramework.Populator.Concrete;

namespace Tests.Tests
{
    [TestClass]
    public class InsertRecordTests
    {
        private Mock<IAttributeDecorator> attributeDecoratorMock;

        private Mock<CircularReferenceBreaker> breakerMock;

        private List<AbstractRepositoryOperation> peers;
            

        private Mock<InsertRecordService> serviceMock;

        private Mock<IWritePrimitives> writePrimitivesMock;

        private InsertRecord CreateInsertRecord<T>(T subject)
        {
            Mock<RecordReference<T>> recordReferenceMock = Helpers.GetMock<RecordReference<T>>();
            recordReferenceMock.Setup(m => m.RecordObjectBase).Returns(subject);
            recordReferenceMock.Setup(m => m.RecordType).Returns(typeof(T));

            var result = new InsertRecord(
                this.serviceMock.Object, recordReferenceMock.Object, this.peers,
                this.attributeDecoratorMock.Object);

            return result;
        }

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.attributeDecoratorMock = new Mock<IAttributeDecorator>();

            this.serviceMock = Helpers.GetMock<InsertRecordService>();

            this.breakerMock = new Mock<CircularReferenceBreaker>();
            this.peers = new List<AbstractRepositoryOperation>();
            this.writePrimitivesMock = new Mock<IWritePrimitives>();
        }

        [TestMethod]
        public void Write_Test()
        {
            // Arrange

            var orderedOperations = new AbstractRepositoryOperation[1];
            IEnumerable<InsertRecord> primaryKeyOperations = new List<InsertRecord>();
            var currentOrder = new Counter();
            IEnumerable<Column> regularColumns = new List<Column>();
            IEnumerable<ExtendedColumnSymbol> foreignKeyColumns = new List<ExtendedColumnSymbol>();
            IEnumerable<Column> columnList = regularColumns.Concat(Helpers.ColumnSymbolToColumn(foreignKeyColumns));
            string tableName = typeof(SubjectClass).Name;

            this.serviceMock.Setup(m => m.GetPrimaryKeyOperations(this.peers)).Returns(primaryKeyOperations);
            this.serviceMock.Setup(m => m.GetRegularColumns(this.writePrimitivesMock.Object)).Returns(regularColumns);
            this.serviceMock.Setup(m => m.GetForeignKeyColumns(primaryKeyOperations)).Returns(foreignKeyColumns);

            InsertRecord insertRecord = this.CreateInsertRecord(new SubjectClass());

            // Act

            insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, currentOrder,
                orderedOperations);

            // Assert

            this.breakerMock.Verify(
                m => m.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(insertRecord.Write),
                Times.Once);

            this.serviceMock.Verify(m => m.GetPrimaryKeyOperations(this.peers), Times.Once);

            this.serviceMock.Verify(
                m =>
                    m.WritePrimaryKeyOperations(this.writePrimitivesMock.Object, primaryKeyOperations,
                        this.breakerMock.Object, currentOrder, orderedOperations), Times.Once);

            this.serviceMock.Verify(m => m.GetRegularColumns(this.writePrimitivesMock.Object), Times.Once);
            this.serviceMock.Verify(m => m.GetForeignKeyColumns(primaryKeyOperations), Times.Once);

            Assert.AreEqual(insertRecord, orderedOperations[0]);

            this.serviceMock.Verify(
                m =>
                    m.WritePrimitives(this.writePrimitivesMock.Object, null, It.IsAny<string>(), tableName, columnList,
                        It.Is<List<ColumnSymbol>>(l => l.Count == 0)), Times.Once);

            this.serviceMock.Verify(m => m.CopyPrimaryToForeignKeyColumns(It.Is<IEnumerable<Column>>(c => !c.Any())),
                Times.Once());
        }

        [TestMethod]
        public void WriteIsVisited_Test()
        {
            // Arrange

            InsertRecord insertRecord = this.CreateInsertRecord(new SubjectClass());

            this.breakerMock
                .Setup(m => m.IsVisited<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(insertRecord
                    .Write)).Returns(true);

            // Act

            insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(),
                new AbstractRepositoryOperation[1]);

            // Assert

            this.breakerMock.Verify(
                m => m.IsVisited<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(insertRecord.Write),
                Times.Once);

            this.breakerMock.Verify(
                m => m.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(insertRecord.Write),
                Times.Never);
        }

        [TestMethod]
        public void WriteOperationOrder_Test()
        {
            // Arrange

            InsertRecord insertRecord = this.CreateInsertRecord(new SubjectClass());
            InsertRecord secondInsertRecord = this.CreateInsertRecord(new SubjectClass());

            var orderedOpertations = new AbstractRepositoryOperation[2];

            var primaryKeyOperations = new List<InsertRecord> {secondInsertRecord};

            IEnumerable<Column> regularColumns = Enumerable.Empty<Column>();
            IEnumerable<ExtendedColumnSymbol> foreignKeyColumns = Enumerable.Empty<ExtendedColumnSymbol>();

            this.serviceMock.Setup(m => m.GetPrimaryKeyOperations(this.peers)).Returns(primaryKeyOperations);
            this.serviceMock.Setup(m => m.GetRegularColumns(this.writePrimitivesMock.Object)).Returns(regularColumns);
            this.serviceMock.Setup(m => m.GetForeignKeyColumns(primaryKeyOperations)).Returns(foreignKeyColumns);

            this.serviceMock.Setup(
                    m =>
                        m.WritePrimaryKeyOperations(It.IsAny<IWritePrimitives>(),
                            It.IsAny<IEnumerable<AbstractRepositoryOperation>>(),
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

            insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(),
                orderedOpertations);

            // Assert

            Assert.AreEqual(secondInsertRecord, orderedOpertations[0]);
            Assert.AreEqual(insertRecord, orderedOpertations[1]);
        }

        [TestMethod]
        public void WriteIsDone_FilterPasses_Test()
        {
            // Arrange

            InsertRecord insertRecord = this.CreateInsertRecord(new SubjectClass());

            var orderedOpertations = new AbstractRepositoryOperation[1];

            // Act

            insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(),
                orderedOpertations);

            // Assert

            this.breakerMock.Verify(
                m => m.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(insertRecord.Write),
                Times.Once);
            Assert.AreEqual(insertRecord, orderedOpertations[0]);
        }

        [TestMethod]
        public void WriteIsDone_FilterFails_Test()
        {
            // Arrange

            InsertRecord insertRecord = this.CreateInsertRecord(new SubjectClass());

            var orderedOpertations = new AbstractRepositoryOperation[1];

            // Act

            insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(),
                orderedOpertations);
            insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(),
                orderedOpertations);

            // Assert

            this.breakerMock.Verify(
                m => m.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(insertRecord.Write),
                Times.Once);
        }

        [TestMethod]
        public void Read_AutoKey_Test()
        {
            // Arrange

            var record = new PrimaryTable();
            InsertRecord insertRecord = this.CreateInsertRecord(record);

            var streamReadPointer = new Counter();

            this.attributeDecoratorMock.Setup(m => m.GetPropertyAttributes(typeof(PrimaryTable)))
                .Returns(new[]
                {
                    new PropertyAttributes
                    {
                        Attributes = new Attribute[] {new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Auto)},
                        PropertyInfo = typeof(PrimaryTable).GetProperty(nameof(PrimaryTable.Key))
                    }
                });

            // Act

            insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(),
                new AbstractRepositoryOperation[1]);

            const int expected = 8;

            var data = new object[] { "Key", expected, "Guid", Guid.NewGuid() };
            insertRecord.Read(streamReadPointer, data);

            // Assert

            Assert.AreEqual(expected, record.Key);
            Assert.AreEqual(2, streamReadPointer.Value);
        }

        [TestMethod]
        public void Read_ManualKey_Test()
        {
            // Arrange

            var record = new ClassWithGuidKeys();
            InsertRecord insertRecord = this.CreateInsertRecord(record);

            var streamReadPointer = new Counter();

            var data = new object[] {"Key1", Guid.NewGuid(), "Key3", Guid.NewGuid(), "Key4", Guid.NewGuid()};

            this.attributeDecoratorMock.Setup(m => m.GetPropertyAttributes(typeof(ClassWithGuidKeys)))
                .Returns(new[]
                {
                    new PropertyAttributes
                    {
                        Attributes = new Attribute[] {new PrimaryKeyAttribute()},
                        PropertyInfo = typeof(ClassWithGuidKeys).GetProperty(nameof(ClassWithGuidKeys.Key1))
                    },

                    new PropertyAttributes
                    {
                        Attributes = new Attribute[] {new PrimaryKeyAttribute()},
                        PropertyInfo = typeof(ClassWithGuidKeys).GetProperty(nameof(ClassWithGuidKeys.Key3))
                    },

                    new PropertyAttributes
                    {
                        Attributes = new Attribute[] {new PrimaryKeyAttribute()},
                        PropertyInfo = typeof(ClassWithGuidKeys).GetProperty(nameof(ClassWithGuidKeys.Key4))
                    },

                });

            // Act

            insertRecord.Write(this.breakerMock.Object, this.writePrimitivesMock.Object, new Counter(),
                new AbstractRepositoryOperation[1]);

            insertRecord.Read(streamReadPointer, data);

            // Assert

            Assert.AreEqual(data[1], record.Key1);
            Assert.AreEqual(data[3], record.Key3);
            Assert.AreEqual(data[5], record.Key4);
            Assert.AreEqual(6, streamReadPointer.Value);
        }

        [TestMethod]
        public void Conversion_Overflow_Test()
        {
            // Arrange

            InsertRecord insertRecord = this.CreateInsertRecord(new SubjectClass());

            var streamWritePointer = new Counter();
            var streamReadPointer = new Counter();

            this.peers.Add(insertRecord);

            this.attributeDecoratorMock.Setup(m => m.GetPropertyAttributes(typeof(SubjectClass)))
                .Returns(new[]
                {
                    new PropertyAttributes
                    {
                        Attributes = new Attribute[] {new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Auto)},
                        PropertyInfo = typeof(SubjectClass).GetProperty(nameof(SubjectClass.Key))
                    }
                });

            insertRecord.Write(this.breakerMock.Object, new Mock<IWritePrimitives>().Object, streamWritePointer, this.peers.ToArray());

            // Act/Assert

            Helpers.ExceptionTest(
                () => insertRecord.Read(streamReadPointer, new object[] {nameof(PrimaryTable.Key), long.MaxValue}),
                typeof(OverflowException), Messages.TypeTooNarrow.Substring(0, 20), MessageOption.MessageStartsWith);
        }
    }
}