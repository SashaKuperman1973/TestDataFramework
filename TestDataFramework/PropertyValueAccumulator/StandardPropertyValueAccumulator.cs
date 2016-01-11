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
using TestDataFramework.UniqueValueGenerator;

namespace TestDataFramework.PropertyValueAccumulator
{
    public class StandardPropertyValueAccumulator : IPropertyValueAccumulator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardPropertyValueAccumulator));

        private readonly LetterEncoder stringGenerator;

        public StandardPropertyValueAccumulator(LetterEncoder stringGenerator)
        {
            this.stringGenerator = stringGenerator;
        }

        public object GetValue(PropertyInfo propertyInfo, ulong initialCount)
        {
            this.countDictionary.AddOrUpdate(propertyInfo, pi => initialCount, (pi, value) => ++value);
            object result = this.PrivateGetValue(propertyInfo);
            return result;
        }

        private object PrivateGetValue(PropertyInfo propertyInfo)
        {
            StandardPropertyValueAccumulator.Logger.Debug("Entering PrivateGetValue");

            StandardPropertyValueAccumulator.Logger.Debug("Property: " + propertyInfo);

            object result = null;

            Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            // string

            if (type == typeof (string))
            {
                StandardPropertyValueAccumulator.Logger.Debug("Property type string");
                result = this.GetString(propertyInfo);
            }

            // integer

            else if (new[] { typeof(byte), typeof (int), typeof (short), typeof (long)}.Contains(type))
            {
                StandardPropertyValueAccumulator.Logger.Debug("Property type integral numeric");
                ulong value = this.GetCount(propertyInfo);
                result = Convert.ChangeType(value, type);
            }

            StandardPropertyValueAccumulator.Logger.Debug("Exiting PrivateGetValue");
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

            string result = this.stringGenerator.Encode(count, stringLength);

            return result;
        }
    }
}
