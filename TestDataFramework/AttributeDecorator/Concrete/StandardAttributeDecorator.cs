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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using log4net;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Helpers.FieldExpressionValidator.Concrete;
using TestDataFramework.Logger;
using TestDataFramework.RepositoryOperations.Model;
using PropertyAttributes = TestDataFramework.RepositoryOperations.Model.PropertyAttributes;

namespace TestDataFramework.AttributeDecorator.Concrete
{
    public class StandardAttributeDecorator : StandardAttributeDecoratorBase, IAttributeDecorator
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardAttributeDecorator));
        private readonly AssemblyWrapper callingAssembly;

        private readonly AddAttributeFieldExpressionValidator fieldExpressionValidator =
            new AddAttributeFieldExpressionValidator();

        private readonly StandardTableTypeCache tableTypeCache;

        public StandardAttributeDecorator(StandardTableTypeCache tableTypeCache,
            AssemblyWrapper callingAssembly, Schema defaultSchema) : base(defaultSchema)
        {
            this.tableTypeCache = tableTypeCache;
            this.callingAssembly = callingAssembly;
        }

        public virtual void DecorateMember<T, TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression,
            Attribute attribute)
        {
            StandardAttributeDecorator.Logger.Debug($"Entering DecorateMember. Attribute: {attribute}");

            MemberInfoProxy memberInfo = new MemberInfoProxy(
                this.fieldExpressionValidator.ValidateMemberAccessExpression(fieldExpression).Member
            );

            this.MemberAttributeDicitonary.AddOrUpdate(memberInfo, new List<Attribute> {attribute},
                (mi, list) =>
                {
                    list.Add(attribute);
                    return list;
                });

            StandardAttributeDecorator.Logger.Debug("Exiting DecorateMember");
        }

        public virtual void DecorateType(Type type, Attribute attribute)
        {
            StandardAttributeDecorator.Logger.Debug($"Entering DecorateType. Type: {type}. Attribute: {attribute}");

            this.MemberAttributeDicitonary.AddOrUpdate(new MemberInfoProxy(type), new List<Attribute> {attribute}, (t, list) =>
            {
                list.Add(attribute);
                return list;
            });
        }

        public virtual IEnumerable<T> GetUniqueAttributes<T>(Type type) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug($"Entering GetUniqueAttributes. T: {typeof(T)} type: {type}");

            IEnumerable<T> result = type.GetPropertiesHelper()
                .Select(this.GetSingleAttribute<T>).Where(a => a != null);

            StandardAttributeDecorator.Logger.Debug($"Exiting GetUniqueAttributes. result: {result}");
            return result;
        }

        public virtual PropertyAttribute<T> GetPropertyAttribute<T>(PropertyInfoProxy propertyInfo) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug(
                $"Entering GetPropertyAttribute. T: {typeof(T)} propertyInfo: {propertyInfo.GetExtendedMemberInfoString()}");

            var result = new PropertyAttribute<T>
            {
                PropertyInfoProxy = propertyInfo,
                Attribute = this.GetSingleAttribute<T>(propertyInfo)
            };

            StandardAttributeDecorator.Logger.Debug($"Exiting GetPropertyAttribute. result: {result}");
            return result;
        }

        public virtual IEnumerable<PropertyAttribute<T>> GetPropertyAttributes<T>(Type type) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug($"Entering GetPropertyAttributes. T: {typeof(T)}, type: {type}");

            IEnumerable<PropertyAttribute<T>> result =
                type.GetPropertiesHelper().Select(this.GetPropertyAttribute<T>).Where(pa => pa.Attribute != null);

            StandardAttributeDecorator.Logger.Debug($"Exiting GetPropertyAttributes. result: {result}");

            return result;
        }

        public virtual IEnumerable<PropertyAttributes> GetPropertyAttributes(Type type)
        {
            StandardAttributeDecorator.Logger.Debug($"Entering GetPropertyAttributes. type: {type}");

            IEnumerable<PropertyAttributes> result =
                type.GetPropertiesHelper()
                    .Select(
                        pi =>
                            new PropertyAttributes
                            {
                                Attributes = this.GetCustomAttributes(pi).ToArray(),
                                PropertyInfoProxy = pi
                            });

            StandardAttributeDecorator.Logger.Debug($"Exiting GetPropertyAttributes. result: {result}");
            return result;
        }

        public virtual T GetCustomAttribute<T>(MemberInfoProxy memberInfo) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug(
                $"Entering GetCustomAttribute<T>. T: {typeof(T)}. MemberInfoProxy: {memberInfo}");

            T result = this.GetCustomAttributes<T>(memberInfo).FirstOrDefault();

            StandardAttributeDecorator.Logger.Debug("Exiting GetCustomAttribute<T>.");
            return result;
        }

        public virtual Type GetPrimaryTableType(ForeignKeyAttribute foreignAttribute, TypeInfoWrapper foreignType)
        {
            StandardAttributeDecorator.Logger.Debug(
                $"Entering GetTableType. ForeignAttribute : {foreignAttribute}. ForeignType: {foreignType}");

            if (foreignAttribute.PrimaryTableType != null)
            {
                StandardAttributeDecorator.Logger.Debug(
                    $"Returning PrimaryTableType: {foreignAttribute.PrimaryTableType}.");

                return foreignAttribute.PrimaryTableType;
            }

            if (!this.tableTypeCache.IsAssemblyCachePopulated(this.callingAssembly))
            {
                StandardAttributeDecorator.Logger.Debug("Populating table-type cache with calling assembly");
                this.tableTypeCache.PopulateAssemblyCache(this.callingAssembly, this.GetSingleAttribute<TableAttribute>,
                    this.DefaultSchema);
            }

            TypeInfoWrapper cachedType = this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType,
                this.callingAssembly,
                this.GetSingleAttribute<TableAttribute>, true);

            if (cachedType != null)
            {
                StandardAttributeDecorator.Logger.Debug(
                    $"Cache hit based on call with calling assembly. Returning {cachedType}.");
                return cachedType.Type;
            }

            if (this.tableTypeCache.IsAssemblyCachePopulated(foreignType.Assembly))
                return null;

            StandardAttributeDecorator.Logger.Debug("Populating table-type cache with foreign type's assembly.");

            this.tableTypeCache.PopulateAssemblyCache(foreignType.Assembly, this.GetSingleAttribute<TableAttribute>,
                this.DefaultSchema);

            cachedType = this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType, foreignType.Assembly,
                this.GetSingleAttribute<TableAttribute>, false);

            if (cachedType == null)
                return null;

            StandardAttributeDecorator.Logger.Debug(
                $"Cache hit based on call with foreign type's assembly. Returning {cachedType}.");
            return cachedType.Type;
        }
    }
}