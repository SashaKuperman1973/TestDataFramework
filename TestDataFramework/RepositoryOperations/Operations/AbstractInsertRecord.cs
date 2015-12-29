using System.Collections.Generic;
using System.Linq;
using TestDataFramework.Exceptions;
using TestDataFramework.RepositoryOperations.KindsOfInformation;

namespace TestDataFramework.RepositoryOperations.Operations
{
    public abstract class AbstractInsertRecord : AbstractRepositoryOperation, IOrderInformation
    {
        public enum KeyTypeEnum
        {
            Auto,
            Manual,
            None,
        }

        protected KeyTypeEnum KeyType;

        public long GetOrder(CircularReferenceBreaker breaker)
        {
            if (this.Done)
            {
                return this.Order;
            }

            if (breaker.Visited(this))
            {
                return default(long);
            }

            this.QueryPeers(breaker);
            return this.Order;
        }

        public bool Done { get; protected set; }
    }
}
