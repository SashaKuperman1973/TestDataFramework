using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.RepositoryOperations
{
    public abstract class AbstractRepositoryOperation
    {
        protected IEnumerable<AbstractRepositoryOperation> Peers;

        public abstract void Write(CircularReferenceBreaker breaker, IWritePrimitives writer, Counter order, AbstractRepositoryOperation[] orderedOperations);
        public abstract void Read(Counter readStreamPointer, object[] data);

        protected bool IsWriteDone { get; set; }
        protected long Order { get; set; }

        public RecordReference RecordReference { get; protected set; }

        public override string ToString()
        {
            string result =
                $"IsWriteDone: {this.IsWriteDone}, Order: {this.Order}. Record object: {Helper.DumpObject(this.RecordReference.RecordObject)}";

            return result;
        }
    }
}
