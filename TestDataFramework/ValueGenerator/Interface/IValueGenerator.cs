using System;
using System.Reflection;

namespace TestDataFramework.ValueGenerator.Interface
{
    public interface IValueGenerator
    {
        object GetValue(PropertyInfo propertyInfo);

        object GetValue(PropertyInfo propertyInfo, Type type);
    }
}
