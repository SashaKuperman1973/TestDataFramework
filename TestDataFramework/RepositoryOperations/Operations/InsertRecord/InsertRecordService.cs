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

        public virtual Columns GetColumnData(IEnumerable<InsertRecord> primaryKeyOperations, IWritePrimitives writer)
        {
            InsertRecordService.Logger.Debug("Entering GetColumnData");

            // ReSharper disable once UseObjectOrCollectionInitializer
            var result = new Columns();

            result.RegularColumns = this.GetRegularColumns(writer);
            result.ForeignKeyColumns = this.GetForeignKeyColumns(primaryKeyOperations);

            InsertRecordService.Logger.Debug("Exiting GetColumnData");
            return result;
        }

        private IEnumerable<Column> GetForeignKeyColumns(IEnumerable<InsertRecord> primaryKeyOperations)
        {
            InsertRecordService.Logger.Debug("Entering GetForeignKeyVariables");

            List<IEnumerable<ColumnSymbol>> keyTableList = primaryKeyOperations.Select(o => o.GetPrimaryKeySymbols()).ToList();

            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> foreignKeyPropertyAttributes = this.recordReference.RecordType.GetPropertyAttributes<ForeignKeyAttribute>();

            var foreignKeys = foreignKeyPropertyAttributes.Select(fk =>
            {
                ColumnSymbol pkColumnMatch = null;

                bool isForeignKeyPrimaryKeyMatch = keyTableList.Any(pkTable =>

                    pkTable.Any(pk =>

                        fk.Attribute.PrimaryTableType == (pkColumnMatch = pk).TableType
                        && fk.Attribute.PrimaryKeyName.Equals(pk.ColumnName, StringComparison.Ordinal)
                        )
                    );

                return new {PkColumnValue = isForeignKeyPrimaryKeyMatch ? pkColumnMatch.Value : null, FkPropertyAttribute = fk};
            });

            IEnumerable<Column> result =
                foreignKeys.Select(
                    fk =>
                        new Column
                        {
                            Name = Helper.GetColunName(fk.FkPropertyAttribute.PropertyInfo),
                            Value = fk.PkColumnValue
                        });

            return result;
        }

        private IEnumerable<Column> GetRegularColumns(IWritePrimitives writer)
        {
            InsertRecordService.Logger.Debug("Entering GetRegularColumns");

            IEnumerable<Column> result =
                this.recordReference.RecordType.GetPropertiesHelper()
                    .Where(
                        p =>
                            p.GetSingleAttribute<ForeignKeyAttribute>() == null &&

                            (p.GetSingleAttribute<PrimaryKeyAttribute>() == null
                             || 
                             this.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Manual
                             && !p.PropertyType.IsGuid())
                    )
                    .Select(
                        p =>
                        {
                            string columnName = Helper.GetColunName(p);

                            var column = new Column
                            {
                                Name = columnName,
                                Value = this.recordReference.RecordType.IsGuid() 
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

            writer.Insert(tableName, columns);
            this.PopulatePrimaryKeyValues(writer, primaryKeyValues);

            InsertRecordService.Logger.Debug("Exiting WritePrimitives");
        }

        private void PopulatePrimaryKeyValues(IWritePrimitives writer, List<ColumnSymbol> primaryKeyValues)
        {
            InsertRecordService.Logger.Debug("Entering HandlePrimaryKeyValues");

            switch (this.KeyType)
            {
                case PrimaryKeyAttribute.KeyTypeEnum.Auto:

                    InsertRecordService.Logger.Debug("Taking KeyTypeEnum.Auto branch");

                    string primaryKeyColumnName = this.GetPrimaryKeyColumnName();
                    object identityVariable = writer.SelectIdentity();

                    primaryKeyValues.Add(new ColumnSymbol
                    {
                        ColumnName = primaryKeyColumnName,
                        Value = identityVariable,
                        TableType = this.recordReference.RecordType
                    });

                    break;

                case PrimaryKeyAttribute.KeyTypeEnum.Manual:

                    InsertRecordService.Logger.Debug("Taking KeyTypeEnum.Auto Manual branch");

                    primaryKeyValues.AddRange(this.GetPrimaryKeyValues(writer));

                    break;

                default:

                    InsertRecordService.Logger.Debug("No special key type handling fo this branch");
                    break;
            }

            InsertRecordService.Logger.Debug("Exiting HandlePrimaryKeyValues");
        }

        private IEnumerable<ColumnSymbol> GetPrimaryKeyValues(IWritePrimitives writer)
        {
            InsertRecordService.Logger.Debug("Entering GetPrimaryKeyValues");

            IEnumerable<PropertyAttribute<PrimaryKeyAttribute>> pkPropertyAttributes =
                this.recordReference.RecordType.GetPropertyAttributes<PrimaryKeyAttribute>();

            IEnumerable<ColumnSymbol> result = pkPropertyAttributes.Select(pa =>
            {
                string columnName = Helper.GetColunName(pa.PropertyInfo);

                var symbol = new ColumnSymbol
                {
                    ColumnName = columnName,
                    TableType = this.recordReference.RecordType,

                    Value = pa.PropertyInfo.PropertyType.IsGuid()
                        ? writer.WriteGuid(columnName)
                        : pa.PropertyInfo.GetValue(this.recordReference.RecordObject)
                };

                return symbol;
            });

            InsertRecordService.Logger.Debug("Exiting GetPrimaryKeyValues");

            return result;
        }

        private string GetPrimaryKeyColumnName()
        {
            InsertRecordService.Logger.Debug("Entering GetPrimaryKeyColumnName");

            PropertyAttribute<PrimaryKeyAttribute> pkPropertyAttribute =
                this.recordReference.RecordType.GetPropertyAttributes<PrimaryKeyAttribute>().First();

            InsertRecordService.Logger.Debug("Exiting GetPrimaryKeyColumnName");

            return Helper.GetColunName(pkPropertyAttribute.PropertyInfo);
        }

        #endregion WritePrimitives

        public virtual void CopyForeignKeyColumns(IEnumerable<Column> foreignKeyColumns)
        {
            foreignKeyColumns.ToList().ForEach(c =>
            {
                if (c.Value.IsSpecialType())
                {
                    return;
                }

                PropertyInfo targetProperty =
                    this.recordReference.RecordType.GetPropertiesHelper().First(p => Helper.GetColunName(p).Equals(c.Name));

                targetProperty.SetValue(this.recordReference.RecordObject, c.Value);
            });
        }
    }
}
