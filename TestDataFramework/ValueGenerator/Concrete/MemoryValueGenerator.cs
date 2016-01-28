using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.UniqueValueGenerator.Interface;
using TestDataFramework.ValueProvider;

namespace TestDataFramework.ValueGenerator.Concrete
{
    public class MemoryValueGenerator : BaseValueGenerator
    {
        public MemoryValueGenerator(IValueProvider valueProvider, GetTypeGeneratorDelegate getTypeGenerator,
            Func<IArrayRandomizer> getArrayRandomizer, IUniqueValueGenerator uniqueValueGenerator)
            : base(valueProvider, getTypeGenerator, getArrayRandomizer, uniqueValueGenerator)
        {
        }

        protected override object GetGuid(PropertyInfo propertyInfo)
        {
            return Guid.NewGuid();
        }
    }
}
