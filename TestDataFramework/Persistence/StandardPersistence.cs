using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.RepositoryOperations.Operations;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.Persistence
{
    public class StandardPersistence : IPersistence
    {
        private readonly IWritePrimitives writePrimitives;

        public StandardPersistence(IWritePrimitives writePrimitives)
        {
            this.writePrimitives = writePrimitives;
        }

        public virtual void Persist(IEnumerable<RecordReference> recordReferences)
        {
            recordReferences = recordReferences.ToList();

            if (!recordReferences.ToList().Any())
            {
                return;
            }

            var operations = new List<AbstractRepositoryOperation>();

            foreach (RecordReference recordReference in recordReferences)
            {
                operations.Add(new InsertRecord(recordReference, operations));
            }

            var orderedOperations = new AbstractRepositoryOperation[operations.Count];

            var currentOrder = new CurrentOrder();

            foreach (AbstractRepositoryOperation operation in operations)
            {
                operation.Write(new CircularReferenceBreaker(), this.writePrimitives, currentOrder,
                    orderedOperations);
            }

            foreach (AbstractRepositoryOperation orderedOperation in orderedOperations)
            {
                orderedOperation.Read();
            }
        }
    }
}
