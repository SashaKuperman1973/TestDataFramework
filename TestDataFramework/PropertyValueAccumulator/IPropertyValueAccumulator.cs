using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Helpers;

namespace TestDataFramework.PropertyValueAccumulator
{
    public interface IPropertyValueAccumulator
    {
        object GetValue(PropertyInfo propertyInfo, LargeInteger initialCount);
        bool IsTypeHandled(Type type);
    }
}
