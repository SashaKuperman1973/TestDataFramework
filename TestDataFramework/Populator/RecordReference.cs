using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.TypeGenerator;

namespace TestDataFramework.Populator
{
    public abstract class RecordReference
    {
        protected RecordReference(object recordObject)
        {
            this.RecordObject = recordObject;
        }

        public object RecordObject { get; }

        public Type RecordType => this.RecordObject.GetType();

        public readonly IEnumerable<RecordReference> PrimaryKeyReferences = new List<RecordReference>();

        public void AddPrimaryRecordReference(RecordReference primaryRecordReference)
        {
            if (!this.ValidateRelationship(primaryRecordReference))
            {
                throw new NoReferentialIntegrityException(primaryRecordReference.RecordType, this.RecordType);
            }

            ((List<RecordReference>)this.PrimaryKeyReferences).Add(primaryRecordReference);
        }

        private bool ValidateRelationship(RecordReference primaryRecordReference)
        {
            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> foreignKeyPropertyAttributes =
                this.RecordType.GetPropertyAttributes<ForeignKeyAttribute>();

            bool result =
                primaryRecordReference.RecordType
                    .GetPropertyAttributes<PrimaryKeyAttribute>()
                    .All(
                        pkPa =>
                            foreignKeyPropertyAttributes.Any(
                                fkPa =>
                                    pkPa.PropertyInfo.DeclaringType == fkPa.Attribute.PrimaryTableType
                                    &&
                                    Helper.GetColunName(pkPa.PropertyInfo)
                                        .Equals(fkPa.Attribute.PrimaryKeyName)));

            return result;
        }
    }
}
