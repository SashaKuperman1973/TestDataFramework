using System.Collections.Generic;
using System.Linq;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.Persistence
{
    public class MemoryPersistence : IPersistence
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MemoryPersistence));

        private readonly IDeferredValueGenerator<LargeInteger> deferredValueGenerator;

        public MemoryPersistence(IDeferredValueGenerator<LargeInteger> deferredValueGenerator)
        {
            MemoryPersistence.Logger.Debug("Entering constructor");

            this.deferredValueGenerator = deferredValueGenerator;

            MemoryPersistence.Logger.Debug("Exiting constructor");
        }

        public void Persist(IEnumerable<RecordReference> recordReferences)
        {
            MemoryPersistence.Logger.Debug("Entering Persist");

            recordReferences = recordReferences.ToList();

            MemoryPersistence.Logger.Debug($"Records: {string.Join(", ", recordReferences.Select(r => r?.RecordObject))}");

            this.deferredValueGenerator.Execute(recordReferences);

            MemoryPersistence.CopyPrimaryToForeignKeys(recordReferences);

            MemoryPersistence.Logger.Debug("Exiting Persist");
        }

        private static void CopyPrimaryToForeignKeys(IEnumerable<RecordReference> recordReferences)
        {
            recordReferences.ToList().ForEach(MemoryPersistence.CopyPrimaryToForeignKeys);
        }

        private static void CopyPrimaryToForeignKeys(RecordReference recordReference)
        {
            var primaryKeys = recordReference.PrimaryKeyReferences.SelectMany(
                pkRef =>
                    pkRef.RecordType.GetPropertyAttributes<PrimaryKeyAttribute>()
                        .Select(pkpa => new { @Object = pkRef.RecordObject, PkProperty = pkpa.PropertyInfo }));

            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> foreignKeyPropertyAttributes =
                recordReference.RecordType.GetPropertyAttributes<ForeignKeyAttribute>();

            foreignKeyPropertyAttributes.ToList().ForEach(fkpa =>
            {
                var primaryKey =
                    primaryKeys.FirstOrDefault(
                        pk =>
                            pk.PkProperty.DeclaringType == fkpa.Attribute.PrimaryTableType &&
                            Helper.GetColumnName(pk.PkProperty) == fkpa.Attribute.PrimaryKeyName);

                if (primaryKey == null)
                {
                    return;
                }

                MemoryPersistence.Logger.Debug($"PropertyInfo to get from: {primaryKey.PkProperty}");
                object primaryKeyPropertyValue = primaryKey.PkProperty.GetValue(primaryKey.Object);
                MemoryPersistence.Logger.Debug($"primaryKeyPropertyValue: {primaryKeyPropertyValue}");

                MemoryPersistence.Logger.Debug($"PropertyInfo to set: {fkpa.PropertyInfo}");
                fkpa.PropertyInfo.SetValue(recordReference.RecordObject, primaryKeyPropertyValue);
            });
        }
    }
}
