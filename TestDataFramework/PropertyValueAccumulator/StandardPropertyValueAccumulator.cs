using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;

namespace TestDataFramework.PropertyValueAccumulator
{
    public class StandardPropertyValueAccumulator : IPropertyValueAccumulator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardPropertyValueAccumulator));

        private readonly LetterEncoder stringGenerator;

        public StandardPropertyValueAccumulator(LetterEncoder stringGenerator)
        {
            StandardPropertyValueAccumulator.Logger.Debug("Entering contructor");

            this.stringGenerator = stringGenerator;

            StandardPropertyValueAccumulator.Logger.Debug("Exiting contructor");
        }

        public object GetValue(PropertyInfo propertyInfo, LargeInteger initialCount)
        {
            StandardPropertyValueAccumulator.Logger.Debug(
                $"Entering GetValue. propertyInfo: {propertyInfo}, initialCount: {initialCount}");

            this.countDictionary.AddOrUpdate(propertyInfo, pi => initialCount, (pi, value) => ++value);
            object result = this.PrivateGetValue(propertyInfo);

            StandardPropertyValueAccumulator.Logger.Debug("Exiting GetValue");
            return result;
        }

        public bool IsTypeHandled(Type type)
        {
            StandardPropertyValueAccumulator.Logger.Debug("Entering IsTypeHandled");

            bool result = new[]
            {
                typeof (string),
                typeof (byte), typeof (int), typeof (short), typeof (long),
                typeof (uint), typeof (ushort), typeof (ulong),
            }.Contains(type);

            StandardPropertyValueAccumulator.Logger.Debug($"Exiting IsTypeHandled. result: {result}");
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

            else if (new[]
            {
                typeof(byte), typeof (int), typeof (short), typeof (long),
                typeof (uint), typeof (ushort), typeof (ulong),
            }.Contains(type))
            {
                StandardPropertyValueAccumulator.Logger.Debug("Property type integral numeric");
                var value = (ulong)this.GetCount(propertyInfo);
                result = Convert.ChangeType(value, type);
            }

            StandardPropertyValueAccumulator.Logger.Debug("Exiting PrivateGetValue");
            return result;
        }

        private readonly ConcurrentDictionary<PropertyInfo, LargeInteger> countDictionary = new ConcurrentDictionary<PropertyInfo, LargeInteger>();

        private LargeInteger GetCount(PropertyInfo propertyInfo)
        {
            LargeInteger result = this.countDictionary.GetOrAdd(propertyInfo,
                pi => { throw new KeyNotFoundException(string.Format(Messages.PropertyNotFound, pi)); });

            return result;
        }

        private string GetString(PropertyInfo propertyInfo)
        {
            const int defaultStringLength = 10;

            LargeInteger count = this.GetCount(propertyInfo);

            var stringLengthAttribute = propertyInfo.GetAttribute<StringLengthAttribute>();

            int stringLength = stringLengthAttribute?.Length ?? defaultStringLength;

            string result = this.stringGenerator.Encode(count, stringLength);

            return result;
        }
    }
}
