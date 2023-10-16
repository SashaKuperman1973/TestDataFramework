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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.Populator;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class StandardDeferredValueGenerator<T> : IDeferredValueGenerator<T>
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardDeferredValueGenerator<T>));

        private static readonly ReferenceRecordObjectEqualityComparer ReferenceRecordObjectEqualityComparerObject =
            new ReferenceRecordObjectEqualityComparer();

        private readonly IPropertyDataGenerator<T> dataSource;

        private readonly Dictionary<PropertyInfoProxy, Data<T>> propertyDataDictionary =
            new Dictionary<PropertyInfoProxy, Data<T>>(new PropertyInfoEqualityComparer());

        public StandardDeferredValueGenerator(IPropertyDataGenerator<T> dataSource)
        {
            StandardDeferredValueGenerator<T>.Logger.Debug($"Entering constructor. T: {typeof(T)}");

            this.dataSource = dataSource;

            StandardDeferredValueGenerator<T>.Logger.Debug("Exiting constructor");
        }

        public void AddDelegate(PropertyInfoProxy targetPropertyInfo, DeferredValueGetterDelegate<T> valueGetter)
        {
            StandardDeferredValueGenerator<T>.Logger.Debug(
                $"Entering AddDelegate. targetPropertyInfo: {targetPropertyInfo.GetExtendedMemberInfoString()}");

            if (this.propertyDataDictionary.ContainsKey(targetPropertyInfo))
            {
                StandardDeferredValueGenerator<T>.Logger.Debug("AddDelegate. Duplicate property. Exiting");

                return;
            }

            this.propertyDataDictionary.Add(targetPropertyInfo, new Data<T>(valueGetter));

            StandardDeferredValueGenerator<T>.Logger.Debug("Exiting AddDelegate");
        }

        public void Execute(IEnumerable<RecordReference> targets)
        {
            StandardDeferredValueGenerator<T>.Logger.Debug("Entering Execute");

            this.dataSource.FillData(this.propertyDataDictionary);

            IEnumerable<RecordReference> uniqueTargets = StandardDeferredValueGenerator<T>.GetUniqueTargets(targets);

            uniqueTargets.ToList().ForEach(targetRecordReference =>
            {
                StandardDeferredValueGenerator<T>.Logger.Debug(
                    "Target object type: " + targetRecordReference.RecordType);

                this.SetValue(targetRecordReference, targetRecordReference.RecordObjectBase);

            });

            this.propertyDataDictionary.Clear();

            StandardDeferredValueGenerator<T>.Logger.Debug("Exiting Execute");
        }

        private void SetValue(RecordReference targetRecordReference, object currentGraphObject)
        {
            var collection = currentGraphObject as IEnumerable;
            if (collection != null)
            {
                foreach (object element in collection)
                {
                    this.DoSetValue(targetRecordReference, element);
                }
            }
            else
            {
                this.DoSetValue(targetRecordReference, currentGraphObject);
            }
        }

        private void DoSetValue(RecordReference targetRecordReference, object currentGraphObject)
        {
            currentGraphObject?.GetType().GetPropertiesHelper().ToList().ForEach(propertyInfo =>
            {
                StandardDeferredValueGenerator<T>.Logger.Debug(
                    "Property: " + propertyInfo.GetExtendedMemberInfoString());

                if (targetRecordReference.IsExplicitlySetAndNotCollectionSizeChangeOnly(propertyInfo))
                {
                    StandardDeferredValueGenerator<T>.Logger.Debug(
                        "Property explicitly set. Continuing to next iteration.");

                    return;
                }

                if (this.propertyDataDictionary.TryGetValue(propertyInfo, out Data<T> data))
                {
                    StandardDeferredValueGenerator<T>.Logger.Debug(
                        $"Property found in deferred properties dictionary. Data: {data}");

                    object value = data.ValueGetter(data.Item);

                    value = value ?? Helper.GetDefaultValue(propertyInfo.PropertyType);

                    propertyInfo.SetValue(currentGraphObject, value);

                    return;
                }

                StandardDeferredValueGenerator<T>.Logger.Debug(
                    "Property not in deferred properties dictionary. Going deeper into the object graph.");

                object nextGraphObject = propertyInfo.GetValue(currentGraphObject);
                this.SetValue(targetRecordReference, nextGraphObject);
            });
        }

        private static IEnumerable<RecordReference> GetUniqueTargets(IEnumerable<RecordReference> targets)
        {
            targets = targets.ToList();

            IEnumerable<RecordReference> distinctReferenceTypes =
                targets.Where(t => !t.RecordObjectBase?.GetType().IsValueType ?? false)
                    .Distinct(StandardDeferredValueGenerator<T>.ReferenceRecordObjectEqualityComparerObject);

            IEnumerable<RecordReference> valueTypes =
                targets.Where(t => t.RecordObjectBase?.GetType().IsValueType ?? false);

            IEnumerable<RecordReference> result = distinctReferenceTypes.Concat(valueTypes);

            return result;
        }

        internal class ReferenceRecordObjectEqualityComparer : IEqualityComparer<RecordReference>
        {
            public bool Equals(RecordReference x, RecordReference y)
            {
                return x.RecordObjectBase == y.RecordObjectBase;
            }

            public int GetHashCode(RecordReference obj)
            {
                return obj.RecordObjectBase.GetHashCode();
            }
        }

        internal class PropertyInfoEqualityComparer : IEqualityComparer<PropertyInfoProxy>
        {
            public bool Equals(PropertyInfoProxy x, PropertyInfoProxy y)
            {
                return x.DeclaringType == y.DeclaringType && x.Name.Equals(y.Name, StringComparison.Ordinal);
            }

            public int GetHashCode(PropertyInfoProxy obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}