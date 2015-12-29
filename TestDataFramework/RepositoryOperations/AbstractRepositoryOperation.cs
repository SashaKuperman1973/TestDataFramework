using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Populator;

namespace TestDataFramework.RepositoryOperations
{
    public abstract class AbstractRepositoryOperation
    {
        protected List<AbstractRepositoryOperation> Peers;

        public abstract void Write();
        public abstract void Read();

        public abstract void QueryPeers(CircularReferenceBreaker breaker);

        public virtual long Order { get; protected set; }
        public RecordReference RecordReference { get; protected set; }
    }
}
