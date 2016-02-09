using System;
using System.Reflection;
using log4net;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.UniqueValueGenerator.Interfaces;
using TestDataFramework.ValueProvider.Interfaces;

namespace TestDataFramework.ValueGenerator.Concrete
{
    public class MemoryValueGenerator : BaseValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MemoryValueGenerator));

        public MemoryValueGenerator(IValueProvider valueProvider, GetTypeGeneratorDelegate getTypeGenerator,
            Func<IArrayRandomizer> getArrayRandomizer, IUniqueValueGenerator uniqueValueGenerator)
            : base(valueProvider, getTypeGenerator, getArrayRandomizer, uniqueValueGenerator)
        {
        }

        protected override object GetGuid(PropertyInfo propertyInfo)
        {
            MemoryValueGenerator.Logger.Debug("Executing GetGuid");

            return Guid.NewGuid();
        }
    }
}
