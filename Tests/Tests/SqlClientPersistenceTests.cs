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
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.Persistence.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class SqlClientPersistenceTests
    {
        private IAttributeDecorator attributeDecorator;
        private Mock<IDeferredValueGenerator<LargeInteger>> deferredValueGeneratorMock;
        private Mock<IWritePrimitives> writePrimitivesMock;

        private SqlClientPersistence persistence;

        [TestInitialize]
        public void Initialize()
        {
            this.attributeDecorator = new StandardAttributeDecorator(null, new AssemblyWrapper(), new Schema());
            this.writePrimitivesMock = new Mock<IWritePrimitives>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<LargeInteger>>();

            this.persistence = new SqlClientPersistence(this.writePrimitivesMock.Object,
                this.deferredValueGeneratorMock.Object, this.attributeDecorator, true);

            XmlConfigurator.Configure();
        }

        [TestMethod]
        public void Insert_Test()
        {
            // Arrange

            var primaryTable = new PrimaryTable {Integer = 5, Text = "Text"};

            var primaryRecordReference =
                new RecordReference<PrimaryTable>(Helpers.GetTypeGeneratorMock(primaryTable).Object,
                    this.attributeDecorator, null, null, null, null);
            primaryRecordReference.Populate();

            string tableName = typeof(PrimaryTable).Name;

            List<Column> primaryTableColumns = null;

            this.writePrimitivesMock.Setup(m => m.Insert(null, It.IsAny<string>(), tableName,
                    It.IsAny<IEnumerable<Column>>()))
                .Callback<string, string, string, IEnumerable<Column>>((cat, s, t, col) => primaryTableColumns =
                    col.ToList());

            this.writePrimitivesMock.Setup(m => m.Execute()).Returns(new object[] {"Key", 0, "Guid", Guid.NewGuid()});

            // Act

            var recordReferenceArray = new RecordReference[] {primaryRecordReference};

            this.persistence.Persist(recordReferenceArray);

            // Assert

            this.writePrimitivesMock.Verify(
                m => m.Insert(null, It.IsAny<string>(), tableName, It.IsAny<IEnumerable<Column>>()), Times.Once());

            this.deferredValueGeneratorMock.Verify(
                m => m.Execute(It.Is<IEnumerable<RecordReference>>(e => e.First() == recordReferenceArray[0])),
                Times.Once);

            Assert.IsNotNull(primaryTableColumns);
            Assert.AreEqual(4, primaryTableColumns.Count);

            Column textColumn = primaryTableColumns.Single(col => col.Name == "Text");
            Assert.AreEqual(primaryTable.Text, textColumn.Value);

            Column integerColumn = primaryTableColumns.Single(col => col.Name == "Integer");
            Assert.AreEqual(primaryTable.Integer, integerColumn.Value);
        }

        [TestMethod]
        public void InsertsInProperOrder_Test()
        {
            // Arrange

            var primaryTable = new PrimaryTable {Integer = 1};
            var primaryRecordReference =
                new RecordReference<PrimaryTable>(Helpers.GetTypeGeneratorMock(primaryTable).Object,
                    this.attributeDecorator, null, null, null, null);
            primaryRecordReference.Populate();

            var foreignTable = new ForeignTable {Integer = 1};
            var foreignRecordReference =
                new RecordReference<ForeignTable>(Helpers.GetTypeGeneratorMock(foreignTable).Object,
                    this.attributeDecorator, null, null, null, null);
            foreignRecordReference.Populate();

            foreignRecordReference.AddPrimaryRecordReference(primaryRecordReference);

            var columns = new List<List<Column>>();

            this.writePrimitivesMock.Setup(m => m.Insert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<IEnumerable<Column>>()))
                .Callback<string, string, string, IEnumerable<Column>>((cat, s, t, col) => columns.Add(col.ToList()));

            this.writePrimitivesMock.Setup(m => m.SelectIdentity(It.IsAny<string>())).Returns(new Variable(null));

            this.writePrimitivesMock.Setup(m => m.Execute())
                .Returns(new object[] {"Key", 0, "Guid", Guid.NewGuid(), "Key", 0});

            // Act

            // Note the foreign key record is being passed in before the primary key record 
            // to test that the primary key record writes first regardless which insert operation's
            // Write method is called.

            this.persistence.Persist(new RecordReference[] {foreignRecordReference, primaryRecordReference});

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

            var primaryRecordReference =
                new RecordReference<ManualKeyPrimaryTable>(Helpers.GetTypeGeneratorMock(primaryTable).Object,
                    this.attributeDecorator, null, null, null, null);
            primaryRecordReference.Populate();

            var foreignRecordReference =
                new RecordReference<ManualKeyForeignTable>(Helpers.GetTypeGeneratorMock(foreignTable).Object,
                    this.attributeDecorator, null, null, null, null);
            foreignRecordReference.Populate();

            foreignRecordReference.AddPrimaryRecordReference(primaryRecordReference);
            const string catalogueName = "catABC";
            const string schema = "schemaABC";
            const string tableName = "ABCD";

            var columns = new List<List<Column>>();

            this.writePrimitivesMock.Setup(m => m.Insert(catalogueName, schema, tableName,
                    It.IsAny<IEnumerable<Column>>()))
                .Callback<string, string, string, IEnumerable<Column>>((cat, s, t, col) => columns.Add(col.ToList()));

            // Act

            this.persistence.Persist(new RecordReference[] {foreignRecordReference, primaryRecordReference});

            // Assert

            Assert.AreEqual(primaryTable.Key1, foreignTable.ForeignKey1);
            Assert.AreEqual(primaryTable.Key2, foreignTable.ForeignKey2);
        }

        [TestMethod]
        public void ForeignKeyCopiedFromAutoPrimaryKey_InCorrectOrder_Test()
        {
            // Arrange

            var primaryTable = new PrimaryTable();
            var primaryRecordReference =
                new RecordReference<PrimaryTable>(Helpers.GetTypeGeneratorMock(primaryTable).Object,
                    this.attributeDecorator, null, null, null, null);
            primaryRecordReference.Populate();

            var foreignTable = new ForeignTable();
            var foreignRecordReference =
                new RecordReference<ForeignTable>(Helpers.GetTypeGeneratorMock(foreignTable).Object,
                    this.attributeDecorator, null, null, null, null);
            foreignRecordReference.Populate();

            foreignRecordReference.AddPrimaryRecordReference(primaryRecordReference);

            var expected = new object[] {"Key", 1, "Guid", Guid.NewGuid(), "Key", 2};

            this.writePrimitivesMock.Setup(m => m.Execute()).Returns(expected);

            // Act

            // Note the foreign key record is being passed in before the primary key record. 
            // This is to test that the primary key record that wrote first gets the first return
            // data element and the foreign key record gets the subsequent one.

            this.persistence.Persist(new RecordReference[] {foreignRecordReference, primaryRecordReference});

            // Assert

            Assert.AreEqual(expected[1], primaryTable.Key);
            Assert.AreEqual(expected[5], foreignTable.Key);
        }

        [TestMethod]
        public void Persist_EmptyInput_Test()
        {
            this.persistence.Persist(new RecordReference[0]);

            this.deferredValueGeneratorMock.VerifyNoOtherCalls();
            this.writePrimitivesMock.VerifyNoOtherCalls();
        }
    }
}