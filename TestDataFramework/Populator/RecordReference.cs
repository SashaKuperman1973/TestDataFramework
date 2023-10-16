/*
    Copyright 2016, 2017, 2018, 2019, 2023 Alexander Kuperman

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
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator
{
    public abstract class RecordReference : Populatable
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(RecordReference));

        protected IAttributeDecorator AttributeDecorator { get; }
        protected ITypeGenerator TypeGenerator { get; }

        internal readonly List<RecordReference> PrimaryKeyReferences = new List<RecordReference>();

        internal Dictionary<string, RecordReference> ExplicitPrimaryKeyRecords
            = new Dictionary<string, RecordReference>();

        protected RecordReference(ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator)
        {
            this.TypeGenerator = typeGenerator;
            this.AttributeDecorator = attributeDecorator;
        }

        protected internal virtual object RecordObjectBase { get; set; }

        public abstract Type RecordType { get; }

        public virtual void AddPrimaryRecordReference(params RecordReference[] primaryRecordReferences)
        {
            RecordReference.Logger.Debug("Entering AddPrimaryRecordReference(RecordReference[])");

            primaryRecordReferences.ToList().ForEach(this.AddPrimaryRecordReference);

            RecordReference.Logger.Debug("Entering AddPrimaryRecordReference(RecordReference[])");
        }

        public virtual void AddPrimaryRecordReference(RecordReference primaryRecordReference)
        {
            RecordReference.Logger.Debug(
                $"Entering AddPrimaryRecordReference(RecordReference). record object: {primaryRecordReference.RecordObjectBase?.GetType()}");

            if (!this.ValidateRelationship(primaryRecordReference))
                throw new NoReferentialIntegrityException(primaryRecordReference.RecordType, this.RecordType);

            this.PrimaryKeyReferences.Add(primaryRecordReference);

            RecordReference.Logger.Debug("Exiting AddPrimaryRecordReference(RecordReference)");
        }

        public abstract bool IsExplicitlySet(PropertyInfoProxy propertyInfo);

        public abstract bool IsExplicitlySetAndNotCollectionSizeChangeOnly(PropertyInfoProxy propertyInfo);

        private bool ValidateRelationship(RecordReference primaryRecordReference)
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
                                    pkPa.PropertyInfoProxy.DeclaringType ==
                                    this.AttributeDecorator.GetPrimaryTableType(fkPa.Attribute,
                                        new TypeInfoWrapper(this.RecordType.GetTypeInfo()))
                                    &&
                                    Helper.GetColumnName(pkPa.PropertyInfoProxy, this.AttributeDecorator)
                                        .Equals(fkPa.Attribute.PrimaryKeyName, StringComparison.Ordinal)
                                    &&
                                    pkPa.PropertyInfoProxy.PropertyType ==
                                    (Nullable.GetUnderlyingType(fkPa.PropertyInfoProxy.PropertyType) ??
                                     fkPa.PropertyInfoProxy.PropertyType
                                    )
                            )
                    );

            RecordReference.Logger.Debug("Exiting ValidateRelationship");

            return result;
        }
    }
}