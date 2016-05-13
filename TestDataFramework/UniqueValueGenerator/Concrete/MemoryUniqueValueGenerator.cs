/*
    Copyright 2016 Alexander Kuperman

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
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator.Concrete
{
    public class MemoryUniqueValueGenerator : BaseUniqueValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MemoryUniqueValueGenerator));

        private readonly IAttributeDecorator attributeDecorator;

        public MemoryUniqueValueGenerator(IPropertyValueAccumulator accumulator, IAttributeDecorator attributeDecorator,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator, bool throwIfUnhandledType)
            : base(accumulator, deferredValueGenerator, throwIfUnhandledType)
        {
            this.attributeDecorator = attributeDecorator;
        }

        public override object GetValue(PropertyInfo propertyInfo)
        {
            MemoryUniqueValueGenerator.Logger.Debug($"Entering GetValue. propertyInfo: {propertyInfo.GetExtendedMemberInfoString()}");

            if (propertyInfo.PropertyType == typeof (Guid))
            {
                MemoryUniqueValueGenerator.Logger.Debug("Property type is Guid. Returning new Guid.");
                Guid result = Guid.NewGuid();

                MemoryUniqueValueGenerator.Logger.Debug($"result: {result}");
                return result;
            }

            var primaryKeyAttribute = this.attributeDecorator.GetSingleAttribute<PrimaryKeyAttribute>(propertyInfo);

            if (primaryKeyAttribute == null)
            {
                MemoryUniqueValueGenerator.Logger.Debug("primaryKeyAttribute == null");
                object result = base.GetValue(propertyInfo);

                MemoryUniqueValueGenerator.Logger.Debug($"result fom base: {result}");
                return result ?? Helper.GetDefaultValue(propertyInfo.PropertyType);
            }

            if (primaryKeyAttribute.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Manual
                || primaryKeyAttribute.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Auto)
            {
                MemoryUniqueValueGenerator.Logger.Debug("Deferring value");
                this.DeferValue(propertyInfo);
            }

            MemoryUniqueValueGenerator.Logger.Debug("Exiting GetValue.");
            return Helper.GetDefaultValue(propertyInfo.PropertyType);
        }
    }
}