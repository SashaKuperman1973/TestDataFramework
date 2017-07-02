/*
    Copyright 2016, 2017 Alexander Kuperman

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

using System.Collections.Generic;
using System.Linq;
using log4net;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.RepositoryOperations.Operations.InsertRecord;
using TestDataFramework.WritePrimitives.Interfaces;

namespace TestDataFramework.Persistence.Concrete
{
    public class SqlClientPersistence : IPersistence
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (SqlClientPersistence));

        private readonly IWritePrimitives writePrimitives;
        private readonly IDeferredValueGenerator<LargeInteger> deferredValueGenerator;
        private readonly IAttributeDecorator attributeDecorator;
        private readonly bool enforceKeyReferenceCheck;

        public SqlClientPersistence(IWritePrimitives writePrimitives,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator,
            IAttributeDecorator attributeDecorator, bool enforceKeyReferenceCheck)
        {
            this.writePrimitives = writePrimitives;
            this.deferredValueGenerator = deferredValueGenerator;
            this.attributeDecorator = attributeDecorator;
            this.enforceKeyReferenceCheck = enforceKeyReferenceCheck;
        }

        public virtual void Persist(IEnumerable<RecordReference> recordReferences)
        {
            SqlClientPersistence.Logger.Debug("Entering Persist");

            recordReferences = recordReferences.ToList();

            if (!recordReferences.Any())
            {
                SqlClientPersistence.Logger.Debug("Empty recordReference collection. Exiting.");
                return;
            }

            SqlClientPersistence.Logger.Debug(
                $"Records: {string.Join(", ", recordReferences.Select(r => r?.RecordObject))}");

            this.deferredValueGenerator.Execute(recordReferences);

            var operations = new List<AbstractRepositoryOperation>();

            foreach (RecordReference recordReference in recordReferences)
            {
                operations.Add(
                    new InsertRecord(
                        new InsertRecordService(recordReference, this.attributeDecorator, this.enforceKeyReferenceCheck),
                        recordReference, operations, this.attributeDecorator));
            }

            var orderedOperations = new AbstractRepositoryOperation[operations.Count];

            var currentOrder = new Counter();

            foreach (AbstractRepositoryOperation operation in operations)
            {
                operation.Write(new CircularReferenceBreaker(), this.writePrimitives, currentOrder,
                    orderedOperations);
            }

            var readStreamPointer = new Counter();

            object[] returnValues = this.writePrimitives.Execute();

            foreach (AbstractRepositoryOperation orderedOperation in orderedOperations)
            {
                orderedOperation.Read(readStreamPointer, returnValues);
            }

            SqlClientPersistence.Logger.Debug("Exiting Persist");
        }
    }
}