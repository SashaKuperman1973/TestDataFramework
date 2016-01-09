using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.UniqueValueGenerator
{
    public interface IUniqueValueGenerator
    {
        object GetValue(PropertyInfo propertyInfo);
        void DeferValue(PropertyInfo propertyInfo);
    }
}
