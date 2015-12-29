using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Filter;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.RepositoryOperations.Operations
{
    public class InsertRecord : AbstractRepositoryOperation
    {
        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger(typeof(InsertRecord));
        private readonly PrimaryKeyAttribute.KeyTypeEnum keyType;
        private readonly List<ColumnSymbol> primaryKeyValues = new List<ColumnSymbol>();

        #endregion Private Fields

        #region Public methods

        public InsertRecord(RecordReference recordReference, IEnumerable<AbstractRepositoryOperation> peers)
        {
            InsertRecord.Logger.Debug("Entering constructor");

            this.Peers = peers.ToList();
            this.RecordReference = recordReference;

            this.keyType = this.DetermineKeyType();

            InsertRecord.Logger.Debug("Exiting constructor");
        }

        private PrimaryKeyAttribute.KeyTypeEnum DetermineKeyType()
        {
            IEnumerable<PrimaryKeyAttribute> pkAttributes =
                this.RecordReference.RecordType.GetUniquePropertyAttributes<PrimaryKeyAttribute>().ToList();

            if (!pkAttributes.Any())
            {
                return PrimaryKeyAttribute.KeyTypeEnum.None;
            }

            if (pkAttributes.Count() > 1)
            {
                return PrimaryKeyAttribute.KeyTypeEnum.Manual;
            }

            PrimaryKeyAttribute.KeyTypeEnum result = pkAttributes.First().KeyType;
            return result;
        }

        public override void Write(CircularReferenceBreaker breaker, IWritePrimitives writer, CurrentOrder currentOrder, AbstractRepositoryOperation[] orderedOperations)
        {
            InsertRecord.Logger.Debug("Entering Write:" + this.DumpObject());

            if (breaker.IsVisited<IWritePrimitives, CurrentOrder, AbstractRepositoryOperation[]>(this.Write))
            {
                InsertRecord.Logger.Debug("Write already visited. Exiting");
                return;
            }

            if (this.IsWriteDone)
            {
                InsertRecord.Logger.Debug("Write already done. Exiting");
                return;
            }

            breaker.Push<IWritePrimitives, CurrentOrder, AbstractRepositoryOperation[]>(this.Write);

            List<InsertRecord> primaryKeyOperations =  this.GetPrimaryKeyOperations();

            InsertRecord.WritePrimaryKeyOperations(writer, primaryKeyOperations, breaker, currentOrder, orderedOperations);

            List<Column> columnData = this.GetColumnData(primaryKeyOperations);

            this.Order = currentOrder.Value++;
            orderedOperations[this.Order] = this;

            this.WritePrimitives(writer, columnData);

            this.IsWriteDone = true;

            breaker.Pop();

            InsertRecord.Logger.Debug("Exiting Write");
        }

        public override void Read()
        {
            InsertRecord.Logger.Debug("Entering Read");

            InsertRecord.Logger.Debug("Exiting Read");
        }

        public IEnumerable<ColumnSymbol> GetPrimaryKeySymbols()
        {
            return this.primaryKeyValues;
        }

        #endregion Public methods

        #region Write related private methods

        private string DumpObject()
        {
            return Helper.DumpObject(this.RecordReference.RecordObject);
        }

        private List<Column> GetColumnData(List<InsertRecord> primaryKeyOperations)
        {
            InsertRecord.Logger.Debug("Entering GetColumnData");

            List<Column> regularColumns = this.GetRegularColumns();
            List<Column> foreignKeyColumns = this.GetForeignKeyColumns(primaryKeyOperations);

            List<Column> result = regularColumns.Concat(foreignKeyColumns).ToList();

            InsertRecord.Logger.Debug("Exiting GetColumnData");
            return result;
        }

        private List<Column> GetForeignKeyColumns(List<InsertRecord> primaryKeyOperations)
        {
            InsertRecord.Logger.Debug("Entering GetForeignKeyVariables");

            IEnumerable<ColumnSymbol> symbolList = primaryKeyOperations.SelectMany(o => o.GetPrimaryKeySymbols());

            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> propertyAttributes = this.RecordReference.RecordType.GetPropertyAttributes<ForeignKeyAttribute>();

            List<Column> foreignKeyColumns = symbolList.Select(sl =>
            {
                PropertyAttribute<ForeignKeyAttribute> matchingPropertyAttribute =
                    propertyAttributes.First(
                        pa =>
                            sl.TableType == pa.Attribute.PrimaryTableType &&
                            sl.ColumnName == pa.Attribute.PrimaryKeyName);

                return new Column
                {
                    Name = Helper.GetColunName(matchingPropertyAttribute.PropertyInfo),
                    Value = sl.Value
                };

            }).ToList();

            InsertRecord.Logger.Debug("Exiting GetForeignKeyVariables");

            return foreignKeyColumns;
        }

        private List<Column> GetRegularColumns()
        {
            InsertRecord.Logger.Debug("Entering GetRegularColumnData");

            IEnumerable<Column> result =
                this.RecordReference.RecordType.GetProperties()
                    .Where(
                        p =>
                            p.GetSingleAttribute<ForeignKeyAttribute>() == null &&
                            (p.GetSingleAttribute<PrimaryKeyAttribute>() == null ||
                             this.keyType == PrimaryKeyAttribute.KeyTypeEnum.Manual))
                    .Select(
                        p =>
                            new Column
                            {
                                Name = Helper.GetColunName(p),
                                Value = p.GetValue(this.RecordReference.RecordObject)
                            });

            InsertRecord.Logger.Debug("Exiting GetRegularColumnData");

            return result.ToList();
        }

        private static void WritePrimaryKeyOperations(IWritePrimitives writer, List<InsertRecord> primaryKeyOperations,
            CircularReferenceBreaker breaker, CurrentOrder currentOrder, AbstractRepositoryOperation[] orderedOperations)
        {
            InsertRecord.Logger.Debug("Entering WriteHigherPriorityOperations");

            primaryKeyOperations.ForEach(o => o.Write(breaker, writer, currentOrder, orderedOperations));

            InsertRecord.Logger.Debug("Exiting WriteHigherPriorityOperations");
        }

        private List<InsertRecord> GetPrimaryKeyOperations()
        {
            InsertRecord.Logger.Debug("Entering GetPrimaryKeyOperations");

            List<InsertRecord> result = this.Peers.Where(
                peer =>
                {
                    var pkRecord = peer as InsertRecord;

                    if (pkRecord == this || pkRecord == null)
                    {
                        return false;
                    }

                    IEnumerable<ForeignKeyAttribute> foreignKeyAttributes =
                        this.RecordReference.RecordType.GetUniquePropertyAttributes<ForeignKeyAttribute>();

                    bool peersResult =
                        foreignKeyAttributes.Any(fka => fka.PrimaryTableType == pkRecord.RecordReference.RecordType);

                    return peersResult;

                }).Cast<InsertRecord>().ToList();

            if (!result.Any())
            {
                throw new NoPeersException();
            }

            InsertRecord.Logger.Debug("Exiting GetPrimaryKeyOperations");

            return result;
        }

        private void WritePrimitives(IWritePrimitives writer, List<Column> columns)
        {
            InsertRecord.Logger.Debug("Entering WritePrimitives");

            writer.Insert(columns);

            if (this.keyType == PrimaryKeyAttribute.KeyTypeEnum.Auto)
            {
                string primaryKeyColumnName = this.GetPrimaryKeyColumnName();
                string identityVariable = writer.SelectIdentity();

                this.primaryKeyValues.Add(new ColumnSymbol
                {
                    ColumnName = primaryKeyColumnName,
                    Value = identityVariable,
                    TableType = this.RecordReference.RecordType
                });
            }
            else if (this.keyType == PrimaryKeyAttribute.KeyTypeEnum.Manual)
            {
                List<ColumnSymbol> primaryKeyValues = Enumerable.ToList<ColumnSymbol>(this.GetPrimaryKeyValues());
                primaryKeyValues.ForEach(v => this.primaryKeyValues.Add(v));
            }

            InsertRecord.Logger.Debug("Exiting WritePrimitives");

            throw new NotImplementedException();
        }

        private IEnumerable<ColumnSymbol> GetPrimaryKeyValues()
        {
            InsertRecord.Logger.Debug("Entering GetPrimaryKeyValues");

            IEnumerable<PropertyAttribute<PrimaryKeyAttribute>> pkPropertyAttributes =
                this.RecordReference.RecordType.GetPropertyAttributes<PrimaryKeyAttribute>();

            IEnumerable<ColumnSymbol> result = pkPropertyAttributes.Select(pa => new ColumnSymbol
            {
                ColumnName = Helper.GetColunName(pa.PropertyInfo),
                TableType = this.RecordReference.RecordType,
                Value = pa.PropertyInfo.GetValue(this.RecordReference.RecordType)
            });

            InsertRecord.Logger.Debug("Exiting GetPrimaryKeyValues");

            return result;
        }

        private string GetPrimaryKeyColumnName()
        {
            InsertRecord.Logger.Debug("Entering GetPrimaryKeyColumnName");

            PropertyAttribute<PrimaryKeyAttribute> pkPropertyAttribute =
                this.RecordReference.RecordType.GetPropertyAttributes<PrimaryKeyAttribute>().First();

            InsertRecord.Logger.Debug("Exiting GetPrimaryKeyColumnName");

            return Helper.GetColunName(pkPropertyAttribute.PropertyInfo);
        }

        #endregion Write related private methods
    }
}
