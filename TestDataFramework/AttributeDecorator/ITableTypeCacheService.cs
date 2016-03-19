using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.AttributeDecorator
{
    public interface ITableTypeCacheService
    {
        T GetSingleAttribute<T>(MemberInfo memberInfo) where T : Attribute;
    }
}
