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

            T[] result = memberInfo.GetCustomAttributes<T>().ToArray();

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
            AttributeExtensions.Logger.Debug($"Entering GetSingleAttribute. T: {typeof(T)} type: {type}");

            IEnumerable <T> result = type.GetPropertiesHelper()
                .Select(p => p.GetSingleAttribute<T>()).Where(a => a != null);

            AttributeExtensions.Logger.Debug($"Exiting GetSingleAttribute. result: {result}");
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
                                Attributes = pi.GetCustomAttributes().ToArray(),
                                PropertyInfo = pi
                            });

            AttributeExtensions.Logger.Debug($"Exiting GetPropertyAttributes. result: {result}");
            return result;
        }
    }
}
