using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.KindsOfInformation;

namespace TestDataFramework.RepositoryOperations.Operations
{
    public class InsertForeignKeyRecord : AbstractInsertRecord
    {
        public InsertForeignKeyRecord(RecordReference recordReference, IEnumerable<AbstractRepositoryOperation> peers, AbstractInsertRecord.KeyTypeEnum keyType)
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
            long highestOrder = InsertForeignKeyRecord.GetHighestDependencyOrder(operationsToQuery, breaker);

            this.Order = highestOrder + 1;

            this.Done = true;
        }

        private List<IOrderInformation> GetOperationsToQuery()
        {
            List<IOrderInformation> result = this.Peers.Where(
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

                }).Cast<IOrderInformation>().ToList();

            if (!result.Any())
            {
                throw new NoPeersException();
            }

            return result;
        }

        private static long GetHighestDependencyOrder(List<IOrderInformation> operations, CircularReferenceBreaker breaker)
        {
            long highestOrder = long.MinValue;
            long order;

            operations.ForEach(
                o =>
                {
                    if ((order = o.GetOrder(breaker)) > highestOrder)
                        highestOrder = order;
                });

            return highestOrder;
        }
    }
}
