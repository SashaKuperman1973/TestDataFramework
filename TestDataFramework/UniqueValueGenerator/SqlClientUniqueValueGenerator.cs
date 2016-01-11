using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;

namespace TestDataFramework.UniqueValueGenerator
{
    public class SqlClientUniqueValueGenerator : MemoryUniqueValueGenerator
    {
        public SqlClientUniqueValueGenerator(IPropertyValueAccumulator accumulator, IDeferredValueGenerator<ulong> deferredValueGenerator) : base(accumulator, deferredValueGenerator)
        {
        }

        public override object GetValue(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
        }
    }
}
