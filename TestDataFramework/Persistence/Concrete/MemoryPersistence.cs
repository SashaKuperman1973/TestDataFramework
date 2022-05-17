/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using log4net;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.Persistence.Concrete
{
    public class MemoryPersistence : IPersistence
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(MemoryPersistence));
        private readonly IAttributeDecorator attributeDecorator;

        private readonly IDeferredValueGenerator<LargeInteger> deferredValueGenerator;

        public MemoryPersistence(IDeferredValueGenerator<LargeInteger> deferredValueGenerator,
            IAttributeDecorator attributeDecorator)
        {
            MemoryPersistence.Logger.Debug("Entering constructor");

            this.deferredValueGenerator = deferredValueGenerator;
            this.attributeDecorator = attributeDecorator;

            MemoryPersistence.Logger.Debug("Exiting constructor");
        }

        public void Persist(IEnumerable<RecordReference> recordReferences)
        {
            MemoryPersistence.Logger.Debug("Entering Persist");

            recordReferences = recordReferences.ToList();

            MemoryPersistence.Logger.Debug(
                $"Records: {string.Join(", ", recordReferences.Select(r => r?.RecordObjectBase?.GetType()))}");

            this.deferredValueGenerator.Execute(recordReferences);

            this.CopyPrimaryToForeignKeys(recordReferences);

            MemoryPersistence.Logger.Debug("Exiting Persist");
        }

        public void DeleteAll(IEnumerable<RecordReference> recordReferences)
        {
            // NoOp
        }

        private void CopyPrimaryToForeignKeys(IEnumerable<RecordReference> recordReferences)
        {
            recordReferences.ToList().ForEach(recordReference =>
            {
                var recordReferenceSet = new List<RecordReference>();
                var selfAndDescendants = recordReference.SelectMany().ToList();
                selfAndDescendants.ForEach(populatable => populatable.AddToReferences(recordReferenceSet));
                recordReferenceSet.ForEach(this.CopyPrimaryToForeignKeys);
            });
        }

        private void CopyPrimaryToForeignKeys(RecordReference recordReference)
        {
            var primaryKeys = recordReference.PrimaryKeyReferences.SelectMany(
                pkRef =>
                    this.attributeDecorator.GetPropertyAttributes<PrimaryKeyAttribute>(pkRef.RecordType)
                        .Select(pkpa => new { PrimaryKeyReference = pkRef, PkProperty = pkpa.PropertyInfoProxy}));

            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> foreignKeyPropertyAttributes =
                this.attributeDecorator.GetPropertyAttributes<ForeignKeyAttribute>(recordReference.RecordType);

            foreignKeyPropertyAttributes.ToList().ForEach(fkpa =>
            {
                var primaryKey =
                    primaryKeys.FirstOrDefault(
                        pk =>
                            Helper.GetColumnName(pk.PkProperty, this.attributeDecorator) ==
                            fkpa.Attribute.PrimaryKeyName

                            &&

                            Helper.IsForeignToPrimaryKeyMatch(
                                recordReference,
                                fkpa,
                                pk.PrimaryKeyReference,
                                this.attributeDecorator)
                    );

                if (primaryKey?.PrimaryKeyReference.RecordObjectBase == null)
                    return;

                MemoryPersistence.Logger.Debug($"PropertyInfoProxy to get from: {primaryKey.PkProperty}");
                object primaryKeyPropertyValue =
                    primaryKey.PkProperty.GetValue(primaryKey.PrimaryKeyReference.RecordObjectBase);
                MemoryPersistence.Logger.Debug($"primaryKeyPropertyValue: {primaryKeyPropertyValue}");

                MemoryPersistence.Logger.Debug($"PropertyInfoProxy to set: {fkpa.PropertyInfoProxy}");
                fkpa.PropertyInfoProxy.SetValue(recordReference.RecordObjectBase, primaryKeyPropertyValue);
            });
        }
    }
}