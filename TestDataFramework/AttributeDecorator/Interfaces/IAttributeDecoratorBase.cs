using System;
using System.Collections.Generic;
using System.Reflection;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;

namespace TestDataFramework.AttributeDecorator.Interfaces
{
    public interface IAttributeDecoratorBase
    {
        T GetSingleAttribute<T>(TestDataTypeInfo testDataTypeInfo) where T : Attribute;

        T GetSingleAttribute<T>(MemberInfo memberInfo) where T : Attribute;

        IEnumerable<T> GetCustomAttributes<T>(MemberInfo memberInfo) where T : Attribute;

        IEnumerable<Attribute> GetCustomAttributes(MemberInfo memberInfo);
    }
}