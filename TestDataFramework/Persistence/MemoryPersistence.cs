using System.Collections.Generic;
using System.Linq;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.Persistence
{
    public class MemoryPersistence : IPersistence
    {
        private readonly IDeferredValueGenerator<LargeInteger> deferredValueGenerator;

        public MemoryPersistence(IDeferredValueGenerator<LargeInteger> deferredValueGenerator)
        {
            this.deferredValueGenerator = deferredValueGenerator;
        }

        public void Persist(IEnumerable<RecordReference> recordReferences)
        {
            recordReferences = recordReferences.ToList();

            this.deferredValueGenerator.Execute(recordReferences.Select(r => r.RecordObject));

            MemoryPersistence.CopyPrimaryToForeignKeys(recordReferences);
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

                object primaryKeyPropertyValue = primaryKey.PkProperty.GetValue(primaryKey.Object);
                fkpa.PropertyInfo.SetValue(recordReference.RecordObject, primaryKeyPropertyValue);
            });
        }
    }
}
