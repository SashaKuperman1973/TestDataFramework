using System.Reflection;

namespace TestDataFramework.UniqueValueGenerator.Interfaces
{
    public interface IUniqueValueGenerator
    {
        object GetValue(PropertyInfo propertyInfo);
    }
}
