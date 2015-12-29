using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.RepositoryOperations.Operations
{
    public class InsertPrimaryKeyRecord : AbstractInsertRecord
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InsertPrimaryKeyRecord));

        public InsertPrimaryKeyRecord(RecordReference recordReference, IEnumerable<AbstractRepositoryOperation> peers, AbstractInsertRecord.KeyTypeEnum keyType)
        {
            InsertPrimaryKeyRecord.Logger.Debug("Entering constructor");

            this.KeyType = keyType;
            this.Peers = peers.ToList();
            this.RecordReference = recordReference;

            InsertPrimaryKeyRecord.Logger.Debug("Exiting constructor");
        }

        public override void Write(IWritePrimitives writer, CircularReferenceBreaker breaker)
        {
            InsertPrimaryKeyRecord.Logger.Debug("Entering Write");

            if (this.IsWriteDone)
            {
                InsertPrimaryKeyRecord.Logger.Debug("Write already done. Exiting.");
                return;
            }

            this.WritePrimitives(writer);
            this.IsWriteDone = true;

            InsertPrimaryKeyRecord.Logger.Debug("Exiting Write");
        }

        private void WritePrimitives(IWritePrimitives writer)
        {
            InsertPrimaryKeyRecord.Logger.Debug("Entering WritePrimitives");

            InsertPrimaryKeyRecord.Logger.Debug("Exiting WritePrimitives");

            throw new NotImplementedException();
        }

        public override void Read()
        {
            InsertPrimaryKeyRecord.Logger.Debug("Entering Read");

            InsertPrimaryKeyRecord.Logger.Debug("Exiting Read");

            throw new NotImplementedException();
        }
    }
}
