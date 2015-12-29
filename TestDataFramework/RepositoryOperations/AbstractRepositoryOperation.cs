using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.RepositoryOperations
{
    public abstract class AbstractRepositoryOperation
    {
        protected List<AbstractRepositoryOperation> Peers;

        public abstract void Write(CircularReferenceBreaker breaker, IWritePrimitives writer, CurrentOrder order, AbstractRepositoryOperation[] orderedOperations);
        public abstract void Read();

        protected bool IsWriteDone { get; set; }
        protected long Order { get; set; }

        public RecordReference RecordReference { get; protected set; }
    }
}
