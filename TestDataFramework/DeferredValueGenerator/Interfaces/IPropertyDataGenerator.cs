using System.Collections.Generic;
using System.Reflection;

namespace TestDataFramework.DeferredValueGenerator.Interfaces
{
    public interface IPropertyDataGenerator<T>
    {
        void FillData(IDictionary<PropertyInfo, Data<T>> propertyDataDictionary);
    }
}