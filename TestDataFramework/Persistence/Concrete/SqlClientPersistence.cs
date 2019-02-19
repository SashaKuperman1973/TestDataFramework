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

using log4net;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.DbClientPopulator;
using TestDataFramework.RepositoryOperations;

namespace TestDataFramework.Persistence.Concrete
{
    public class SqlClientPersistence : IPersistence
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(SqlClientPersistence));
        private readonly IDeferredValueGenerator<LargeInteger> deferredValueGenerator;
        private readonly bool enforceKeyReferenceCheck;
        private readonly DbProviderFactory dbProviderFactory;
        private readonly DbClientConnection dbClientConnection;
        private readonly ISqlClientPersistenceService service;

        private DbClientTransaction transaction;

        public SqlClientPersistence(
            ISqlClientPersistenceService service,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator,
            bool enforceKeyReferenceCheck, DbProviderFactory dbProviderFactory, 
            DbClientConnection connection)
        {
            this.service = service;
            this.deferredValueGenerator = deferredValueGenerator;
            this.enforceKeyReferenceCheck = enforceKeyReferenceCheck;
            this.dbProviderFactory = dbProviderFactory;
            this.dbClientConnection = connection;
        }

        public virtual void Persist(IEnumerable<RecordReference> recordReferences)
        {
            recordReferences = recordReferences.ToList();

            if (!recordReferences.Any())
            {
                SqlClientPersistence.Logger.Debug("Empty recordReference collection. Exiting.");
                return;
            }

            if (this.transaction != null)
            {
                this.PersistWithTransaction(recordReferences.ToList());
            }
            else
            {
                this.PersistWithoutATransaction(recordReferences.ToList());
            }
        }

        private void PersistWithTransaction(List<RecordReference> recordReferences)
        {
            DbConnection connection = this.dbProviderFactory.CreateConnection();
            this.dbClientConnection.DbConnection = connection;
            connection.ConnectionString = this.dbClientConnection.ConnectionStringWithDefaultCatalogue;
            connection.Open();

            this.transaction.DbTransaction =
                connection.BeginTransaction(this.transaction.Options.IsolationLevel);

            this.dbClientConnection.DbTransaction = this.transaction.DbTransaction;

            this.transaction.OnDisposed = () =>
            {
                this.transaction.DbTransaction = null;
                this.dbClientConnection.DbTransaction = null;
                this.transaction.OnDisposed = null;
                this.transaction = null;
                connection.Dispose();
            };

            this.DoPersist(recordReferences);
        }

        private void PersistWithoutATransaction(List<RecordReference> recordReferences)
        {
            using (DbConnection connection = this.dbProviderFactory.CreateConnection())
            {
                this.dbClientConnection.DbConnection = connection;

                connection.ConnectionString = this.dbClientConnection.ConnectionStringWithDefaultCatalogue;
                connection.Open();

                this.DoPersist(recordReferences);
            }
        }

        private void DoPersist(List<RecordReference> recordReferences)
        {
            SqlClientPersistence.Logger.Debug("Entering Persist");

            SqlClientPersistence.Logger.Debug(
                $"Records: {string.Join(", ", recordReferences.Select(r => r?.RecordObjectBase))}");

            this.deferredValueGenerator.Execute(recordReferences);

            List<AbstractRepositoryOperation> operations =
                this.service.GetOperations(this.enforceKeyReferenceCheck, recordReferences).ToList();

            var orderedOperations = new AbstractRepositoryOperation[operations.Count];

            this.service.WriteOperations(operations, orderedOperations);

            this.service.ReadOrderedOperations(orderedOperations);

            SqlClientPersistence.Logger.Debug("Exiting Persist");
        }

        public virtual void UseTransaction(DbClientTransaction transaction)
        {
            this.transaction = transaction;
        }
    }
}