/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.Persistence.Concrete;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.DbClientPopulator;
using TestDataFramework.RepositoryOperations;

namespace Tests.Tests
{
    [TestClass]
    public class SqlClientPersistenceTests
    {
        private IAttributeDecorator attributeDecorator;
        private Mock<ISqlClientPersistenceService> serviceMock;
        private Mock<IDeferredValueGenerator<LargeInteger>> deferredValueGeneratorMock;
        private Mock<DbProviderFactory> dbProviderFactoryMock;
        private Mock<DbClientConnection> connectionMock;

        private SqlClientPersistence persistence;

        [TestInitialize]
        public void Initialize()
        {
            this.attributeDecorator = new StandardAttributeDecorator(null, new AssemblyWrapper(), new Schema());
            this.serviceMock = new Mock<ISqlClientPersistenceService>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<LargeInteger>>();
            this.dbProviderFactoryMock = new Mock<DbProviderFactory>();
            this.connectionMock = new Mock<DbClientConnection>();

            this.persistence = new SqlClientPersistence(
                this.serviceMock.Object,
                this.deferredValueGeneratorMock.Object,
                true,
                this.dbProviderFactoryMock.Object,
                this.connectionMock.Object,
                this.attributeDecorator);
        }

        [TestMethod]
        public void PersistWithTransaction_Test()
        {
            // Arrange

            var dbTransactionMock = new Mock<DbTransaction>();
            var dbConnection = new FakeDbConnection(dbTransactionMock.Object);
            this.dbProviderFactoryMock.Setup(m => m.CreateConnection()).Returns(dbConnection);

            const string connectionString = "cn";
            this.connectionMock.SetupGet(m => m.ConnectionStringWithDefaultCatalogue).Returns(connectionString);

            // Act

            var transaction = new DbClientTransaction(new DbClientTransactionOptions());
            this.persistence.UseTransaction(transaction);

            var recordReferences = new RecordReference[1];

            this.persistence.Persist(recordReferences);

            // Assert

            this.connectionMock.VerifySet(m => m.DbConnection = dbConnection);
            this.connectionMock.VerifySet(m => m.DbTransaction = dbTransactionMock.Object);
            Assert.AreEqual(connectionString, dbConnection.ConnectionString);
            Assert.IsTrue(dbConnection.Opened);
            Assert.AreEqual(IsolationLevel.ReadCommitted, dbConnection.IsolationLevel);
            Assert.AreEqual(dbTransactionMock.Object, transaction.DbTransaction);
        }

        [TestMethod]
        public void PersistWithoutATransaction_Test()
        {
            // Arrange

            var dbConnection = new FakeDbConnection(new Mock<DbTransaction>().Object);
            this.dbProviderFactoryMock.Setup(m => m.CreateConnection()).Returns(dbConnection);

            const string connectionString = "cn";
            this.connectionMock.SetupGet(m => m.ConnectionStringWithDefaultCatalogue).Returns(connectionString);

            // Act

            var recordReferences = new RecordReference[1];

            this.persistence.Persist(recordReferences);

            // Assert

            this.connectionMock.VerifySet(m => m.DbConnection = dbConnection);
            this.connectionMock.VerifySet(m => m.DbTransaction = It.IsAny<DbTransaction>(), Times.Never);
            Assert.AreEqual(connectionString, dbConnection.ConnectionString);
            Assert.IsTrue(dbConnection.Opened);
            Assert.IsNull(dbConnection.IsolationLevel);
        }

        [TestMethod]
        public void DoPersist_Test()
        {
            // Arrange

            this.dbProviderFactoryMock.Setup(m => m.CreateConnection()).Returns(new Mock<DbConnection>().Object);

            var recordReferences = new RecordReference[1];
            var operations = new[] { new Mock<AbstractRepositoryOperation>().Object};

            this.serviceMock.Setup(m => m.GetOperations(true, recordReferences)).Returns(operations);

            // Act

            this.persistence.Persist(recordReferences);

            // Assert

            this.deferredValueGeneratorMock.Verify(m => m.Execute(recordReferences));

            this.serviceMock.Verify(m => m.WriteOperations(
                It.Is<List<AbstractRepositoryOperation>>(ops => ops[0] == operations[0]),
                It.Is<AbstractRepositoryOperation[]>(orderedOps => orderedOps.Length == 1)));

            this.serviceMock.Verify(
                m => m.ReadOrderedOperations(
                    It.Is<AbstractRepositoryOperation[]>(orderedOps => orderedOps.Length == 1)));
        }

        [TestMethod]
        public void Transaction_Is_Disposed_Test()
        {
            var dbConnection = new FakeDbConnection(new Mock<DbTransaction>().Object);
            this.dbProviderFactoryMock.Setup(m => m.CreateConnection()).Returns(dbConnection);

            // Act

            var transaction = new DbClientTransaction(new DbClientTransactionOptions());
            this.persistence.UseTransaction(transaction);

            var recordReferences = new RecordReference[1];

            this.persistence.Persist(recordReferences);
            transaction.Dispose();

            // Assert

            Assert.IsNull(transaction.DbTransaction);
            Assert.IsNull(transaction.OnDisposed);
            this.connectionMock.VerifySet(m => m.DbTransaction = null);
            Assert.IsTrue(dbConnection.IsDisposed);

            // Checking that a second call to Persist without
            // using a transaction will call PersistWithoutATransaction.
            Assert.AreEqual(IsolationLevel.ReadCommitted, dbConnection.IsolationLevel);
            dbConnection = new FakeDbConnection(new Mock<DbTransaction>().Object);
            this.dbProviderFactoryMock.Setup(m => m.CreateConnection()).Returns(dbConnection);
            this.persistence.Persist(recordReferences);
            Assert.IsNull(dbConnection.IsolationLevel);
        }

        public class FakeDbConnection : DbConnection
        {
            private readonly DbTransaction dbTransaction;

            public FakeDbConnection(DbTransaction dbTransaction)
            {
                this.dbTransaction = dbTransaction;
            }

            public IsolationLevel? IsolationLevel { get; set; }

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            {
                this.IsolationLevel = isolationLevel;
                return this.dbTransaction;
            }

            public override void Close()
            {
                throw new NotImplementedException();
            }

            public override void ChangeDatabase(string databaseName)
            {
                throw new NotImplementedException();
            }

            public bool Opened { get; set; }

            public override void Open()
            {
                this.Opened = true;
            }

            public override string ConnectionString { get; set; }
            public override string Database { get; }
            public override ConnectionState State { get; }
            public override string DataSource { get; }
            public override string ServerVersion { get; }

            protected override DbCommand CreateDbCommand()
            {
                throw new NotImplementedException();
            }

            public bool IsDisposed { get; set; }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                this.IsDisposed = true;
            }
        }
    }
}
