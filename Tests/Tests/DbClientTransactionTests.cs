using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator.Concrete.DbClientPopulator;
using IsolationLevel = System.Data.IsolationLevel;

namespace Tests.Tests
{
    [TestClass]
    public class DbClientTransactionTests
    {
        private DbClientTransaction dbClientTransaction;

        private Mock<DbTransaction> dbTransactionMock;

        [TestInitialize]
        public void Initialize()
        {
            this.dbTransactionMock = new Mock<DbTransaction>();

            this.dbClientTransaction =
                new DbClientTransaction(new DbClientTransactionOptions());
        }

        [TestMethod]
        public void Options_Test()
        {
            Assert.AreEqual(IsolationLevel.ReadCommitted, this.dbClientTransaction.Options.IsolationLevel);
        }

        [TestMethod]
        public void Commit_Test()
        {
            this.dbClientTransaction.DbTransaction = this.dbTransactionMock.Object;

            this.dbClientTransaction.Commit();

            this.dbTransactionMock.Verify(m => m.Commit());
        }

        [TestMethod]
        public void Commit_No_Internal_Transaction_Throws_Test()
        {
            Helpers.ExceptionTest(() => this.dbClientTransaction.Commit(), typeof(TransactionException),
                Messages.NoTransaction);
        }

        [TestMethod]
        public void Rollback_Test()
        {
            this.dbClientTransaction.DbTransaction = this.dbTransactionMock.Object;

            this.dbClientTransaction.Rollback();

            this.dbTransactionMock.Verify(m => m.Rollback());
        }

        [TestMethod]
        public void Rollback_No_Internal_Transaction_Throws_Test()
        {
            Helpers.ExceptionTest(() => this.dbClientTransaction.Rollback(), typeof(TransactionException),
                Messages.NoTransaction);
        }

        [TestMethod]
        public void Dispose_Test()
        {
            var fakeDbTransaction = new FakeDbTransaction();

            this.dbClientTransaction.DbTransaction = fakeDbTransaction;

            this.dbClientTransaction.Dispose();

            Assert.IsTrue(fakeDbTransaction.IsDisposed);
        }

        [TestMethod]
        public void Dispose_No_Internal_Transaction_Throws_Test()
        {
            Helpers.ExceptionTest(() => this.dbClientTransaction.Dispose(), typeof(TransactionException),
                Messages.NoTransaction);
        }

        [TestMethod]
        public void OnDisposed_Invoked_Test()
        {
            // Arrange

            this.dbClientTransaction.DbTransaction = this.dbTransactionMock.Object;

            bool isDisposed = false;

            this.dbClientTransaction.OnDisposed = () => isDisposed = true;

            // Act

            this.dbClientTransaction.Dispose();

            // Assert

            Assert.IsTrue(isDisposed);
        }

        public class FakeDbTransaction : DbTransaction
        {
            public override void Commit()
            {
                throw new NotImplementedException();
            }

            public override void Rollback()
            {
                throw new NotImplementedException();
            }

            protected override DbConnection DbConnection { get; }
            public override IsolationLevel IsolationLevel { get; }

            public bool IsDisposed { get; set; }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                this.IsDisposed = true;
            }
        }
    }
}
