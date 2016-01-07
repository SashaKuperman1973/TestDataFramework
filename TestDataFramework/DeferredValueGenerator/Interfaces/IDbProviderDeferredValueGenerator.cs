using System.Collections.Generic;
using System.Reflection;
using TestDataFramework.DeferredValueGenerator.Concrete;

namespace TestDataFramework.DeferredValueGenerator.Interfaces
{
    public interface IDbProviderDeferredValueGenerator<T>
    {
        void FillData(Dictionary<PropertyInfo, DeferredValueGenerator<T>.Data> propertyDataDictionary);
    }
}