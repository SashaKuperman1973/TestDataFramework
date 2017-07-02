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
using TestDataFramework.Logger;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator
{
    public abstract class RecordReference
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(RecordReference));

        protected readonly ITypeGenerator TypeGenerator;
        protected readonly IAttributeDecorator AttributeDecorator;

        protected RecordReference(ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator)
        {
            this.TypeGenerator = typeGenerator;
            this.AttributeDecorator = attributeDecorator;
        }

        #region Public fields/properties

        public virtual object RecordObject { get; protected internal set; }

        protected internal virtual Func<object> PreBoundObject { get; set; }

        protected internal virtual Type RecordType { get; protected set; }

        protected internal readonly IEnumerable<RecordReference> PrimaryKeyReferences = new List<RecordReference>();

        #endregion Public fields/properties

        #region Public methods

        public virtual void AddPrimaryRecordReference(params RecordReference[] primaryRecordReferences)
        {
            RecordReference.Logger.Debug("Entering AddPrimaryRecordReference(RecordReference[])");

            primaryRecordReferences.ToList().ForEach(this.AddPrimaryRecordReference);

            RecordReference.Logger.Debug("Entering AddPrimaryRecordReference(RecordReference[])");
        }

        public virtual void AddPrimaryRecordReference(RecordReference primaryRecordReference)
        {
            RecordReference.Logger.Debug(
                $"Entering AddPrimaryRecordReference(RecordReference). record object: {primaryRecordReference.RecordObject}");

            if (!this.ValidateRelationship(primaryRecordReference))
            {
                throw new NoReferentialIntegrityException(primaryRecordReference.RecordType, this.RecordType);
            }

            ((List<RecordReference>)this.PrimaryKeyReferences).Add(primaryRecordReference);

            RecordReference.Logger.Debug("Exiting AddPrimaryRecordReference(RecordReference)");
        }

        protected internal abstract bool IsExplicitlySet(PropertyInfo propertyInfo);

        #endregion Public methods

        protected virtual bool ValidateRelationship(RecordReference primaryRecordReference)
        {
            RecordReference.Logger.Debug("Entering ValidateRelationship");

            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> foreignKeyPropertyAttributes =
                this.AttributeDecorator.GetPropertyAttributes<ForeignKeyAttribute>(this.RecordType).ToList();

            bool result =
                this.AttributeDecorator.GetPropertyAttributes<PrimaryKeyAttribute>(primaryRecordReference.RecordType)
                    .All(
                        pkPa =>
                            foreignKeyPropertyAttributes.Any(
                                fkPa =>
                                    pkPa.PropertyInfo.DeclaringType ==
                                    this.AttributeDecorator.GetTableType(fkPa.Attribute, this.RecordType)
                                    &&
                                    Helper.GetColumnName(pkPa.PropertyInfo, this.AttributeDecorator)
                                        .Equals(fkPa.Attribute.PrimaryKeyName, StringComparison.Ordinal)
                                    &&
                                    pkPa.PropertyInfo.PropertyType ==
                                    (Nullable.GetUnderlyingType(fkPa.PropertyInfo.PropertyType) ??
                                     fkPa.PropertyInfo.PropertyType
                                    )
                            )
                    );

            RecordReference.Logger.Debug("Exiting ValidateRelationship");

            return result;
        }

        protected internal abstract void Populate();
    }
}
