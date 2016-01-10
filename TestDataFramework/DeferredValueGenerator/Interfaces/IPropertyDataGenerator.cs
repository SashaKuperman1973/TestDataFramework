using System.Collections.Generic;
using System.Reflection;
using TestDataFramework.DeferredValueGenerator.Concrete;

namespace TestDataFramework.DeferredValueGenerator.Interfaces
{
    public interface IPropertyDataGenerator<T>
    {
        void FillData(IDictionary<PropertyInfo, StandardDeferredValueGenerator<T>.Data> propertyDataDictionary);
    }
}