using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Helpers;

namespace TestDataFramework
{
    public static class Entity<T>
    {
        public static void Decorate<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, Attribute attribute)
        {
            MemberInfo propertyInfo = Helper.ValidateFieldExpression(fieldExpression);

            DecoratorHelper.AttributeDicitonary.AddOrUpdate(propertyInfo, new List<Attribute> {attribute},
                (pi, list) =>
                {
                    list.Add(attribute);
                    return list;
                });
        }
    }

    public static class DecoratorHelper
    {
        public static ConcurrentDictionary<MemberInfo, List<Attribute>> AttributeDicitonary =
            new ConcurrentDictionary<MemberInfo, List<Attribute>>();
    }
}
