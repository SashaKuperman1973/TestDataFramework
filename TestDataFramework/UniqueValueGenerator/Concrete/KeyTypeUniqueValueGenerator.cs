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
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator.Concrete
{
    public class KeyTypeUniqueValueGenerator : BaseUniqueValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (KeyTypeUniqueValueGenerator));

        private readonly IAttributeDecorator attributeDecorator;

        public KeyTypeUniqueValueGenerator(IPropertyValueAccumulator accumulator, IAttributeDecorator attributeDecorator,
            IDeferredValueGenerator<LargeInteger> deferredValueGenerator, bool throwIfUnhandledType)
            : base(accumulator, deferredValueGenerator, throwIfUnhandledType)
        {
            this.attributeDecorator = attributeDecorator;
        }

        public override object GetValue(PropertyInfo propertyInfo)
        {
            KeyTypeUniqueValueGenerator.Logger.Debug(
                $"Entering GetValue. propertyInfo: {propertyInfo.GetExtendedMemberInfoString()}");

            var primaryKeyAttribute = this.attributeDecorator.GetSingleAttribute<PrimaryKeyAttribute>(propertyInfo);

            if (primaryKeyAttribute == null)
            {
                KeyTypeUniqueValueGenerator.Logger.Debug("primaryKeyAttribute == null");

                object result = base.GetValue(propertyInfo);

                KeyTypeUniqueValueGenerator.Logger.Debug($"result fom base: {result}");
                return result ?? Helper.GetDefaultValue(propertyInfo.PropertyType);
            }

            KeyTypeUniqueValueGenerator.Logger.Debug($"primaryKeyAttribute.KeyType: {primaryKeyAttribute.KeyType}");

            if (this.attributeDecorator.GetSingleAttribute<ForeignKeyAttribute>(propertyInfo) != null ||
                primaryKeyAttribute.KeyType != PrimaryKeyAttribute.KeyTypeEnum.Manual || !new[]
                {
                    typeof (byte), typeof (int), typeof (short), typeof (long), typeof (string),
                    typeof (uint), typeof (ushort), typeof (ulong),
                }.Contains(propertyInfo.PropertyType))
            {
                return Helper.GetDefaultValue(propertyInfo.PropertyType);                
            }

            KeyTypeUniqueValueGenerator.Logger.Debug("Deferring value");
            this.DeferValue(propertyInfo);

            return Helper.GetDefaultValue(propertyInfo.PropertyType);
        }
    }
}