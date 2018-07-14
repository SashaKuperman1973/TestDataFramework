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

        public List<PropertyInfo> GetObjectGraph<T, TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression)
        {
            var propertyChain = new List<PropertyInfo>();
            this.GetMemberInfo(propertyChain, fieldExpression.Body);

            return propertyChain;
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