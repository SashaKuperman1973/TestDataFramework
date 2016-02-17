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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.Helpers
{
    public static class AttributeExtensions
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AttributeExtensions));

        public static T GetSingleAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            AttributeExtensions.Logger.Debug(
                $"Entering GetSingleAttribute. T: {typeof(T)} memberInfo: {memberInfo.GetExtendedMemberInfoString()}");

            T[] result = memberInfo.GetCustomAttributesHelper<T>().ToArray();

            if (result.Length <= 1)
            {
                T firstOrDefaultResult = result.FirstOrDefault();

                AttributeExtensions.Logger.Debug($"Member attributes count <= 1. firstOrDefaultResult: {firstOrDefaultResult}");
                return firstOrDefaultResult;
            }

            string message =
                memberInfo.MemberType == MemberTypes.Property
                    ? Messages.AmbigousPropertyAttributeMatch
                    : memberInfo.MemberType == (MemberTypes.TypeInfo | MemberTypes.NestedType)
                        ? Messages.AmbigousTypeAttributeMatch
                        : Messages.AmbigousAttributeMatch;

            throw new AmbiguousMatchException(string.Format(message, typeof(T), memberInfo.Name, memberInfo.DeclaringType));
        }

        public static IEnumerable<T> GetUniqueAttributes<T>(this Type type) where T : Attribute
        {
            AttributeExtensions.Logger.Debug($"Entering GetUniqueAttributes. T: {typeof(T)} type: {type}");

            IEnumerable <T> result = type.GetPropertiesHelper()
                .Select(p => p.GetSingleAttribute<T>()).Where(a => a != null);

            AttributeExtensions.Logger.Debug($"Exiting GetUniqueAttributes. result: {result}");
            return result;
        }

        public static PropertyAttribute<T> GetPropertyAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            AttributeExtensions.Logger.Debug($"Entering GetPropertyAttribute. T: {typeof(T)} propertyInfo: {propertyInfo.GetExtendedMemberInfoString()}");

            var result = new PropertyAttribute<T>
            {
                PropertyInfo = propertyInfo,
                Attribute = propertyInfo.GetSingleAttribute<T>()
            };

            AttributeExtensions.Logger.Debug($"Exiting GetPropertyAttribute. result: {result}");
            return result;
        }

        public static IEnumerable<PropertyAttribute<T>> GetPropertyAttributes<T>(this Type type) where T : Attribute
        {
            AttributeExtensions.Logger.Debug($"Entering GetPropertyAttributes. T: {typeof(T)}, type: {type}");

            IEnumerable <PropertyAttribute<T>> result =
                type.GetPropertiesHelper().Select(pi => pi.GetPropertyAttribute<T>()).Where(pa => pa.Attribute != null);

            AttributeExtensions.Logger.Debug($"Exiting GetPropertyAttributes. result: {result}");

            return result;
        }

        public static IEnumerable<RepositoryOperations.Model.PropertyAttributes> GetPropertyAttributes(this Type type)
        {
            AttributeExtensions.Logger.Debug($"Entering GetPropertyAttributes. type: {type}");

            IEnumerable<RepositoryOperations.Model.PropertyAttributes> result =
                type.GetPropertiesHelper()
                    .Select(
                        pi =>
                            new RepositoryOperations.Model.PropertyAttributes
                            {
                                Attributes = pi.GetCustomAttributesHelper().ToArray(),
                                PropertyInfo = pi
                            });

            AttributeExtensions.Logger.Debug($"Exiting GetPropertyAttributes. result: {result}");
            return result;
        }

        public static IEnumerable<T> GetCustomAttributesHelper<T>(this MemberInfo memberInfo) where T : Attribute
        {
            List<Attribute> programmaticAttributeList;

            List<T> result = DecoratorHelper.AttributeDicitonary.TryGetValue(memberInfo, out programmaticAttributeList)
                ? programmaticAttributeList.Where(a => a.GetType() == typeof (T)).Cast<T>().ToList()
                : new List<T>();

            result.AddRange(memberInfo.GetCustomAttributes<T>());
            return result;
        }

        public static T GetCustomAttributeHelper<T>(this MemberInfo memberInfo) where T : Attribute
        {
            T result = memberInfo.GetCustomAttributesHelper<T>().FirstOrDefault();
            return result;
        }

        public static IEnumerable<Attribute> GetCustomAttributesHelper(this MemberInfo memberInfo)
        {
            List<Attribute> result;

            if (!DecoratorHelper.AttributeDicitonary.TryGetValue(memberInfo, out result))
            {
                result = new List<Attribute>();
            }

            result.AddRange(memberInfo.GetCustomAttributes());
            return result;
        }
    }
}
