using System;
using System.Reflection;
using log4net;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.UniqueValueGenerator.Interfaces;
using TestDataFramework.ValueProvider.Interfaces;

namespace TestDataFramework.ValueGenerator.Concrete
{
    public class SqlClientValueGenerator : BaseValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlClientValueGenerator));

        public SqlClientValueGenerator(IValueProvider valueProvider, GetTypeGeneratorDelegate getTypeGenerator,
            Func<IArrayRandomizer> getArrayRandomizer, IUniqueValueGenerator uniqueValueGenerator)
            : base(valueProvider, getTypeGenerator, getArrayRandomizer, uniqueValueGenerator)
        {
        }

        protected override object GetGuid(PropertyInfo propertyInfo)
        {
            SqlClientValueGenerator.Logger.Debug("Executing GetGuid");

            return default(Guid);
        }
    }
}
