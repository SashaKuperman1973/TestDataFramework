using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;

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

        public static IEnumerable<T> GetUniquePropertyAttributes<T>(this Type type) where T : Attribute
        {
            IEnumerable<T> result = type.GetProperties()
                .Select(p => p.GetSingleAttribute<T>()).ToList();

            return result;
        }
    }
}
