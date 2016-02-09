using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.RepositoryOperations.Operations.InsertRecord
{
    public class InsertRecordService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InsertRecordService));
        private readonly RecordReference recordReference;

        private PrimaryKeyAttribute.KeyTypeEnum? keyType;
        public PrimaryKeyAttribute.KeyTypeEnum KeyType
        {
            get
            {
                PrimaryKeyAttribute.KeyTypeEnum result = (this.keyType ?? (this.keyType = this.DetermineKeyType())).Value;
                return result;
            }
        }

        private PrimaryKeyAttribute.KeyTypeEnum DetermineKeyType()
        {
            IEnumerable<PrimaryKeyAttribute> pkAttributes =
                this.recordReference.RecordType.GetUniqueAttributes<PrimaryKeyAttribute>().ToList();

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

        #region Constructor

        public InsertRecordService(RecordReference recordReference)
        {
            InsertRecordService.Logger.Debug("Entering constructor");

            this.recordReference = recordReference;

            InsertRecordService.Logger.Debug("Exiting constructor");
        }

        #endregion Constructor

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

        public virtual void WritePrimaryKeyOperations(IWritePrimitives writer, IEnumerable<AbstractRepositoryOperation> primaryKeyOperations,
            CircularReferenceBreaker breaker, Counter order, AbstractRepositoryOperation[] orderedOperations)
        {
            InsertRecordService.Logger.Debug("Entering WritePrimaryKeyOperations");

            primaryKeyOperations = primaryKeyOperations.ToList();

            InsertRecordService.Logger.Debug($"primaryKeyOperations: {primaryKeyOperations.GetRecordTypesString()}");
            InsertRecordService.Logger.Debug($"orderedOperations: {orderedOperations.GetRecordTypesString()}");
            InsertRecordService.Logger.Debug($"order: {order.Value}");

            primaryKeyOperations.ToList().ForEach(o => o.Write(breaker, writer, order, orderedOperations));

            InsertRecordService.Logger.Debug("Exiting WritePrimaryKeyOperations");
        }

        #region GetColumnData

        public virtual IEnumerable<ExtendedColumnSymbol> GetForeignKeyColumns(IEnumerable<InsertRecord> primaryKeyOperations)
        {
            InsertRecordService.Logger.Debug("Entering GetForeignKeyVariables");

            primaryKeyOperations = primaryKeyOperations.ToList();

            InsertRecordService.Logger.Debug(
                $"primaryKeyOperations: {primaryKeyOperations.GetRecordTypesString()}");

            List<IEnumerable<ColumnSymbol>> keyTableList =
                primaryKeyOperations.Select(o => o.GetPrimaryKeySymbols()).ToList();

            InsertRecordService.Logger.Debug($"keyTableList: {Helper.ToCompositeString(keyTableList.Select(kt => string.Join(", ", kt)))}");

            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> foreignKeyPropertyAttributes =
                this.recordReference.RecordType.GetPropertyAttributes<ForeignKeyAttribute>();

            var foreignKeys = foreignKeyPropertyAttributes.Select(fkpa =>
            {
                InsertRecordService.Logger.Debug($"fkpa (foreign Key Property Arttribute) : {fkpa}");

                ColumnSymbol pkColumnMatch = null;

                bool isForeignKeyPrimaryKeyMatch = keyTableList.Any(pkTable =>

                    pkTable.Any(pk =>

                        fkpa.Attribute.PrimaryTableType == (pkColumnMatch = pk).TableType
                        && fkpa.Attribute.PrimaryKeyName.Equals(pk.ColumnName, StringComparison.Ordinal)
                        )
                    );

                return
                    new
                    {
                        PkColumnValue =

                            this.recordReference.IsExplicitlySet(fkpa.PropertyInfo)

                                ? fkpa.PropertyInfo.GetValue(this.recordReference.RecordObject)

                                : isForeignKeyPrimaryKeyMatch

                                    ? pkColumnMatch.Value

                                    : Helper.GetDefaultValue(fkpa.PropertyInfo.PropertyType),

                        FkPropertyAttribute = fkpa
                    };
            });

            IEnumerable<ExtendedColumnSymbol> result =
                foreignKeys.Select(
                    fk =>
                        new ExtendedColumnSymbol
                        {
                            TableType = fk.FkPropertyAttribute.PropertyInfo.DeclaringType,
                            ColumnName = Helper.GetColumnName(fk.FkPropertyAttribute.PropertyInfo),
                            Value = fk.PkColumnValue,
                            PropertyAttribute = fk.FkPropertyAttribute,
                        }).ToList();

            InsertRecordService.Logger.Debug($"result: {Helper.ToCompositeString(result)}");
            return result;
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

                            p.GetSingleAttribute<ForeignKeyAttribute>() == null

                            &&

                            ((pka = p.GetSingleAttribute<PrimaryKeyAttribute>()) == null ||
                             pka.KeyType != PrimaryKeyAttribute.KeyTypeEnum.Auto);

                        return filter;
                    })
                    .Select(
                        p =>
                        {
                            string columnName = Helper.GetColumnName(p);

                            var column = new Column
                            {
                                Name = columnName,

                                Value =

                                    p.PropertyType.IsGuid() && !this.recordReference.IsExplicitlySet(p)

                                        ? writer.WriteGuid(columnName)

                                        : p.GetValue(this.recordReference.RecordObject)
                            };

                            return column;
                        }
                    ).ToList();

            InsertRecordService.Logger.Debug($"Exiting GetRegularColumns. result: {Helper.ToCompositeString(result)}");

            return result;
        }

        #endregion GetColumnData

        #region WritePrimitives

        public virtual void WritePrimitives(IWritePrimitives writer, string tableName, IEnumerable<Column> columns, List<ColumnSymbol> primaryKeyValues)
        {
            InsertRecordService.Logger.Debug("Entering WritePrimitives");

            columns = columns.ToList();

            InsertRecordService.Logger.Debug($"tableName: {tableName}, columns: {Helper.ToCompositeString(columns)}");

            writer.Insert(tableName, columns);

            this.PopulatePrimaryKeyValues(writer, primaryKeyValues, columns);

            InsertRecordService.Logger.Debug("Exiting WritePrimitives");
        }

        private void PopulatePrimaryKeyValues(IWritePrimitives writer, List<ColumnSymbol> primaryKeyValues, IEnumerable<Column> columns)
        {
            InsertRecordService.Logger.Debug("Entering PopulatePrimaryKeyValues");

            columns = columns.ToList();

            InsertRecordService.Logger.Debug($"columns: {Helper.ToCompositeString(columns)}");

            IEnumerable<PropertyAttribute<PrimaryKeyAttribute>> pkPropertyAttributes =
                this.recordReference.RecordType.GetPropertyAttributes<PrimaryKeyAttribute>();

            IEnumerable<ColumnSymbol> result = pkPropertyAttributes.Select(pa =>
            {
                string columnName = Helper.GetColumnName(pa.PropertyInfo);

                Column sourceColumn = columns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.Ordinal));

                if (sourceColumn == null)
                {
                    if (pa.Attribute.KeyType != PrimaryKeyAttribute.KeyTypeEnum.Auto)
                    {
                        throw new PopulatePrimaryKeyException(Messages.ColumnNotInInputList, pa.PropertyInfo);
                    }

                    if (
                        !new[]
                        {typeof (int), typeof (short), typeof (long), typeof (uint), typeof (ushort), typeof (ulong),}
                            .Contains(pa.PropertyInfo.PropertyType.GetUnderLyingType()))
                    {
                        throw new PopulatePrimaryKeyException(Messages.AutoKeyMustBeInteger, pa.PropertyInfo);
                    }

                    sourceColumn = new Column {Name = columnName, Value = writer.SelectIdentity(columnName)};
                }

                var symbol = new ColumnSymbol
                {
                    ColumnName = sourceColumn.Name,

                    TableType = this.recordReference.RecordType,

                    Value = sourceColumn.Value,
                };

                return symbol;
            }).ToList();

            InsertRecordService.Logger.Debug($"Exiting PopulatePrimaryKeyValues. result: {Helper.ToCompositeString(result)}");

            primaryKeyValues.AddRange(result);
        }

        #endregion WritePrimitives

        public virtual void CopyPrimaryToForeignKeyColumns(IEnumerable<Column> foreignKeyColumns)
        {
            InsertRecordService.Logger.Debug("Entering PopulatePrimaryKeyValues");

            foreignKeyColumns.ToList().ForEach(c =>
            {
                InsertRecordService.Logger.Debug($"foreignKeyColumn: {c}");

                if (c.Value.IsSpecialType())
                {
                    return;
                }

                PropertyInfo targetProperty =

                    this.recordReference.RecordType.GetPropertiesHelper().First(p =>
                        Helper.GetColumnName(p).Equals(c.Name)
                        );

                InsertRecordService.Logger.Debug($"targetProperty: {targetProperty.GetExtendedMemberInfoString()}");

                targetProperty.SetValue(this.recordReference.RecordObject, c.Value);
            });

            InsertRecordService.Logger.Debug("Exiting PopulatePrimaryKeyValues");
        }
    }
}
