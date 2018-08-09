/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.DeepSetting;
using TestDataFramework.Exceptions;

namespace TestDataFramework.Populator.Concrete.FieldExpression
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
