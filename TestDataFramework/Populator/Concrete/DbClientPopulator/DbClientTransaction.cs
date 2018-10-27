using System;
using System.Data.Common;
using System.Transactions;
using TestDataFramework.Exceptions;

namespace TestDataFramework.Populator.Concrete.DbClientPopulator
{
    public class DbClientTransaction : IDisposable
    {
        internal DbTransaction DbTransaction { get; set; }

        public DbClientTransaction(DbClientTransactionOptions options)
        {
            this.Options = options;
        }

        public DbClientTransactionOptions Options { get; }

        public virtual void Commit()
        {
            if (this.DbTransaction == null)
            {
                throw new TransactionException(Messages.NoTransaction);
            }

            this.DbTransaction.Commit();
        }

        public virtual void Rollback()
        {
            if (this.DbTransaction == null)
            {
                throw new TransactionException(Messages.NoTransaction);
            }

            this.DbTransaction.Rollback();
        }

        public virtual void Dispose()
        {
            this.DbTransaction.Dispose();
            this.OnDisposed?.Invoke();
        }

        internal Action OnDisposed { get; set; }
    }
}
