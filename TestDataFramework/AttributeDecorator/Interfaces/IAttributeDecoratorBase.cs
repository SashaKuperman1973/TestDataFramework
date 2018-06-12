using System;
using System.Collections.Generic;
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Interfaces
{
    public interface IAttributeDecoratorBase
    {
        T GetSingleAttribute<T>(MemberInfo memberInfo) where T : Attribute;

        IEnumerable<T> GetCustomAttributes<T>(MemberInfo memberInfo) where T : Attribute;

        IEnumerable<Attribute> GetCustomAttributes(MemberInfo memberInfo);
    }
}