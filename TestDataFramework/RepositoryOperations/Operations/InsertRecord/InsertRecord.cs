﻿using System;
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
        private IEnumerable<ExtendedColumnSymbol> foreignKeyColumns;
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
            InsertRecord.Logger.Debug("Entering Write:" + this);
            InsertRecord.Logger.Debug($"breaker: {breaker}");

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

            Columns columnData = this.GetColumnData(writer);

            this.Order = order.Value++;
            InsertRecord.Logger.Debug($"this.Order: {this.Order}");
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
            InsertRecord.Logger.Debug($"Entering Read. readStreamPointer.Value: {readStreamPointer}, data: {string.Join(",", data)}");

            IEnumerable<PropertyAttributes> propertyAttributes =
                this.RecordReference.RecordType.GetPropertyAttributes().ToList();

            List<PropertyInfo> propertiesForRead = this.GetPropertiesForRead(propertyAttributes).ToList();

            InsertRecord.Logger.Debug(
                $"propertiesForRead: {string.Join(",", propertiesForRead.Select(p => p.GetExtendedMemberInfoString()))}");

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

            IEnumerable<ExtendedColumnSymbol> specialForeignKeyColumns =
                this.foreignKeyColumns.Where(c => c.Value.IsSpecialType());

            InsertRecord.Logger.Debug("Iterating specialForeignKeyColumns");

            specialForeignKeyColumns.ToList().ForEach(fkColumn =>
            {
                InsertRecord.Logger.Debug($"fkColumn: {fkColumn}");

                InsertRecord primaryKeyOperation = this.primaryKeyOperations.First(selectedPrimaryKeyOperation =>
                    selectedPrimaryKeyOperation.RecordReference.RecordType == fkColumn.PropertyAttribute.Attribute.PrimaryTableType);

                InsertRecord.Logger.Debug($"primaryKeyOperation: {primaryKeyOperation}");

                PropertyInfo primaryKeyProperty =
                    InsertRecord.GetPrimaryKeyProperty(primaryKeyOperation.RecordReference.RecordType,
                        fkColumn.PropertyAttribute.Attribute);

                InsertRecord.Logger.Debug($"primaryKeyProperty : {primaryKeyProperty.GetExtendedMemberInfoString()}");

                fkColumn.PropertyAttribute.PropertyInfo.SetValue(this.RecordReference.RecordObject,
                    primaryKeyProperty.GetValue(primaryKeyOperation.RecordReference.RecordObject));
            });

            InsertRecord.Logger.Debug("Exiting Read");
        }

        public virtual IEnumerable<ColumnSymbol> GetPrimaryKeySymbols()
        {
            InsertRecord.Logger.Debug("Executing GetPrimaryKeySymbols");

            return this.primaryKeyValues;
        }

        #endregion Public methods

        #region Private methods

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

        private static IEnumerable<PropertyAttributes> GetForeignKeyProperties(IEnumerable<PropertyAttributes> propertyAttributes)
        {
            IEnumerable<PropertyAttributes> result =
                propertyAttributes.Where(
                    pa =>
                        pa.Attributes.Any(a => a.GetType() == typeof(ForeignKeyAttribute)));

            return result;
        }

        private Columns GetColumnData(IWritePrimitives writer)
        {
            InsertRecord.Logger.Debug("Entering GetColumnData");

            // ReSharper disable once UseObjectOrCollectionInitializer
            var result = new Columns();

            result.RegularColumns = this.service.GetRegularColumns(writer);

            this.foreignKeyColumns = this.service.GetForeignKeyColumns(this.primaryKeyOperations);

            result.ForeignKeyColumns =
                this.foreignKeyColumns.Select(
                    columnSymbol => new Column {Name = columnSymbol.ColumnName, Value = columnSymbol.Value});

            InsertRecord.Logger.Debug("Exiting GetColumnData");
            return result;
        }

        private IEnumerable<PropertyInfo> GetPropertiesForRead(IEnumerable<PropertyAttributes> propertyAttributes)
        {
            IEnumerable<PropertyAttributes> result = propertyAttributes.Where(pa =>

                !this.RecordReference.IsExplicitlySet(pa.PropertyInfo)

                &&

                (pa.Attributes.Any(
                    a =>

                        (a.GetType() == typeof (PrimaryKeyAttribute) &&
                         ((PrimaryKeyAttribute) a).KeyType == PrimaryKeyAttribute.KeyTypeEnum.Auto)
                    )

                || pa.PropertyInfo.PropertyType.IsGuid() && pa.Attributes.All(a => a.GetType() != typeof (ForeignKeyAttribute)))

                );

            return result.Select(pa => pa.PropertyInfo);
        }

        #endregion Private methods
    }
}
