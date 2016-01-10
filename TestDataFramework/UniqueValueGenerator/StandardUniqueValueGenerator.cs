using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using log4net;
using TestDataFramework.DeferredValueGenerator;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator
{
    public class StandardUniqueValueGenerator : IUniqueValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardUniqueValueGenerator));

        private readonly IPropertyValueAccumulator accumulator;
        private readonly IDeferredValueGenerator<ulong> deferredValueGenerator;

        public StandardUniqueValueGenerator(IPropertyValueAccumulator accumulator, IDeferredValueGenerator<ulong> deferredValueGenerator)
        {
            this.accumulator = accumulator;
            this.deferredValueGenerator = deferredValueGenerator;
        }

        public object GetValue(PropertyInfo propertyInfo)
        {
            object result = this.accumulator.GetValue(propertyInfo, 0);
            return result;
        }

        public void DeferValue(PropertyInfo propertyInfo)
        {
            StandardUniqueValueGenerator.Logger.Debug("Entering GetValue");

            this.deferredValueGenerator.AddDelegate(propertyInfo, initialCount =>
            {
                object result = this.accumulator.GetValue(propertyInfo, initialCount);
                return result;
            });

            StandardUniqueValueGenerator.Logger.Debug("Exiting GetValue");
        }

    }
}
