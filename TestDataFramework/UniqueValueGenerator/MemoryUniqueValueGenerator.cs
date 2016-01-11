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
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator
{
    public class MemoryUniqueValueGenerator : IUniqueValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MemoryUniqueValueGenerator));

        private readonly IPropertyValueAccumulator accumulator;
        private readonly IDeferredValueGenerator<ulong> deferredValueGenerator;

        public MemoryUniqueValueGenerator(IPropertyValueAccumulator accumulator, IDeferredValueGenerator<ulong> deferredValueGenerator)
        {
            this.accumulator = accumulator;
            this.deferredValueGenerator = deferredValueGenerator;
        }

        public virtual object GetValue(PropertyInfo propertyInfo)
        {
            object result = this.accumulator.GetValue(propertyInfo, Helper.DefaultInitalCount);
            return result;
        }

        public virtual void DeferValue(PropertyInfo propertyInfo)
        {
            MemoryUniqueValueGenerator.Logger.Debug("Entering GetValue");

            this.deferredValueGenerator.AddDelegate(propertyInfo, initialCount =>
            {
                object result = this.accumulator.GetValue(propertyInfo, initialCount);
                return result;
            });

            MemoryUniqueValueGenerator.Logger.Debug("Exiting GetValue");
        }

    }
}
