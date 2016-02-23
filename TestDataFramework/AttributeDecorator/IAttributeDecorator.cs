using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.AttributeDecorator
{
    public interface IAttributeDecorator
    {
        T GetSingleAttribute<T>(MemberInfo memberInfo) where T : Attribute;

        IEnumerable<T> GetUniqueAttributes<T>(Type type) where T : Attribute;

        PropertyAttribute<T> GetPropertyAttribute<T>(PropertyInfo propertyInfo) where T : Attribute;

        IEnumerable<PropertyAttribute<T>> GetPropertyAttributes<T>(Type type) where T : Attribute;

        IEnumerable<RepositoryOperations.Model.PropertyAttributes> GetPropertyAttributes(Type type);

        IEnumerable<T> GetCustomAttributes<T>(MemberInfo memberInfo) where T : Attribute;

        T GetCustomAttribute<T>(MemberInfo memberInfo) where T : Attribute;

        IEnumerable<Attribute> GetCustomAttributes(MemberInfo memberInfo);

        void DecorateMember<T, TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, Attribute attribute);

        void DecorateType(Type type, Attribute attribute);
    }
}