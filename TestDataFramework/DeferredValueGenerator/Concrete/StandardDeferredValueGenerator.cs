using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class StandardDeferredValueGenerator<T> : IDeferredValueGenerator<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (StandardDeferredValueGenerator<T>));

        private readonly IPropertyDataGenerator<T> dataSource;
        private readonly Dictionary<PropertyInfo, Data<T>> propertyDataDictionary = new Dictionary<PropertyInfo, Data<T>>();

        public StandardDeferredValueGenerator(IPropertyDataGenerator<T> dataSource)
        {
            StandardDeferredValueGenerator<T>.Logger.Debug($"Entering constructor. T: {typeof(T)}");

            this.dataSource = dataSource;

            StandardDeferredValueGenerator<T>.Logger.Debug("Exiting constructor");
        }

        public void AddDelegate(PropertyInfo targetPropertyInfo, DeferredValueGetterDelegate<T> valueGetter)
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

            targets.ToList().ForEach(targetRecordReference =>
            {
                StandardDeferredValueGenerator<T>.Logger.Debug("Target object type: " + targetRecordReference.RecordType);

                targetRecordReference.RecordType.GetPropertiesHelper().ToList().ForEach(propertyInfo =>
                {
                    StandardDeferredValueGenerator<T>.Logger.Debug("Property: " + propertyInfo.GetExtendedMemberInfoString());

                    if (targetRecordReference.IsExplicitlySet(propertyInfo))
                    {
                        StandardDeferredValueGenerator<T>.Logger.Debug(
                            "Property explicitly set. Continuing to next iteration.");

                        return;
                    }

                    Data<T> data;
                    if (!this.propertyDataDictionary.TryGetValue(propertyInfo, out data))
                    {
                        StandardDeferredValueGenerator<T>.Logger.Debug(
                            "Property not in deferred properties dictionary. Continuing to next iteration.");

                        return;
                    }

                    StandardDeferredValueGenerator<T>.Logger.Debug($"Property found in deferred properties dictionary. Data: {data}");

                    object value = data.ValueGetter(data.Item);

                    value = value ?? Helper.GetDefaultValue(propertyInfo.PropertyType);

                    propertyInfo.SetValue(targetRecordReference.RecordObject, value);
                });
            });

            this.propertyDataDictionary.Clear();

            StandardDeferredValueGenerator<T>.Logger.Debug("Exiting Execute");
        }
    }
}
