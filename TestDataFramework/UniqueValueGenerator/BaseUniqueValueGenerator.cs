/*
    Copyright 2016, 2017 Alexander Kuperman

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
using System.Reflection;
using log4net;
using TestDataFramework.Logger;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.UniqueValueGenerator.Interfaces;

namespace TestDataFramework.UniqueValueGenerator
{
    public abstract class BaseUniqueValueGenerator : IUniqueValueGenerator
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(BaseUniqueValueGenerator));

        private readonly IPropertyValueAccumulator accumulator;
        private readonly IDeferredValueGenerator<LargeInteger> deferredValueGenerator;
        private readonly bool throwIfUnhandledType;


        protected BaseUniqueValueGenerator(IPropertyValueAccumulator accumulator,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator, bool throwIfUnhandledType)
        {
            this.accumulator = accumulator;
            this.deferredValueGenerator = deferredValueGenerator;
            this.throwIfUnhandledType = throwIfUnhandledType;
        }

        public virtual object GetValue(PropertyInfo propertyInfo)
        {
            BaseUniqueValueGenerator.Logger.Debug("Entering GetValue");

            this.UnhandledTypeCheck(propertyInfo.PropertyType);

            object result = this.accumulator.GetValue(propertyInfo, Helper.DefaultInitalCount);

            BaseUniqueValueGenerator.Logger.Debug($"Exiting GetValue. result: {result}");
            return result;
        }

        protected virtual void DeferValue(PropertyInfo propertyInfo)
        {
            BaseUniqueValueGenerator.Logger.Debug("Entering DeferValue");

            this.UnhandledTypeCheck(propertyInfo.PropertyType);

            this.deferredValueGenerator.AddDelegate(propertyInfo,

                initialCount => this.accumulator.GetValue(propertyInfo, initialCount)

                );

            BaseUniqueValueGenerator.Logger.Debug("Exiting DeferValue");
        }

        private void UnhandledTypeCheck(Type type)
        {
            if (this.throwIfUnhandledType && !this.accumulator.IsTypeHandled(type))
            {
                throw new UnHandledTypeException(Messages.UnhandledUniqueKeyType, type);
            }
        }
    }
}
