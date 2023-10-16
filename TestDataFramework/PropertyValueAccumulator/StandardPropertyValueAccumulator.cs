/*
    Copyright 2016, 2017, 2018, 2019, 2023 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;

namespace TestDataFramework.PropertyValueAccumulator
{
    public class StandardPropertyValueAccumulator : IPropertyValueAccumulator
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardPropertyValueAccumulator));
        private readonly IAttributeDecorator attributeDecorator;

        private readonly ConcurrentDictionary<PropertyInfoProxy, LargeInteger> countDictionary =
            new ConcurrentDictionary<PropertyInfoProxy, LargeInteger>();

        private readonly LetterEncoder stringGenerator;

        public StandardPropertyValueAccumulator(LetterEncoder stringGenerator, IAttributeDecorator attributeDecorator)
        {
            StandardPropertyValueAccumulator.Logger.Debug("Entering contructor");

            this.stringGenerator = stringGenerator;
            this.attributeDecorator = attributeDecorator;

            StandardPropertyValueAccumulator.Logger.Debug("Exiting contructor");
        }

        public object GetValue(PropertyInfoProxy propertyInfo, LargeInteger initialCount)
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
                typeof(string),
                typeof(byte), typeof(int), typeof(short), typeof(long),
                typeof(uint), typeof(ushort), typeof(ulong)
            }.Contains(type);

            StandardPropertyValueAccumulator.Logger.Debug($"Exiting IsTypeHandled. result: {result}");
            return result;
        }

        private object PrivateGetValue(PropertyInfoProxy propertyInfo)
        {
            StandardPropertyValueAccumulator.Logger.Debug("Entering PrivateGetValue");

            StandardPropertyValueAccumulator.Logger.Debug("Property: " + propertyInfo);

            object result = null;

            Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            // string

            if (type == typeof(string))
            {
                StandardPropertyValueAccumulator.Logger.Debug("Property type string");
                result = this.GetString(propertyInfo);
            }

            // integer

            else if (new[]
            {
                typeof(byte), typeof(int), typeof(short), typeof(long),
                typeof(uint), typeof(ushort), typeof(ulong)
            }.Contains(type))
            {
                StandardPropertyValueAccumulator.Logger.Debug("Property type integral numeric");
                ulong value = (ulong) this.GetCount(propertyInfo);
                result = Convert.ChangeType(value, type);
            }

            StandardPropertyValueAccumulator.Logger.Debug("Exiting PrivateGetValue");
            return result;
        }

        private LargeInteger GetCount(PropertyInfoProxy propertyInfo)
        {
            LargeInteger result = this.countDictionary.GetOrAdd(propertyInfo,
                pi => { throw new KeyNotFoundException(string.Format(Messages.PropertyNotFound, pi)); });

            return result;
        }

        private string GetString(PropertyInfoProxy propertyInfo)
        {
            const int defaultStringLength = 10;

            LargeInteger count = this.GetCount(propertyInfo);

            var stringLengthAttribute = this.attributeDecorator.GetCustomAttribute<StringLengthAttribute>(propertyInfo);

            int stringLength = stringLengthAttribute?.Length ?? defaultStringLength;

            string result = this.stringGenerator.Encode(count, stringLength);

            return result;
        }
    }
}