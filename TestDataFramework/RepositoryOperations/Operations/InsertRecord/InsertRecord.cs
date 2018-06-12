/*
    Copyright 2016, 2017 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives.Interfaces;
using PropertyAttributes = TestDataFramework.RepositoryOperations.Model.PropertyAttributes;

namespace TestDataFramework.RepositoryOperations.Operations.InsertRecord
{
    public class InsertRecord : AbstractRepositoryOperation
    {
        public InsertRecord(InsertRecordService service, RecordReference recordReference,
            IEnumerable<AbstractRepositoryOperation> peers, IAttributeDecorator attributeDecorator)
        {
            InsertRecord.Logger.Debug("Entering constructor");

            this.Peers = peers;
            this.RecordReference = recordReference;
            this.service = service;
            this.attributeDecorator = attributeDecorator;

            InsertRecord.Logger.Debug("Exiting constructor");
        }

        #region Private Fields

        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(InsertRecord));

        private readonly InsertRecordService service;
        private readonly IAttributeDecorator attributeDecorator;

        private readonly List<ColumnSymbol> primaryKeyValues = new List<ColumnSymbol>();
        private IEnumerable<ExtendedColumnSymbol> foreignKeyColumns;
        private IEnumerable<InsertRecord> primaryKeyOperations;

        #endregion Private Fields

        #region Public methods

        public override void Write(CircularReferenceBreaker breaker, IWritePrimitives writer, Counter order,
            AbstractRepositoryOperation[] orderedOperations)
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

            this.service.WritePrimaryKeyOperations(writer, this.primaryKeyOperations, breaker, order,
                orderedOperations);

            Columns columnData = this.GetColumnData(writer);

            this.Order = order.Value++;
            InsertRecord.Logger.Debug($"this.Order: {this.Order}");
            orderedOperations[this.Order] = this;

            TableName tableName = Helper.GetTableName(this.RecordReference.RecordType, this.attributeDecorator);

            this.service.WritePrimitives(writer, tableName.CatalogueName, tableName.Schema, tableName.Name,
                columnData.AllColumns, this.primaryKeyValues);

            this.service.CopyPrimaryToForeignKeyColumns(columnData.ForeignKeyColumns);

            this.IsWriteDone = true;

            breaker.Pop();

            InsertRecord.Logger.Debug("Exiting Write");
        }

        public override void Read(Counter readStreamPointer, object[] data)
        {
            InsertRecord.Logger.Debug(
                $"Entering Read. readStreamPointer.Value: {readStreamPointer.Value}, data: {string.Join(",", data)}");

            IEnumerable<PropertyAttributes> propertyAttributes =
                this.attributeDecorator.GetPropertyAttributes(this.RecordReference.RecordType).ToList();

            List<PropertyInfo> propertiesForRead = this.GetPropertiesForRead(propertyAttributes).ToList();

            InsertRecord.Logger.Debug(
                $"propertiesForRead: {string.Join(",", propertiesForRead.Select(p => p.GetExtendedMemberInfoString()))}");

            while (propertiesForRead.Any())
            {
                var columnName = (string) data[readStreamPointer.Value++];

                PropertyInfo property = propertiesForRead.First(
                    p => Helper.GetColumnName(p, this.attributeDecorator).Equals(columnName, StringComparison.Ordinal)
                );

                try
                {
                    property.SetValue(this.RecordReference.RecordObject,
                        data[readStreamPointer.Value] is IConvertible
                            ? Convert.ChangeType(data[readStreamPointer.Value++], property.PropertyType)
                            : data[readStreamPointer.Value++]);
                }
                catch (OverflowException overflowException)
                {
                    throw new OverflowException(
                        string.Format(Messages.TypeTooNarrow, property.GetExtendedMemberInfoString(),
                            data[readStreamPointer.Value - 1]), overflowException);
                }

                propertiesForRead.Remove(property);
            }

            IEnumerable<ExtendedColumnSymbol> specialForeignKeyColumns =
                this.foreignKeyColumns.Where(c => c.Value.IsSpecialType());

            InsertRecord.Logger.Debug("Iterating specialForeignKeyColumns");

            specialForeignKeyColumns.ToList().ForEach(fkColumn =>
            {
                InsertRecord.Logger.Debug($"fkColumn: {fkColumn}");

                InsertRecord primaryKeyOperation = this.primaryKeyOperations.First(selectedPrimaryKeyOperation =>
                    selectedPrimaryKeyOperation.RecordReference.RecordType ==
                    this.attributeDecorator.GetTableType(fkColumn.PropertyAttribute.Attribute,
                        new TypeInfoWrapper(fkColumn.TableType.GetTypeInfo())));

                InsertRecord.Logger.Debug($"primaryKeyOperation: {primaryKeyOperation}");

                PropertyInfo primaryKeyProperty =
                    this.GetPrimaryKeyProperty(primaryKeyOperation.RecordReference.RecordType,
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

        private PropertyInfo GetPrimaryKeyProperty(Type recordType, ForeignKeyAttribute foreignKeyAttribute)
        {
            PropertyInfo result =
                recordType.GetPropertiesHelper()
                    .First(
                        p =>
                            Helper.GetColumnName(p, this.attributeDecorator)
                                .Equals(foreignKeyAttribute.PrimaryKeyName, StringComparison.Ordinal));

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
            InsertRecord.Logger.Debug("Entering GetPropertiesForRead");

            IEnumerable<PropertyAttributes> result = propertyAttributes.Where(pa =>
                !this.RecordReference.IsExplicitlySet(pa.PropertyInfo)
                &&
                (pa.Attributes.Any(
                     a =>
                         a.GetType() == typeof(PrimaryKeyAttribute) &&
                         ((PrimaryKeyAttribute) a).KeyType == PrimaryKeyAttribute.KeyTypeEnum.Auto
                 )
                 || pa.PropertyInfo.PropertyType.IsGuid() &&
                 pa.Attributes.All(a => a.GetType() != typeof(ForeignKeyAttribute)))
            );

            InsertRecord.Logger.Debug("Exiting GetPropertiesForRead");
            return result.Select(pa => pa.PropertyInfo);
        }

        #endregion Private methods
    }
}