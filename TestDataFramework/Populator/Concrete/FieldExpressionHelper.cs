using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.DeepSetting;
using TestDataFramework.Exceptions;

namespace TestDataFramework.Populator.Concrete
{
    public static class FieldExpressionHelper
    {
        public static ExplicitPropertySetter GetFuncOrValueBasedExlicitPropertySetter<TProperty>(object value, List<PropertyInfo> setterObjectGraph)
        {
            if (value == null)
                return new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                    Action = @object => setterObjectGraph.Last().SetValue(@object, null)
                };

            var valueFunc = value as Func<TProperty>;
            if (valueFunc == null && !(value is TProperty))
            {
                throw new ValueGuaranteeException(string.Format(Messages.GuaranteedTypeNotOfListType,
                    typeof(TProperty), value.GetType()));
            }

            if (valueFunc != null)
            {
                return new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                    Action = @object => setterObjectGraph.Last().SetValue(@object, valueFunc())
                };
            }

            return new ExplicitPropertySetter
            {
                PropertyChain = setterObjectGraph,
                Action = @object => setterObjectGraph.Last().SetValue(@object, value)
            };
        }
    }
}
