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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.AttributeDecorator
{
    public class StandardAttributeDecorator : IAttributeDecorator, ITableTypeCacheService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardAttributeDecorator));

        private readonly ConcurrentDictionary<MemberInfo, List<Attribute>> memberAttributeDicitonary =
            new ConcurrentDictionary<MemberInfo, List<Attribute>>();

        private readonly TableTypeCache tableTypeCache;
        private readonly string defaultSchema;
        private readonly Assembly callingAssembly;
        
        public StandardAttributeDecorator(Func<ITableTypeCacheService, TableTypeCache> createTableTypeCache,
            Assembly callingAssembly) : this(createTableTypeCache, callingAssembly, null)
        {
        }

        public StandardAttributeDecorator(Func<ITableTypeCacheService, TableTypeCache> createTableTypeCache,
            Assembly callingAssembly, string defaultSchema)
        {
            this.tableTypeCache = createTableTypeCache(this);
            this.defaultSchema = defaultSchema;
            this.callingAssembly = callingAssembly;
        }

        public virtual void DecorateMember<T, TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, Attribute attribute)
        {
            StandardAttributeDecorator.Logger.Debug($"Entering DecorateMember. Attribute: {attribute}");

            MemberInfo memberInfo = Helper.ValidateFieldExpression(fieldExpression);

            this.memberAttributeDicitonary.AddOrUpdate(memberInfo, new List<Attribute> { attribute },
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

            this.memberAttributeDicitonary.AddOrUpdate(type, new List<Attribute> {attribute}, (t, list) =>
            {
                list.Add(attribute);
                return list;
            });
        }

        public virtual T GetSingleAttribute<T>(MemberInfo memberInfo) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug(
                $"Entering GetSingleAttribute. T: {typeof(T)} memberInfo: {memberInfo.GetExtendedMemberInfoString()}");

            T[] result = this.GetCustomAttributes<T>(memberInfo).ToArray();

            if (result.Length <= 1)
            {
                T firstOrDefaultResult = result.FirstOrDefault();

                StandardAttributeDecorator.Logger.Debug($"Member attributes count <= 1. firstOrDefaultResult: {firstOrDefaultResult}");
                return firstOrDefaultResult;
            }

            string message =
                memberInfo.MemberType == MemberTypes.Property
                    ? Messages.AmbigousPropertyAttributeMatch
                    : memberInfo.MemberType == MemberTypes.TypeInfo || memberInfo.MemberType == MemberTypes.NestedType
                        ? Messages.AmbigousTypeAttributeMatch
                        : Messages.AmbigousAttributeMatch;

            throw new AmbiguousMatchException(string.Format(message, typeof(T), memberInfo.Name, memberInfo.DeclaringType));
        }

        public virtual IEnumerable<T> GetUniqueAttributes<T>(Type type) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug($"Entering GetUniqueAttributes. T: {typeof(T)} type: {type}");

            IEnumerable<T> result = type.GetPropertiesHelper()
                .Select(this.GetSingleAttribute<T>).Where(a => a != null);

            StandardAttributeDecorator.Logger.Debug($"Exiting GetUniqueAttributes. result: {result}");
            return result;
        }

        public virtual PropertyAttribute<T> GetPropertyAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug($"Entering GetPropertyAttribute. T: {typeof(T)} propertyInfo: {propertyInfo.GetExtendedMemberInfoString()}");

            var result = new PropertyAttribute<T>
            {
                PropertyInfo = propertyInfo,
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

        public virtual IEnumerable<RepositoryOperations.Model.PropertyAttributes> GetPropertyAttributes(Type type)
        {
            StandardAttributeDecorator.Logger.Debug($"Entering GetPropertyAttributes. type: {type}");

            IEnumerable<RepositoryOperations.Model.PropertyAttributes> result =
                type.GetPropertiesHelper()
                    .Select(
                        pi =>
                            new RepositoryOperations.Model.PropertyAttributes
                            {
                                Attributes = this.GetCustomAttributes(pi).ToArray(),
                                PropertyInfo = pi
                            });

            StandardAttributeDecorator.Logger.Debug($"Exiting GetPropertyAttributes. result: {result}");
            return result;
        }

        public virtual IEnumerable<T> GetCustomAttributes<T>(MemberInfo memberInfo) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug(
                $"Entering GetCustomAttributes<T>. T: {typeof (T)}. memberInfo: {memberInfo}");

            List<Attribute> programmaticAttributeList;

            List<Attribute> attributeResult = this.memberAttributeDicitonary.TryGetValue(memberInfo, out programmaticAttributeList)

                ? programmaticAttributeList.Where(a => a.GetType() == typeof(T)).ToList()

                : new List<Attribute>();

            attributeResult.AddRange(memberInfo.GetCustomAttributes<T>());

            List<T> result = this.InsertDefaultSchema(attributeResult).Cast<T>().ToList();

            StandardAttributeDecorator.Logger.Debug("Exiting GetCustomAttributes<T>");
            return result;
        }

        public virtual IEnumerable<Attribute> GetCustomAttributes(MemberInfo memberInfo)
        {
            StandardAttributeDecorator.Logger.Debug(
                $"Entering GetCustomAttributes. MemberInfo: {memberInfo}");

            List<Attribute> result;

            if (!this.memberAttributeDicitonary.TryGetValue(memberInfo, out result))
            {
                result = new List<Attribute>();
            }

            result.AddRange(memberInfo.GetCustomAttributes());

            result = this.InsertDefaultSchema(result);

            StandardAttributeDecorator.Logger.Debug("Exiting GetCustomAttributes.");
            return result;
        }

        private List<Attribute> InsertDefaultSchema(IEnumerable<Attribute> attributes)
        {
            StandardAttributeDecorator.Logger.Debug("Entering InsertDefaultSchema");

            attributes = attributes.ToList();

            StandardAttributeDecorator.Logger.Debug(
                $"Attributes: {string.Join(",", attributes)}");

            List <Attribute> result = attributes.Select(a =>
            {
                var canHaveDefaultSchema = a as ICanHaveDefaultSchema;

                Attribute resultAttribute = canHaveDefaultSchema?.IsDefaultSchema ?? false
                    ? canHaveDefaultSchema.GetAttributeUsingDefaultSchema(this.defaultSchema)
                    : a;

                return resultAttribute;

            }).ToList();

            StandardAttributeDecorator.Logger.Debug("Exiting InsertDefaultSchema");
            return result;
        }

        public virtual T GetCustomAttribute<T>(MemberInfo memberInfo) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug($"Entering GetCustomAttribute<T>. T: {typeof(T)}. MemberInfo: {memberInfo}");

            T result = this.GetCustomAttributes<T>(memberInfo).FirstOrDefault();

            StandardAttributeDecorator.Logger.Debug("Exiting GetCustomAttribute<T>.");
            return result;
        }

        public virtual Type GetTableType(ForeignKeyAttribute foreignAttribute, Type foreignType)
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
                this.tableTypeCache.PopulateAssemblyCache(this.callingAssembly, this.GetSingleAttribute<TableAttribute>, this.defaultSchema);
            }

            Type cachedType = this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType, this.callingAssembly,
                this.GetSingleAttribute<TableAttribute>, canScanAllCachedAssemblies: true);

            if (cachedType != null)
            {
                StandardAttributeDecorator.Logger.Debug(
                    $"Cache hit based on call with calling assembly. Returning {cachedType}.");
                return cachedType;
            }

            if (this.tableTypeCache.IsAssemblyCachePopulated(foreignType.Assembly))
            {
                throw new AttributeDecoratorException(Messages.CannotResolveForeignKey, foreignAttribute,
                    foreignType);
            }

            StandardAttributeDecorator.Logger.Debug("Populating table-type cache with foreign type's assembly.");

            this.tableTypeCache.PopulateAssemblyCache(foreignType.Assembly, this.GetSingleAttribute<TableAttribute>,
                this.defaultSchema);

            cachedType = this.tableTypeCache.GetCachedTableType(foreignAttribute, foreignType, foreignType.Assembly,
                this.GetSingleAttribute<TableAttribute>, canScanAllCachedAssemblies: false);

            if (cachedType == null)
            {
                throw new AttributeDecoratorException(Messages.CannotResolveForeignKey, foreignAttribute,
                    foreignType);
            }

            StandardAttributeDecorator.Logger.Debug(
                $"Cache hit based on call with foreign type's assembly. Returning {cachedType}.");
            return cachedType;
        }
    }
}
