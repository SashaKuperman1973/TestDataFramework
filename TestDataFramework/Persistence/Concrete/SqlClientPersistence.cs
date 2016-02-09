using System.Collections.Generic;
using System.Linq;
using log4net;
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

        public SqlClientPersistence(IWritePrimitives writePrimitives,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator)
        {
            this.writePrimitives = writePrimitives;
            this.deferredValueGenerator = deferredValueGenerator;
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

            SqlClientPersistence.Logger.Debug("Exiting Persist");
        }
    }
}