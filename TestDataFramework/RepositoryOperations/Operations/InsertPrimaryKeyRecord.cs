using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.KindsOfInformation;

namespace TestDataFramework.RepositoryOperations.Operations
{
    public class InsertPrimaryKeyRecord : AbstractInsertRecord
    {
        public InsertPrimaryKeyRecord(RecordReference recordReference, IEnumerable<AbstractRepositoryOperation> peers, AbstractInsertRecord.KeyTypeEnum keyType)
        {
            this.KeyType = keyType;
            this.Peers = peers.ToList();
            this.RecordReference = recordReference;
        }

        public override void Write()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            throw new NotImplementedException();
        }

        public override void QueryPeers(CircularReferenceBreaker breaker)
        {
            List<IOrderInformation> operationsToQuery = this.GetOperationsToQuery();
            long lowestOrder = InsertPrimaryKeyRecord.GetLowestDependencyOrder(operationsToQuery, breaker);

            this.Order = lowestOrder - 1;

            this.Done = true;
        }

        private List<IOrderInformation> GetOperationsToQuery()
        {
            List<IOrderInformation> result = this.Peers.Where(peer =>
            {
                var fkRecord = peer as InsertForeignKeyRecord;

                if (fkRecord == null)
                {
                    return false;
                }

                IEnumerable<ForeignKeyAttribute> foreignKeyAttributes =
                    fkRecord.RecordReference.RecordType.GetUniquePropertyAttributes<ForeignKeyAttribute>();

                bool peersResult =
                    foreignKeyAttributes.Any(fka => fka.PrimaryTableType == this.RecordReference.RecordType);

                return peersResult;

            }).Cast<IOrderInformation>().ToList();

            if (!result.Any())
            {
                throw new NoPeersException();
            }

            return result;
        }

        private static long GetLowestDependencyOrder(List<IOrderInformation> operations, CircularReferenceBreaker breaker)
        {
            long lowestOrder = long.MaxValue;
            long order;

            operations.ForEach(
                o =>
                {
                    if ((order = o.GetOrder(breaker)) < lowestOrder)
                        lowestOrder = order;
                });

            return lowestOrder;
        }
    }
}
