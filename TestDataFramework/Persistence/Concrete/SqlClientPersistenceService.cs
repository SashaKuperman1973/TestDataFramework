/*
    Copyright 2016, 2017, 2018, 2019, 2023 Alexander Kuperman

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
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.RepositoryOperations.Operations.InsertRecord;
using TestDataFramework.WritePrimitives.Interfaces;

namespace TestDataFramework.Persistence.Concrete
{
    public class SqlClientPersistenceService : ISqlClientPersistenceService
    {
        private readonly IAttributeDecorator attributeDecorator;
        private readonly IWritePrimitives writePrimitives;

        public SqlClientPersistenceService(
            IAttributeDecorator attributeDecorator,
            IWritePrimitives writePrimitives)
        {
            this.attributeDecorator = attributeDecorator;
            this.writePrimitives = writePrimitives;
        }

        public IEnumerable<AbstractRepositoryOperation> GetOperations(
            bool enforceKeyReferenceCheck, IEnumerable<RecordReference> recordReferences)
        {
            var operations = new List<AbstractRepositoryOperation>();

            foreach (RecordReference recordReference in recordReferences)
                operations.Add(
                    new InsertRecord(
                        new InsertRecordService(recordReference, this.attributeDecorator,
                            enforceKeyReferenceCheck),
                        recordReference, operations, this.attributeDecorator));

            return operations;
        }

        public void WriteOperations(List<AbstractRepositoryOperation> operations,
            AbstractRepositoryOperation[] orderedOperations)
        {
            var currentOrder = new Counter();

            foreach (AbstractRepositoryOperation operation in operations)
                operation.Write(new CircularReferenceBreaker(), this.writePrimitives, currentOrder,
                    orderedOperations);
        }

        public void ReadOrderedOperations(AbstractRepositoryOperation[] orderedOperations)
        {
            var readStreamPointer = new Counter();

            object[] returnValues = this.writePrimitives.Execute();

            foreach (AbstractRepositoryOperation orderedOperation in orderedOperations)
                orderedOperation.Read(readStreamPointer, returnValues);
        }
    }
}
