/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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

namespace TestDataFramework.RepositoryOperations.Operations.InsertRecord
{
    public class InsertRecordService
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(InsertRecordService));
        private readonly IAttributeDecorator attributeDecorator;
        private readonly bool enforceKeyReferenceCheck;

        private readonly RecordReference recordReference;

        private PrimaryKeyAttribute.KeyTypeEnum? keyType;

        #region Constructor

        public InsertRecordService(RecordReference recordReference, IAttributeDecorator attributeDecorator,
            bool enforceKeyReferenceCheck)
        {
            InsertRecordService.Logger.Debug("Entering constructor");

            this.recordReference = recordReference;
            this.attributeDecorator = attributeDecorator;
            this.enforceKeyReferenceCheck = enforceKeyReferenceCheck;

            InsertRecordService.Logger.Debug("Exiting constructor");
        }

        #endregion Constructor

        public PrimaryKeyAttribute.KeyTypeEnum KeyType
        {
            get
            {
                PrimaryKeyAttribute.KeyTypeEnum result =
                    (this.keyType ?? (this.keyType = this.DetermineKeyType())).Value;
                return result;
            }
        }

        private PrimaryKeyAttribute.KeyTypeEnum DetermineKeyType()
        {
            IEnumerable<PrimaryKeyAttribute> pkAttributes =
                this.attributeDecorator.GetUniqueAttributes<PrimaryKeyAttribute>(this.recordReference.RecordType)
                    .ToList();

            if (!pkAttributes.Any())
                return PrimaryKeyAttribute.KeyTypeEnum.None;

            if (pkAttributes.Count() > 1)
                return PrimaryKeyAttribute.KeyTypeEnum.Manual;

            PrimaryKeyAttribute.KeyTypeEnum result = pkAttributes.First().KeyType;
            return result;
        }

        public virtual IEnumerable<InsertRecord> GetPrimaryKeyOperations(IEnumerable<AbstractRepositoryOperation> peers)
        {
            InsertRecordService.Logger.Debug("Entering GetPrimaryKeyOperations");

            peers = peers.ToList();

            InsertRecordService.Logger.Debug($"peer objects: {peers.GetRecordTypesString()}");

            IEnumerable<InsertRecord> result = peers.Where(
                peer =>
                {
                    var pkRecord = peer as InsertRecord;

                    bool peerResult = pkRecord != null
                                      && this.recordReference.PrimaryKeyReferences.Any(
                                          primaryKeyReference => primaryKeyReference == pkRecord.RecordReference);

                    return peerResult;
                }).Cast<InsertRecord>().ToList();

            InsertRecordService.Logger.Debug(
                $"Exiting GetPrimaryKeyOperations. result: {result.GetRecordTypesString()}");

            return result;
        }

        public virtual void WritePrimaryKeyOperations(IWritePrimitives writer,
            IEnumerable<AbstractRepositoryOperation> primaryKeyOperations,
            CircularReferenceBreaker breaker, Counter order, AbstractRepositoryOperation[] orderedOperations)
        {
            InsertRecordService.Logger.Debug("Entering WritePrimaryKeyOperations");

            primaryKeyOperations = primaryKeyOperations.ToList();

            InsertRecordService.Logger.Debug($"primaryKeyOperations: {primaryKeyOperations.GetRecordTypesString()}");
            InsertRecordService.Logger.Debug($"order: {order.Value}");

            InsertRecordService.Logger.Debug($"orderedOperations: {orderedOperations.GetRecordTypesString()}");

            primaryKeyOperations.ToList().ForEach(o => o.Write(breaker, writer, order, orderedOperations));

            InsertRecordService.Logger.Debug("Exiting WritePrimaryKeyOperations");
        }

        public virtual void CopyPrimaryToForeignKeyColumns(IEnumerable<Column> foreignKeyColumns)
        {
            InsertRecordService.Logger.Debug("Entering PopulatePrimaryKeyValues");

            foreignKeyColumns.ToList().ForEach(c =>
            {
                InsertRecordService.Logger.Debug($"foreignKeyColumn: {c}");

                if (c.Value.IsSpecialType())
                    return;

                PropertyInfo targetProperty =
                    this.recordReference.RecordType.GetPropertiesHelper().First(p =>
                        Helper.GetColumnName(p, this.attributeDecorator).Equals(c.Name)
                    );

                InsertRecordService.Logger.Debug($"targetProperty: {targetProperty.GetExtendedMemberInfoString()}");

                targetProperty.SetValue(this.recordReference.RecordObjectBase, c.Value);
            });

            InsertRecordService.Logger.Debug("Exiting PopulatePrimaryKeyValues");
        }

        #region GetColumnData

        private struct PrimaryKeyColumn
        {
            public ColumnSymbol Column { get; set; }
            public bool HasBeenMatched { get; set; }
        }

        public virtual IEnumerable<ExtendedColumnSymbol> GetForeignKeyColumns(
            IEnumerable<InsertRecord> primaryKeyOperations)
        {
            InsertRecordService.Logger.Debug("Entering GetForeignKeyVariables");

            primaryKeyOperations = primaryKeyOperations.ToList();

            InsertRecordService.Logger.Debug(
                $"primaryKeyOperations: {primaryKeyOperations.GetRecordTypesString()}");

            var keyTableList =
                primaryKeyOperations.Select(o => new
                {
                    Columns = o.GetPrimaryKeySymbols()
                        .Select(pkColumn => new PrimaryKeyColumn { Column = pkColumn, HasBeenMatched = false}),

                    RecordReference = o.RecordReference
                }).ToList();

            InsertRecordService.Logger.Debug(
                $"keyTableList: {Helper.ToCompositeString(keyTableList.Select(kt => string.Join(", ", kt)))}");

            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> foreignKeyPropertyAttributes =
                this.attributeDecorator.GetPropertyAttributes<ForeignKeyAttribute>(this.recordReference.RecordType);

            var foreignKeys = foreignKeyPropertyAttributes.Select(fkpa =>
            {
                InsertRecordService.Logger.Debug($"fkpa (foreign Key Property Arttribute) : {fkpa}");

                PrimaryKeyColumn pkColumnMatch = new PrimaryKeyColumn();

                bool isExplicitlySet = this.recordReference.IsExplicitlySet(fkpa.PropertyInfo);

                if (isExplicitlySet)
                {
                    pkColumnMatch.Column =
                        new ColumnSymbol {Value = fkpa.PropertyInfo.GetValue(this.recordReference.RecordObjectBase)};
                    pkColumnMatch.HasBeenMatched = true;

                    return
                        new
                        {
                            PkColumn = pkColumnMatch,
                            FkPropertyAttribute = fkpa
                        };
                }

                bool isForeignKeyPrimaryKeyMatch = keyTableList.Any(pkTable =>

                    pkTable.Columns.Any(pkColumn =>
                        fkpa.Attribute.PrimaryKeyName.Equals((pkColumnMatch = pkColumn).Column.ColumnName,
                            StringComparison.Ordinal)
                    )

                    &&

                    Helper.IsForeignToPrimaryKeyMatch(
                        this.recordReference,
                        fkpa,
                        pkTable.RecordReference,
                        this.attributeDecorator
                    )
                );

                if (this.enforceKeyReferenceCheck && !isForeignKeyPrimaryKeyMatch)
                {
                    InsertRecordService.ThrowReferentialIntegrityException(fkpa);
                }

                if (isForeignKeyPrimaryKeyMatch)
                {
                    pkColumnMatch.HasBeenMatched = true;

                    return new
                    {
                        PkColumn = pkColumnMatch,
                        FkPropertyAttribute = fkpa
                    };
                }

                return
                    new
                    {
                        PkColumn = new PrimaryKeyColumn
                        {
                            HasBeenMatched = false,
                            Column = new ColumnSymbol {Value = Helper.GetDefaultValue(fkpa.PropertyInfo.PropertyType)}
                        },

                        FkPropertyAttribute = fkpa
                    };
            }).ToList();

            PropertyAttribute<ForeignKeyAttribute> enforcedFkpa = null;

            if (this.enforceKeyReferenceCheck && foreignKeys.Any(fk =>
            {
                enforcedFkpa = fk.FkPropertyAttribute;
                return !fk.PkColumn.HasBeenMatched;
            }))
            {
                InsertRecordService.ThrowReferentialIntegrityException(enforcedFkpa);
            }

            IEnumerable<ExtendedColumnSymbol> result =
                foreignKeys.Select(
                    fk =>
                        new ExtendedColumnSymbol
                        {
                            TableType = fk.FkPropertyAttribute.PropertyInfo.DeclaringType,
                            ColumnName =
                                Helper.GetColumnName(fk.FkPropertyAttribute.PropertyInfo, this.attributeDecorator),
                            Value = fk.PkColumn.Column.Value,
                            PropertyAttribute = fk.FkPropertyAttribute
                        }).ToList();

            InsertRecordService.Logger.Debug(
                $"Entering GetForeignKeyVariables. Result: {Helper.ToCompositeString(result)}");
            return result;
        }

        private static void ThrowReferentialIntegrityException(PropertyAttribute<ForeignKeyAttribute> fkpa)
        {
            InsertRecordService.Logger.Debug(
                $"Key reference check branch taken. Referential integrity check failed. Foreign Key PropertyAttribute : {fkpa}");

            throw new InserRecordServiceException(Messages.ForeignKeyRecordWithNoPrimaryKeyRecord,
                fkpa.PropertyInfo.DeclaringType.FullName, fkpa.PropertyInfo.Name);
        }

        public virtual IEnumerable<Column> GetRegularColumns(IWritePrimitives writer)
        {
            InsertRecordService.Logger.Debug("Entering GetRegularColumns");

            IEnumerable<Column> result =
                this.recordReference.RecordType.GetPropertiesHelper()
                    .Where(p =>
                    {
                        PrimaryKeyAttribute pka;

                        bool filter =
                            this.attributeDecorator.GetSingleAttribute<ForeignKeyAttribute>(p) == null
                            &&
                            ((pka = this.attributeDecorator.GetSingleAttribute<PrimaryKeyAttribute>(p)) == null ||
                             pka.KeyType != PrimaryKeyAttribute.KeyTypeEnum.Auto);

                        return filter;
                    })
                    .Select(
                        p =>
                        {
                            string columnName = Helper.GetColumnName(p, this.attributeDecorator);

                            var column = new Column
                            {
                                Name = columnName,

                                Value =
                                    p.PropertyType.IsGuid() && !this.recordReference.IsExplicitlySet(p)
                                        ? writer.WriteGuid(columnName)
                                        : p.GetValue(this.recordReference.RecordObjectBase)
                            };

                            return column;
                        }
                    ).ToList();

            InsertRecordService.Logger.Debug($"Exiting GetRegularColumns. result: {Helper.ToCompositeString(result)}");

            return result;
        }

        #endregion GetColumnData

        #region WritePrimitives

        public virtual void WritePrimitives(IWritePrimitives writer, string catalogueName, string schema,
            string tableName, IEnumerable<Column> columns, List<ColumnSymbol> primaryKeyValues)
        {
            InsertRecordService.Logger.Debug("Entering WritePrimitives");

            columns = columns.ToList();

            InsertRecordService.Logger.Debug($"tableName: {tableName}, columns: {Helper.ToCompositeString(columns)}");

            writer.Insert(catalogueName, schema, tableName, columns);

            this.PopulatePrimaryKeyValues(writer, primaryKeyValues, columns);

            InsertRecordService.Logger.Debug("Exiting WritePrimitives");
        }

        private void PopulatePrimaryKeyValues(IWritePrimitives writer, List<ColumnSymbol> primaryKeyValues,
            IEnumerable<Column> columns)
        {
            InsertRecordService.Logger.Debug("Entering PopulatePrimaryKeyValues");

            columns = columns.ToList();

            InsertRecordService.Logger.Debug($"columns: {Helper.ToCompositeString(columns)}");

            IEnumerable<PropertyAttribute<PrimaryKeyAttribute>> pkPropertyAttributes =
                this.attributeDecorator.GetPropertyAttributes<PrimaryKeyAttribute>(this.recordReference.RecordType);

            IEnumerable<ColumnSymbol> result = pkPropertyAttributes.Select(pa =>
            {
                string columnName = Helper.GetColumnName(pa.PropertyInfo, this.attributeDecorator);

                Column sourceColumn = columns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.Ordinal));

                if (sourceColumn == null)
                {
                    if (pa.Attribute.KeyType != PrimaryKeyAttribute.KeyTypeEnum.Auto)
                        throw new PopulatePrimaryKeyException(Messages.ColumnNotInInputList, pa.PropertyInfo);

                    if (
                        !new[]
                                {typeof(int), typeof(short), typeof(long), typeof(uint), typeof(ushort), typeof(ulong)}
                            .Contains(pa.PropertyInfo.PropertyType.GetUnderLyingType()))
                        throw new PopulatePrimaryKeyException(Messages.AutoKeyMustBeInteger, pa.PropertyInfo);

                    sourceColumn = new Column {Name = columnName, Value = writer.SelectIdentity(columnName)};
                }

                var symbol = new ColumnSymbol
                {
                    ColumnName = sourceColumn.Name,

                    TableType = this.recordReference.RecordType,

                    Value = sourceColumn.Value
                };

                return symbol;
            }).ToList();

            InsertRecordService.Logger.Debug(
                $"Exiting PopulatePrimaryKeyValues. result: {Helper.ToCompositeString(result)}");

            primaryKeyValues.AddRange(result);
        }

        #endregion WritePrimitives
    }
}