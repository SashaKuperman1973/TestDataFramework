using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace TestDataFramework.DeepSetting.Interfaces
{
    public interface IObjectGraphService
    {
        List<PropertyInfo> GetObjectGraph<T, TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression);
    }
}