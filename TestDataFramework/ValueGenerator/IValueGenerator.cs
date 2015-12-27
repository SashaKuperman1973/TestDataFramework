using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.ValueGenerator
{
    public interface IValueGenerator
    {
        object GetValue(PropertyInfo propertyInfo, Type type);

        object GetValue(PropertyInfo propertyInfo);
    }
}
