using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class StandardDeferredValueGenerator<T> : IDeferredValueGenerator<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (StandardDeferredValueGenerator<T>));

        private readonly IPropertyDataGenerator<T> dataSource;
        private readonly Dictionary<PropertyInfo, Data<T>> propertyDataDictionary = new Dictionary<PropertyInfo, Data<T>>();

        public StandardDeferredValueGenerator(IPropertyDataGenerator<T> dataSource)
        {
            this.dataSource = dataSource;
        }

        public void AddDelegate(PropertyInfo targetPropertyInfo, DeferredValueGetterDelegate<T> valueGetter)
        {
            StandardDeferredValueGenerator<T>.Logger.Debug(
                $"Entering AddDelegate. targetPropertyInfo: {targetPropertyInfo}");

            if (this.propertyDataDictionary.ContainsKey(targetPropertyInfo))
            {
                StandardDeferredValueGenerator<T>.Logger.Debug("AddDelegate. Duplicate property. Exiting");

                return;
            }

            this.propertyDataDictionary.Add(targetPropertyInfo, new Data<T>(valueGetter));

            StandardDeferredValueGenerator<T>.Logger.Debug("Exiting AddDelegate");
        }

        public void Execute(IEnumerable<object> targets)
        {
            StandardDeferredValueGenerator<T>.Logger.Debug("Entering Execute");

            this.dataSource.FillData(this.propertyDataDictionary);

            targets.ToList().ForEach(targetObject =>
            {
                targetObject.GetType().GetProperties().ToList().ForEach(propertyInfo =>
                {
                    Data<T> data;
                    if (!this.propertyDataDictionary.TryGetValue(propertyInfo, out data))
                    {
                        return;
                    }

                    object value = data.ValueGetter(data.Item);

                    propertyInfo.SetValue(propertyInfo, value);
                });
            });

            this.propertyDataDictionary.Clear();

            StandardDeferredValueGenerator<T>.Logger.Debug("Exiting Execute");
        }
    }
}
