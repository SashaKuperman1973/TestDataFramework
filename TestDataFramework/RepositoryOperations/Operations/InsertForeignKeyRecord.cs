using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.RepositoryOperations.Operations
{
    public class InsertForeignKeyRecord : AbstractInsertRecord
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InsertForeignKeyRecord));

        public InsertForeignKeyRecord(RecordReference recordReference, IEnumerable<AbstractRepositoryOperation> peers, AbstractInsertRecord.KeyTypeEnum keyType)
        {
            InsertForeignKeyRecord.Logger.Debug("Entering constructor");

            this.KeyType = keyType;
            this.Peers = peers.ToList();
            this.RecordReference = recordReference;

            InsertForeignKeyRecord.Logger.Debug("Exiting constructor");
        }

        public override void Write(IWritePrimitives writer, CircularReferenceBreaker breaker)
        {
            InsertForeignKeyRecord.Logger.Debug("Entering Write");

            if (this.IsWriteDone)
            {
                InsertForeignKeyRecord.Logger.Debug("Write already done. Exiting.");

                return;
            }

            this.WriteHigherPriorityOperations(writer, breaker);
            this.WritePrimitives(writer);
            this.IsWriteDone = true;

            InsertForeignKeyRecord.Logger.Debug("Exiting Write");
        }

        private void WritePrimitives(IWritePrimitives writer)
        {
            InsertForeignKeyRecord.Logger.Debug("Entering WritePrimitives");

            InsertForeignKeyRecord.Logger.Debug("Exiting WritePrimitives");

            throw new NotImplementedException();
        }

        public override void Read()
        {
            InsertForeignKeyRecord.Logger.Debug("Entering Read");

            InsertForeignKeyRecord.Logger.Debug("Exiting Read");

            throw new NotImplementedException();
        }

        private void WriteHigherPriorityOperations(IWritePrimitives writer, CircularReferenceBreaker breaker)
        {
            InsertForeignKeyRecord.Logger.Debug("Entering WriteHigherPriorityOperations");

            List<AbstractRepositoryOperation> higherPriorityOperations = this.GetHigherPriorityOperations();

            higherPriorityOperations.ForEach(o => o.Write(writer, breaker));

            InsertForeignKeyRecord.Logger.Debug("Exiting WriteHigherPriorityOperations");
        }

        private List<AbstractRepositoryOperation> GetHigherPriorityOperations()
        {
            InsertForeignKeyRecord.Logger.Debug("Entering GetHigherPriorityOperations");

            List<AbstractRepositoryOperation> result = this.Peers.Where(
                peer =>
                {
                    var pkRecord = peer as InsertPrimaryKeyRecord;

                    if (pkRecord == null)
                    {
                        return false;
                    }

                    IEnumerable<ForeignKeyAttribute> foreignKeyAttributes =
                        this.RecordReference.RecordType.GetUniquePropertyAttributes<ForeignKeyAttribute>();

                    bool peersResult =
                        foreignKeyAttributes.Any(fka => fka.PrimaryTableType == pkRecord.RecordReference.RecordType);

                    return peersResult;

                }).ToList();

            if (!result.Any())
            {
                throw new NoPeersException();
            }

            InsertForeignKeyRecord.Logger.Debug("Exiting GetHigherPriorityOperations");

            return result;
        }
    }
}
