using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class StandardDeferredValueGenerator<T> : IDeferredValueGenerator<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (StandardDeferredValueGenerator<T>));

        private readonly IPropertyDataGenerator<T> dataSource;
        private readonly Dictionary<PropertyInfo, Data> propertyDataDictionary = new Dictionary<PropertyInfo, Data>();

        public StandardDeferredValueGenerator(IPropertyDataGenerator<T> dataSource)
        {
            this.dataSource = dataSource;
        }

        public class Data
        {
            public Data(DeferredValueGetterDelegate<T> valueGetter)
            {
                this.ValueGetter = valueGetter;
            }

            public T Item { get; set; }
            public DeferredValueGetterDelegate<T> ValueGetter { get; }
        }

        private bool hasExecuted = false;

        public void AddDelegate(PropertyInfo propertyInfo, DeferredValueGetterDelegate<T> valueGetter)
        {
            StandardDeferredValueGenerator<T>.Logger.Debug("Entering AddDelegate. propertyInfo: " + propertyInfo);

            if (this.hasExecuted)
            {
                throw new DeferredValueGeneratorExecutedException();
            }

            this.propertyDataDictionary.Add(propertyInfo, new Data(valueGetter));

            StandardDeferredValueGenerator<T>.Logger.Debug("Exiting AddDelegate");
        }

        public void Execute(IEnumerable<object> targets)
        {
            StandardDeferredValueGenerator<T>.Logger.Debug("Entering Execute");

            if (this.hasExecuted)
            {
                throw new DeferredValueGeneratorExecutedException();
            }

            this.dataSource.FillData(this.propertyDataDictionary);

            targets.ToList().ForEach(target =>
            {
                target.GetType().GetProperties().ToList().ForEach(propertyInfo =>
                {
                    Data data;
                    if (!this.propertyDataDictionary.TryGetValue(propertyInfo, out data))
                    {
                        return;
                    }

                    object value = data.ValueGetter(data.Item);

                    propertyInfo.SetValue(target, value);
                });
            });

            this.hasExecuted = true;

            StandardDeferredValueGenerator<T>.Logger.Debug("Exiting Execute");
        }
    }
}
