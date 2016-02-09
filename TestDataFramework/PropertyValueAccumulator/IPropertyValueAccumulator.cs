using System;
using System.Reflection;
using TestDataFramework.Helpers;

namespace TestDataFramework.PropertyValueAccumulator
{
    public interface IPropertyValueAccumulator
    {
        object GetValue(PropertyInfo propertyInfo, LargeInteger initialCount);
        bool IsTypeHandled(Type type);
    }
}
