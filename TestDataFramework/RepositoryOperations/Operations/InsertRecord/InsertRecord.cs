using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;
using PropertyAttributes = TestDataFramework.RepositoryOperations.Model.PropertyAttributes;

namespace TestDataFramework.RepositoryOperations.Operations.InsertRecord
{
    public class InsertRecord : AbstractRepositoryOperation
    {
        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger(typeof(InsertRecord));
        private readonly InsertRecordService service;

        private readonly List<ColumnSymbol> primaryKeyValues = new List<ColumnSymbol>();
        private IEnumerable<InsertRecord> primaryKeyOperations;

        #endregion Private Fields

        public InsertRecord(InsertRecordService service, RecordReference recordReference, IEnumerable<AbstractRepositoryOperation> peers)
        {
            InsertRecord.Logger.Debug("Entering constructor");

            this.Peers = peers;
            this.RecordReference = recordReference;
            this.service = service;

            InsertRecord.Logger.Debug("Exiting constructor");
        }

        #region Public methods

        public override void Write(CircularReferenceBreaker breaker, IWritePrimitives writer, Counter order, AbstractRepositoryOperation[] orderedOperations)
        {
            InsertRecord.Logger.Debug("Entering Write:" + this.DumpObject());

            if (breaker.IsVisited<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.Write))
            {
                InsertRecord.Logger.Debug("Write already visited. Exiting");
                return;
            }

            if (this.IsWriteDone)
            {
                InsertRecord.Logger.Debug("Write already done. Exiting");
                return;
            }

            breaker.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.Write);

            this.primaryKeyOperations = this.service.GetPrimaryKeyOperations(this.Peers).ToList();

            this.service.WritePrimaryKeyOperations(writer, this.primaryKeyOperations, breaker, order, orderedOperations);

            Columns columnData = this.GetColumnData(this.primaryKeyOperations, writer);

            this.Order = order.Value++;
            orderedOperations[this.Order] = this;

            string tableName = Helper.GetTableName(this.RecordReference.RecordType);

            this.service.WritePrimitives(writer, tableName, columnData.AllColumns, this.primaryKeyValues);

            this.service.CopyPrimaryToForeignKeyColumns(columnData.ForeignKeyColumns);

            this.IsWriteDone = true;

            breaker.Pop();

            InsertRecord.Logger.Debug("Exiting Write");
        }

        public override void Read(Counter readStreamPointer, object[] data)
        {
            InsertRecord.Logger.Debug("Entering Read");

            IEnumerable<PropertyAttributes> propertyAttributes =
                this.RecordReference.RecordType.GetPropertyAttributes().ToList();

            List<PropertyInfo> propertiesForRead = this.GetPropertiesForRead(propertyAttributes).ToList();

            while (propertiesForRead.Any())
            {
                var columnName = (string) data[readStreamPointer.Value++];

                PropertyInfo property = propertiesForRead.First(
                    p => Helper.GetColumnName(p).Equals(columnName, StringComparison.Ordinal)
                    );

                property.SetValue(this.RecordReference.RecordObject,
                    data[readStreamPointer.Value] is IConvertible
                        ? Convert.ChangeType(data[readStreamPointer.Value++], property.PropertyType)
                        : data[readStreamPointer.Value++]);

                propertiesForRead.Remove(property);
            }

            IEnumerable<PropertyAttributes> foreignKeyProperties = InsertRecord.GetForeignKeyProperties(propertyAttributes);

            foreignKeyProperties.ToList().ForEach(pa =>
            {
                var foreignKeyAttribute = pa.PropertyInfo.GetSingleAttribute<ForeignKeyAttribute>();

                InsertRecord primaryKeyOperation = this.primaryKeyOperations.First(selectedPrimaryKeyOperation =>
                    selectedPrimaryKeyOperation.RecordReference.RecordType == foreignKeyAttribute.PrimaryTableType);

                PropertyInfo primaryKeyProperty =
                    InsertRecord.GetPrimaryKeyProperty(primaryKeyOperation.RecordReference.RecordType, foreignKeyAttribute);

                pa.PropertyInfo.SetValue(this.RecordReference.RecordObject,
                    primaryKeyProperty.GetValue(primaryKeyOperation.RecordReference.RecordObject));
            });

            InsertRecord.Logger.Debug("Exiting Read");
        }

        private static PropertyInfo GetPrimaryKeyProperty(Type recordType, ForeignKeyAttribute foreignKeyAttribute)
        {
            PropertyInfo result =
                recordType.GetPropertiesHelper()
                    .First(
                        p =>
                            Helper.GetColumnName(p)
                                .Equals(foreignKeyAttribute.PrimaryKeyName, StringComparison.Ordinal));

            return result;
        }

        private static IEnumerable<Model.PropertyAttributes> GetForeignKeyProperties(IEnumerable<Model.PropertyAttributes> propertyAttributes)
        {
            IEnumerable<PropertyAttributes> result =
                propertyAttributes.Where(
                    pa =>
                        pa.Attributes.Any(a => a.GetType() == typeof (ForeignKeyAttribute)));

            return result;
        }

        public virtual IEnumerable<ColumnSymbol> GetPrimaryKeySymbols()
        {
            return this.primaryKeyValues;
        }

        #endregion Public methods

        #region Private methods

        private Columns GetColumnData(IEnumerable<InsertRecord> primaryKeyOperations, IWritePrimitives writer)
        {
            InsertRecord.Logger.Debug("Entering GetColumnData");

            // ReSharper disable once UseObjectOrCollectionInitializer
            var result = new Columns();

            result.RegularColumns = this.service.GetRegularColumns(writer);
            result.ForeignKeyColumns = this.service.GetForeignKeyColumns(primaryKeyOperations);

            InsertRecord.Logger.Debug("Exiting GetColumnData");
            return result;
        }

        private IEnumerable<PropertyInfo> GetPropertiesForRead(IEnumerable<Model.PropertyAttributes> propertyAttributes)
        {
            IEnumerable<Model.PropertyAttributes> result = propertyAttributes.Where(pa =>

                pa.Attributes.Any(
                    a =>

                        (a.GetType() == typeof (PrimaryKeyAttribute) &&
                         ((PrimaryKeyAttribute) a).KeyType == PrimaryKeyAttribute.KeyTypeEnum.Auto)
                    )

                && !this.RecordReference.IsExplicitlySet(pa.PropertyInfo)

                || pa.PropertyInfo.PropertyType.IsGuid() && pa.Attributes.All(a => a.GetType() != typeof (ForeignKeyAttribute))

                );

            return result.Select(pa => pa.PropertyInfo);
        }

        private string DumpObject()
        {
            return Helper.DumpObject(this.RecordReference.RecordObject);
        }

        #endregion Private methods
    }
}
