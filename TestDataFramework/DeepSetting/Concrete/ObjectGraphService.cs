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
using System.Linq.Expressions;
using System.Reflection;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Helpers.FieldExpressionValidator.Concrete;

namespace TestDataFramework.DeepSetting.Concrete
{
    public class ObjectGraphService : IObjectGraphService
    {
        private readonly PropertySetFieldExpressionValidator fieldExpressionValidator =
            new PropertySetFieldExpressionValidator();

        public List<PropertyInfo> GetObjectGraph<T, TPropertyValue>(Expression<Func<T, TPropertyValue>> fieldExpression)
        {
            var propertyChain = new List<PropertyInfo>();
            this.GetMemberInfo(propertyChain, fieldExpression.Body);

            return propertyChain;
        }

        public bool DoesPropertyHaveSetter(List<PropertyInfo> objectGraphNodeList, IEnumerable<ExplicitPropertySetter> explicitPropertySetters)
        {
            foreach (ExplicitPropertySetter aSetter in explicitPropertySetters)
            {
                if (objectGraphNodeList.Count > aSetter.PropertyChain.Count)
                    continue;

                bool result = true;
                for (int i = 0; i < objectGraphNodeList.Count; i++)
                {
                    if (objectGraphNodeList[i].PropertyType == aSetter.PropertyChain[i].PropertyType) continue;

                    result = false;
                    break;
                }

                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        private void GetMemberInfo(ICollection<PropertyInfo> list, Expression expression)
        {
            if (expression.NodeType == ExpressionType.Parameter || expression.NodeType == ExpressionType.Convert)
                return;

            MemberExpression memberExpression =
                this.fieldExpressionValidator.ValidateMemberAccessExpression(expression);

            this.GetMemberInfo(list, memberExpression.Expression);
            list.Add((PropertyInfo) memberExpression.Member);
        }
    }
}