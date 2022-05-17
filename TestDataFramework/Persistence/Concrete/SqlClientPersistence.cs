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

using System;
using log4net;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.DbClientPopulator;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.ValueFormatter.Concrete;

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
        private readonly IAttributeDecorator attributeDecorator;
        private readonly SqlClientValueFormatter formatter = new SqlClientValueFormatter();

        private DbClientTransaction transaction;

        public SqlClientPersistence(
            ISqlClientPersistenceService service,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator,
            bool enforceKeyReferenceCheck, DbProviderFactory dbProviderFactory, 
            DbClientConnection connection,
            IAttributeDecorator attributeDecorator)
        {
            this.service = service;
            this.deferredValueGenerator = deferredValueGenerator;
            this.enforceKeyReferenceCheck = enforceKeyReferenceCheck;
            this.dbProviderFactory = dbProviderFactory;
            this.dbClientConnection = connection;
            this.attributeDecorator = attributeDecorator;
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

        public void DeleteAll(IEnumerable<RecordReference> recordReferences)
        {
            var sb = new StringBuilder();

            foreach (RecordReference recordReference in recordReferences)
            {
                try
                {
                    sb.AppendLine(this.GetDeleteSql(recordReference));
                }
                catch (SqlPersistenceException e)
                {
                    SqlClientPersistence.Logger.Warn($"{e.Message}. Skipping.");
                }
            }

            using (DbConnection connection = this.dbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = this.dbClientConnection.ConnectionStringWithDefaultCatalogue;
                connection.Open();

                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (DbCommand command = connection.CreateCommand())
                        {
                            command.Connection = connection;
                            command.Transaction = transaction;
                            command.CommandType = CommandType.Text;
                            command.CommandText = sb.ToString();

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private string GetDeleteSql(RecordReference recordReference)
        {
            TableName tableName = Helper.GetTableName(recordReference.RecordType, this.attributeDecorator);

            IEnumerable<PropertyAttribute<PrimaryKeyAttribute>> primaryKeys =
                this.attributeDecorator.GetPropertyAttributes<PrimaryKeyAttribute>(recordReference.RecordType);

            string tableNameString = $"[{(tableName.IsDefaultSchema ? "dbo" : tableName.Schema)}].[{tableName.Name}]";

            string deletePredicate = this.GetDeletePredicate(recordReference.RecordObjectBase, primaryKeys);

            if (string.IsNullOrEmpty(deletePredicate))
            {
                throw new SqlPersistenceException($"Deletion: Table '{tableNameString}', type '{recordReference.RecordType}' does not have a primary key");
            }

            string deleteSql = $"delete from {tableNameString} where {deletePredicate};";

            return deleteSql;
        }

        private string GetDeletePredicate(object objectValue, IEnumerable<PropertyAttribute<PrimaryKeyAttribute>> primaryKeys)
        {
            string criteria = string.Join(" and ",
                primaryKeys.Select(
                    key =>
                        $"{Helper.GetColumnName(key.PropertyInfoProxy, this.attributeDecorator)} = {this.formatter.Format(key.PropertyInfoProxy.GetValue(objectValue))}"));
            
            return criteria;
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