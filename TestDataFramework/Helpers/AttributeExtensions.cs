using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.Helpers
{
    public static class AttributeExtensions
    {
        public static T GetSingleAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            T[] result = memberInfo.GetCustomAttributes<T>().ToArray();

            if (result.Length <= 1)
            {
                return result.FirstOrDefault();
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
            IEnumerable<T> result = type.GetPropertiesHelper()
                .Select(p => p.GetSingleAttribute<T>()).Where(a => a != null);

            return result;
        }

        public static PropertyAttribute<T> GetPropertyAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            var result = new PropertyAttribute<T>
            {
                PropertyInfo = propertyInfo,
                Attribute = propertyInfo.GetSingleAttribute<T>()
            };

            return result;
        }

        public static IEnumerable<PropertyAttribute<T>> GetPropertyAttributes<T>(
            this Type type) where T : Attribute
        {
            IEnumerable<PropertyAttribute<T>> result =
                type.GetPropertiesHelper().Select(pi => pi.GetPropertyAttribute<T>()).Where(pa => pa.Attribute != null);

            return result;
        }
    }
}
