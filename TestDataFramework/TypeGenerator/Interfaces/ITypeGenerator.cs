using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace TestDataFramework.TypeGenerator.Interfaces
{
    public interface ITypeGenerator
    {
        object GetObject<T>(ConcurrentDictionary<PropertyInfo, Action<T>> propertyExpressionDictionary);

        object GetObject(Type forType);
    }
}