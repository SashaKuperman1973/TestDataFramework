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
            this.recordReference = recordReference;
        }

        #endregion Constructor

        public virtual IEnumerable<InsertRecord> GetPrimaryKeyOperations(IEnumerable<AbstractRepositoryOperation> peers)
        {
            InsertRecordService.Logger.Debug("Entering GetPrimaryKeyOperations");

            IEnumerable<InsertRecord> result = peers.Where(
                peer =>
                {
                    var pkRecord = peer as InsertRecord;

                    bool peerResult = pkRecord != null
                                  && this.recordReference.PrimaryKeyReferences.Any(
                                      primaryKeyReference => primaryKeyReference == pkRecord.RecordReference);

                    return peerResult;

                }).Cast<InsertRecord>().ToList();

            InsertRecordService.Logger.Debug("Exiting GetPrimaryKeyOperations");

            return result;
        }

        public virtual void WritePrimaryKeyOperations(IWritePrimitives writer, IEnumerable<AbstractRepositoryOperation> primaryKeyOperations,
            CircularReferenceBreaker breaker, Counter order, AbstractRepositoryOperation[] orderedOperations)
        {
            InsertRecordService.Logger.Debug("Entering WritePrimaryKeyOperations");

            primaryKeyOperations.ToList().ForEach(o => o.Write(breaker, writer, order, orderedOperations));

            InsertRecordService.Logger.Debug("Exiting WritePrimaryKeyOperations");
        }

        #region GetColumnData

        public virtual IEnumerable<ExtendedColumnSymbol> GetForeignKeyColumns(IEnumerable<InsertRecord> primaryKeyOperations)
        {
            InsertRecordService.Logger.Debug("Entering GetForeignKeyVariables");

            List<IEnumerable<ColumnSymbol>> keyTableList =
                primaryKeyOperations.Select(o => o.GetPrimaryKeySymbols()).ToList();

            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> foreignKeyPropertyAttributes =
                this.recordReference.RecordType.GetPropertyAttributes<ForeignKeyAttribute>();

            var foreignKeys = foreignKeyPropertyAttributes.Select(fkpa =>
            {
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
                        });

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

                        bool filter = p.GetSingleAttribute<ForeignKeyAttribute>() == null
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
                    );

            InsertRecordService.Logger.Debug("Exiting GetRegularColumns");

            return result.ToList();
        }

        #endregion GetColumnData

        #region WritePrimitives

        public virtual void WritePrimitives(IWritePrimitives writer, string tableName, IEnumerable<Column> columns, List<ColumnSymbol> primaryKeyValues)
        {
            InsertRecordService.Logger.Debug("Entering WritePrimitives");

            columns = columns.ToList();

            writer.Insert(tableName, columns);

            this.PopulatePrimaryKeyValues(writer, primaryKeyValues, columns);

            InsertRecordService.Logger.Debug("Exiting WritePrimitives");
        }

        private void PopulatePrimaryKeyValues(IWritePrimitives writer, List<ColumnSymbol> primaryKeyValues, IEnumerable<Column> columns)
        {
            InsertRecordService.Logger.Debug("Entering PopulatePrimaryKeyValues");

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
            });

            InsertRecordService.Logger.Debug("Exiting PopulatePrimaryKeyValues");

            primaryKeyValues.AddRange(result);
        }

        #endregion WritePrimitives

        public virtual void CopyPrimaryToForeignKeyColumns(IEnumerable<Column> foreignKeyColumns)
        {
            foreignKeyColumns.ToList().ForEach(c =>
            {
                if (c.Value.IsSpecialType())
                {
                    return;
                }

                PropertyInfo targetProperty =

                    this.recordReference.RecordType.GetPropertiesHelper().First(p =>
                        Helper.GetColumnName(p).Equals(c.Name)
                        );

                targetProperty.SetValue(this.recordReference.RecordObject, c.Value);
            });
        }
    }
}
