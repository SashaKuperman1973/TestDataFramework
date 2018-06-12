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