﻿/*
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

using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator.Concrete
{
    public class KeyTypeUniqueValueGenerator : BaseUniqueValueGenerator
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(KeyTypeUniqueValueGenerator));

        private readonly IAttributeDecorator attributeDecorator;

        public KeyTypeUniqueValueGenerator(IPropertyValueAccumulator accumulator,
            IAttributeDecorator attributeDecorator,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator, bool throwIfUnhandledType)
            : base(accumulator, deferredValueGenerator, throwIfUnhandledType)
        {
            this.attributeDecorator = attributeDecorator;
        }

        public override object GetValue(PropertyInfoProxy propertyInfo)
        {
            KeyTypeUniqueValueGenerator.Logger.Debug(
                $"Entering GetValue. propertyInfo: {propertyInfo.GetExtendedMemberInfoString()}");

            var primaryKeyAttribute = this.attributeDecorator.GetSingleAttribute<PrimaryKeyAttribute>(propertyInfo);

            if (primaryKeyAttribute == null)
            {
                KeyTypeUniqueValueGenerator.Logger.Debug("primaryKeyAttribute == null");

                object result = base.GetValue(propertyInfo);

                KeyTypeUniqueValueGenerator.Logger.Debug($"result fom base: {result?.GetType()}");

                return result ?? Helper.GetDefaultValue(propertyInfo.PropertyType);
            }

            KeyTypeUniqueValueGenerator.Logger.Debug($"primaryKeyAttribute.KeyType: {primaryKeyAttribute.KeyType}");

            if (this.attributeDecorator.GetSingleAttribute<ForeignKeyAttribute>(propertyInfo) != null ||
                primaryKeyAttribute.KeyType != PrimaryKeyAttribute.KeyTypeEnum.Manual || !new[]
                {
                    typeof(byte), typeof(int), typeof(short), typeof(long), typeof(string),
                    typeof(uint), typeof(ushort), typeof(ulong)
                }.Contains(propertyInfo.PropertyType))
            {
                KeyTypeUniqueValueGenerator.Logger.Debug("Not deferring value. Exiting GetValue.");
                return Helper.GetDefaultValue(propertyInfo.PropertyType);
            }

            KeyTypeUniqueValueGenerator.Logger.Debug("Deferring value");

            this.DeferValue(propertyInfo);

            KeyTypeUniqueValueGenerator.Logger.Debug("Exiting GetValue");
            return Helper.GetDefaultValue(propertyInfo.PropertyType);
        }
    }
}