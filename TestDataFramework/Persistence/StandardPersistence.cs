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

            var insertOperations = new List<AbstractRepositoryOperation>();

            foreach (RecordReference recordReference in recordReferences)
            {
                insertOperations.Add(new InsertRecord(recordReference, insertOperations));
            }

            var orderedOperations = new AbstractRepositoryOperation[insertOperations.Count];

            var currentOrder = new CurrentOrder();

            insertOperations[0].Write(new CircularReferenceBreaker(), this.writePrimitives, currentOrder, orderedOperations);

            foreach (AbstractRepositoryOperation orderedOperation in orderedOperations)
            {
                orderedOperation.Read();
            }
        }
    }
}
