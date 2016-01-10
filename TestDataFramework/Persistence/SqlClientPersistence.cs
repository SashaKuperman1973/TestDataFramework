using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.RepositoryOperations.Operations;
using TestDataFramework.RepositoryOperations.Operations.InsertRecord;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.Persistence
{
    public class SqlClientPersistence : IPersistence
    {
        private readonly IWritePrimitives writePrimitives;
        private readonly IDeferredValueGenerator<ulong> deferredValueGenerator;

        public SqlClientPersistence(IWritePrimitives writePrimitives, IDeferredValueGenerator<ulong> deferredValueGenerator)
        {
            this.writePrimitives = writePrimitives;
            this.deferredValueGenerator = deferredValueGenerator;
        }

        public virtual void Persist(IEnumerable<RecordReference> recordReferences)
        {
            recordReferences = recordReferences.ToList();

            if (!recordReferences.ToList().Any())
            {
                return;
            }

            this.deferredValueGenerator.Execute(recordReferences.Select(r => r.RecordObject));

            var operations = new List<AbstractRepositoryOperation>();

            foreach (RecordReference recordReference in recordReferences)
            {
                operations.Add(new InsertRecord(new InsertRecordService(recordReference), recordReference, operations));
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
        }
    }
}
