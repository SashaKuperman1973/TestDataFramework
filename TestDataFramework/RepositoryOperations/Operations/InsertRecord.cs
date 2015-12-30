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

            this.Peers = peers;
            this.RecordReference = recordReference;

            this.keyType = this.DetermineKeyType();

            InsertRecord.Logger.Debug("Exiting constructor");
        }

        private PrimaryKeyAttribute.KeyTypeEnum DetermineKeyType()
        {
            IEnumerable<PrimaryKeyAttribute> pkAttributes =
                this.RecordReference.RecordType.GetUniqueAttributes<PrimaryKeyAttribute>().ToList();

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

            IEnumerable<InsertRecord> primaryKeyOperations = this.GetPrimaryKeyOperations().ToList();

            InsertRecord.WritePrimaryKeyOperations(writer, primaryKeyOperations, breaker, currentOrder, orderedOperations);

            Columns columnData = this.GetColumnData(primaryKeyOperations);

            this.Order = currentOrder.Value++;
            orderedOperations[this.Order] = this;

            this.WritePrimitives(writer, columnData.AllColumns);

            this.CopyForeignKeyColumns(columnData.ForeignKeyColumns);

            this.IsWriteDone = true;

            breaker.Pop();

            InsertRecord.Logger.Debug("Exiting Write");
        }

        private void CopyForeignKeyColumns(IEnumerable<Column> foreignKeyColumns)
        {
            foreignKeyColumns.ToList().ForEach(c =>
            {
                if (c.Value.IsSpecialType())
                {
                    return;
                }

                PropertyInfo targetProperty =
                    this.RecordReference.RecordType.GetProperties().First(p => Helper.GetColunName(p).Equals(c.Name));

                targetProperty.SetValue(this.RecordReference.RecordObject, c.Value);
            });
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

        private Columns GetColumnData(IEnumerable<InsertRecord> primaryKeyOperations)
        {
            InsertRecord.Logger.Debug("Entering GetColumnData");

            // ReSharper disable once UseObjectOrCollectionInitializer
            var result = new Columns();

            result.RegularColumns = this.GetRegularColumns();
            result.ForeignKeyColumns = this.GetForeignKeyColumns(primaryKeyOperations);

            InsertRecord.Logger.Debug("Exiting GetColumnData");
            return result;
        }

        private IEnumerable<Column> GetForeignKeyColumns(IEnumerable<InsertRecord> primaryKeyOperations)
        {
            InsertRecord.Logger.Debug("Entering GetForeignKeyVariables");

            IEnumerable<ColumnSymbol> symbolList = primaryKeyOperations.SelectMany(o => o.GetPrimaryKeySymbols());

            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> propertyAttributes = this.RecordReference.RecordType.GetPropertyAttributes<ForeignKeyAttribute>();

            IEnumerable<Column> result = symbolList.Select(sl =>
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

            });

            InsertRecord.Logger.Debug("Exiting GetForeignKeyVariables");

            return result;
        }

        private IEnumerable<Column> GetRegularColumns()
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

        private static void WritePrimaryKeyOperations(IWritePrimitives writer, IEnumerable<InsertRecord> primaryKeyOperations,
            CircularReferenceBreaker breaker, CurrentOrder currentOrder, AbstractRepositoryOperation[] orderedOperations)
        {
            InsertRecord.Logger.Debug("Entering WriteHigherPriorityOperations");

            primaryKeyOperations.ToList().ForEach(o => o.Write(breaker, writer, currentOrder, orderedOperations));

            InsertRecord.Logger.Debug("Exiting WriteHigherPriorityOperations");
        }

        private IEnumerable<InsertRecord> GetPrimaryKeyOperations()
        {
            InsertRecord.Logger.Debug("Entering GetPrimaryKeyOperations");

            IEnumerable<InsertRecord> result = this.Peers.Where(
                peer =>
                {
                    var pkRecord = peer as InsertRecord;

                    bool peerResult = pkRecord != null & pkRecord != this 
                                  && this.RecordReference.PrimaryKeyReferences.Any(
                                      primaryKeyReference => primaryKeyReference == pkRecord.RecordReference);

                    return peerResult;

                }).Cast<InsertRecord>().ToList();

            InsertRecord.Logger.Debug("Exiting GetPrimaryKeyOperations");

            return result;
        }

        private void WritePrimitives(IWritePrimitives writer, IEnumerable<Column> columns)
        {
            InsertRecord.Logger.Debug("Entering WritePrimitives");

            writer.Insert(columns);
            this.HandlePrimaryKeyValues(writer);

            InsertRecord.Logger.Debug("Exiting WritePrimitives");
        }

        private void HandlePrimaryKeyValues(IWritePrimitives writer)
        {
            InsertRecord.Logger.Debug("Entering HandlePrimaryKeyValues");

            switch (this.keyType)
            {
                case PrimaryKeyAttribute.KeyTypeEnum.Auto:
                    InsertRecord.Logger.Debug("Taking KeyTypeEnum.Auto branch");

                    string primaryKeyColumnName = this.GetPrimaryKeyColumnName();
                    object identityVariable = writer.SelectIdentity();

                    this.primaryKeyValues.Add(new ColumnSymbol
                    {
                        ColumnName = primaryKeyColumnName,
                        Value = identityVariable,
                        TableType = this.RecordReference.RecordType
                    });

                    break;

                case PrimaryKeyAttribute.KeyTypeEnum.Manual:
                    InsertRecord.Logger.Debug("Taking KeyTypeEnum.Auto Manual branch");

                    this.primaryKeyValues.AddRange(this.GetPrimaryKeyValues());

                    break;

                default:
                    InsertRecord.Logger.Debug("No special key type handling fo this branch");
                    break;
            }

            InsertRecord.Logger.Debug("Exiting HandlePrimaryKeyValues");
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
                Value = pa.PropertyInfo.GetValue(this.RecordReference.RecordObject)
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
