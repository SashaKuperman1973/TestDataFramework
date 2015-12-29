using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Populator;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.RepositoryOperations
{
    public abstract class AbstractRepositoryOperation
    {
        protected List<AbstractRepositoryOperation> Peers;

        public abstract void Write(IWritePrimitives writer, CircularReferenceBreaker breaker);
        public abstract void Read();

        protected bool IsWriteDone { get; set; }

        public RecordReference RecordReference { get; protected set; }
    }
}
