using System;
using System.Reflection;

namespace TestDataFramework.ValueGenerator.Interfaces
{
    public interface IValueGenerator
    {
        object GetValue(PropertyInfo propertyInfo);

        object GetValue(PropertyInfo propertyInfo, Type type);
    }
}
