using System;
using System.Reflection;

namespace TestDataFramework.ArrayRandomizer
{
    public interface IArrayRandomizer
    {
        object GetArray(PropertyInfo propertyInfo, Type type);
    }
}
