using System;
using System.Reflection;

namespace TestDataFramework.ValueGenerator
{
    public interface IValueGenerator
    {
        object GetValue(PropertyInfo propertyInfo, Type type);

        object GetValue(PropertyInfo propertyInfo);
    }
}
