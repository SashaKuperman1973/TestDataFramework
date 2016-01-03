using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.UniqueValueGenerator
{
    public class StandardUniqueValueGenerator : IUniqueValueGenerator
    {
        private readonly StringGenerator stringGenerator;

        public StandardUniqueValueGenerator(StringGenerator stringGenerator)
        {
            this.stringGenerator = stringGenerator;
        }

        public object GetValue(PropertyInfo propertyInfo)
        {
            object result = null;

            Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            // string

            if (type == typeof (string))
            {
                result = this.GetString(propertyInfo);
            }

            // integer

            else if (new[] {typeof (int), typeof (short), typeof (long)}.Contains(type))
            {
                object value = this.GetInteger(propertyInfo, type);
                result = Convert.ChangeType(value, type);
            }

            return result;
        }

        private readonly ConcurrentDictionary<PropertyInfo, long> stringCountDictionary = new ConcurrentDictionary<PropertyInfo, long>();

        private long GetCount(PropertyInfo propertyInfo)
        {
            long result = this.stringCountDictionary.AddOrUpdate(propertyInfo, pi => 0, (pi, v) => ++v);
            return result;
        }

        private string GetString(PropertyInfo propertyInfo)
        {
            const int defaultStringLength = 10;

            long count = this.GetCount(propertyInfo);

            var stringLengthAttribute = propertyInfo.GetAttribute<StringLengthAttribute>();

            int stringLength = stringLengthAttribute?.Length ?? defaultStringLength;

            string result = this.stringGenerator.GetValue(count, stringLength);

            return result;
        }

        private object GetInteger(PropertyInfo propertyInfo, Type type)
        {
            long value = this.GetCount(propertyInfo);
            object result = Convert.ChangeType(value, type);
            return result;
        }
    }
}
