using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.UniqueValueGenerator.Interface;

namespace TestDataFramework.UniqueValueGenerator
{
    public abstract class BaseUniqueValueGenerator : IUniqueValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseUniqueValueGenerator));

        private readonly IPropertyValueAccumulator accumulator;
        private readonly IDeferredValueGenerator<ulong> deferredValueGenerator;

        protected BaseUniqueValueGenerator(IPropertyValueAccumulator accumulator, IDeferredValueGenerator<ulong> deferredValueGenerator)
        {
            this.accumulator = accumulator;
            this.deferredValueGenerator = deferredValueGenerator;
        }

        public virtual object GetValue(PropertyInfo propertyInfo)
        {
            object result = this.accumulator.GetValue(propertyInfo, Helper.DefaultInitalCount);
            return result;
        }

        protected virtual void DeferValue(PropertyInfo propertyInfo)
        {
            BaseUniqueValueGenerator.Logger.Debug("Entering GetValue");

            this.deferredValueGenerator.AddDelegate(propertyInfo,

                initialCount => this.accumulator.GetValue(propertyInfo, initialCount)

                );

            BaseUniqueValueGenerator.Logger.Debug("Exiting GetValue");
        }
    }
}
