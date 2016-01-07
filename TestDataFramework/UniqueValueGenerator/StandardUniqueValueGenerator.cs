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

namespace TestDataFramework.UniqueValueGenerator
{
    public class StandardUniqueValueGenerator : IUniqueValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardUniqueValueGenerator));

        private readonly StringGenerator stringGenerator;
        private readonly IDeferredValueGenerator<ulong> deferredValueGenerator;

        public StandardUniqueValueGenerator(StringGenerator stringGenerator, IDeferredValueGenerator<ulong> deferredValueGenerator)
        {
            this.stringGenerator = stringGenerator;
            this.deferredValueGenerator = deferredValueGenerator;
        }

        public object GetValue(PropertyInfo propertyInfo)
        {
            StandardUniqueValueGenerator.Logger.Debug("Entering GetValue");

            this.deferredValueGenerator.AddDelegate(propertyInfo, initialCount =>
            {
                this.countDictionary.AddOrUpdate(propertyInfo, pi => initialCount, (pi, value) => ++value);

                object result = this.PrivateGetValue(propertyInfo);

                return result;
            });

            StandardUniqueValueGenerator.Logger.Debug("Exiting GetValue");

            return propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
        }

        private object PrivateGetValue(PropertyInfo propertyInfo)
        {
            StandardUniqueValueGenerator.Logger.Debug("Entering PrivateGetValue");

            StandardUniqueValueGenerator.Logger.Debug("Property: " + propertyInfo);

            object result = null;

            Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            // string

            if (type == typeof (string))
            {
                StandardUniqueValueGenerator.Logger.Debug("Property type string");
                result = this.GetString(propertyInfo);
            }

            // integer

            else if (new[] { typeof(byte), typeof (int), typeof (short), typeof (long)}.Contains(type))
            {
                StandardUniqueValueGenerator.Logger.Debug("Property type integral numeric");
                ulong value = this.GetCount(propertyInfo);
                result = Convert.ChangeType(value, type);
            }

            StandardUniqueValueGenerator.Logger.Debug("Exiting PrivateGetValue");
            return result;
        }

        private readonly ConcurrentDictionary<PropertyInfo, ulong> countDictionary = new ConcurrentDictionary<PropertyInfo, ulong>();

        private ulong GetCount(PropertyInfo propertyInfo)
        {
            ulong result = this.countDictionary.GetOrAdd(propertyInfo,
                pi => { throw new KeyNotFoundException(string.Format(Messages.PropertyNotFound, pi)); });

            return result;
        }

        private string GetString(PropertyInfo propertyInfo)
        {
            const int defaultStringLength = 10;

            ulong count = this.GetCount(propertyInfo);

            var stringLengthAttribute = propertyInfo.GetAttribute<StringLengthAttribute>();

            int stringLength = stringLengthAttribute?.Length ?? defaultStringLength;

            string result = this.stringGenerator.GetValue(count, stringLength);

            return result;
        }
    }
}
