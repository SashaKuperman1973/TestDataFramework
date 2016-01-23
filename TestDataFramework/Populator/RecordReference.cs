using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.Populator
{
    public abstract class RecordReference
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RecordReference));

        private readonly ICollection<PropertyInfo> explicitlySetProperties;

        protected RecordReference(object recordObject, ICollection<PropertyInfo> explicitlySetProperties)
        {
            this.RecordObject = recordObject;
            this.explicitlySetProperties = explicitlySetProperties ?? Enumerable.Empty<PropertyInfo>().ToArray();
        }

        public virtual object RecordObject { get; }

        public virtual Type RecordType => this.RecordObject.GetType();

        public readonly IEnumerable<RecordReference> PrimaryKeyReferences = new List<RecordReference>();

        public virtual void AddPrimaryRecordReference(params RecordReference[] primaryRecordReferences)
        {
            RecordReference.Logger.Debug("Entering AddPrimaryRecordReference(RecordReference[])");

            primaryRecordReferences.ToList().ForEach(this.AddPrimaryRecordReference);

            RecordReference.Logger.Debug("Entering AddPrimaryRecordReference(RecordReference[])");
        }

        public virtual void AddPrimaryRecordReference(RecordReference primaryRecordReference)
        {
            RecordReference.Logger.Debug("Entering AddPrimaryRecordReference(RecordReference)");

            if (!this.ValidateRelationship(primaryRecordReference))
            {
                throw new NoReferentialIntegrityException(primaryRecordReference.RecordType, this.RecordType);
            }

            ((List<RecordReference>)this.PrimaryKeyReferences).Add(primaryRecordReference);

            RecordReference.Logger.Debug("Exiting AddPrimaryRecordReference(RecordReference)");
        }

        protected virtual bool ValidateRelationship(RecordReference primaryRecordReference)
        {
            RecordReference.Logger.Debug("Entering ValidateRelationship");

            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> foreignKeyPropertyAttributes =
                this.RecordType.GetPropertyAttributes<ForeignKeyAttribute>().ToList();

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
                                        .Equals(fkPa.Attribute.PrimaryKeyName, StringComparison.Ordinal)
                                    &&
                                    pkPa.PropertyInfo.PropertyType == fkPa.PropertyInfo.PropertyType
                                )
                    );

            RecordReference.Logger.Debug("Exiting ValidateRelationship");

            return result;
        }

        public virtual bool IsExplicitlySet(PropertyInfo propertyInfo)
        {
            return this.explicitlySetProperties.Contains(propertyInfo);
        }
    }
}
