using System.Reflection;

namespace TestDataFramework.UniqueValueGenerator.Interface
{
    public interface IUniqueValueGenerator
    {
        object GetValue(PropertyInfo propertyInfo);
    }
}
