using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class DeferredValueGenerator<T> : IDeferredValueGenerator<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (DeferredValueGenerator<T>));

        private readonly IDbProviderDeferredValueGenerator<T> dataSource;

        public DeferredValueGenerator(IDbProviderDeferredValueGenerator<T> dataSource)
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
        private readonly Dictionary<PropertyInfo, Data> propertyDataDictionary = new Dictionary<PropertyInfo, Data>();

        public void AddDelegate(PropertyInfo propertyInfo, DeferredValueGetterDelegate<T> valueGetter)
        {
            DeferredValueGenerator<T>.Logger.Debug("Entering AddDelegate. propertyInfo: " + propertyInfo);

            this.propertyDataDictionary.Add(propertyInfo, new Data(valueGetter));

            DeferredValueGenerator<T>.Logger.Debug("Exiting AddDelegate");
        }

        public void Execute(IEnumerable<object> targets)
        {
            DeferredValueGenerator<T>.Logger.Debug("Entering Execute");

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

                    object value = data.ValueGetter(this.propertyDataDictionary[propertyInfo].Item);

                    propertyInfo.SetValue(target, value);
                });
            });

            this.hasExecuted = true;

            DeferredValueGenerator<T>.Logger.Debug("Exiting Execute");
        }
    }
}
