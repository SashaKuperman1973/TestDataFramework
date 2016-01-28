using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class DefaultInitialCountGenerator : IPropertyDataGenerator<LargeInteger>
    {
        public void FillData(IDictionary<PropertyInfo, Data<LargeInteger>> propertyDataDictionary)
        {
            propertyDataDictionary.ToList().ForEach(kvp => kvp.Value.Item = Helper.DefaultInitalCount);
        }
    }
}
