using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Exceptions;

namespace TestDataFramework.DeepSetting.Concrete
{
    public class ObjectGraphService : IObjectGraphService
    {
        public List<PropertyInfo> GetObjectGraph<T, TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression)
        {
            var memberExpression = fieldExpression.Body as MemberExpression;

            var propertyChain = new List<PropertyInfo>();
            ObjectGraphService.GetMemberInfo(propertyChain, memberExpression);

            return propertyChain;
        }

        private static void GetMemberInfo(List<PropertyInfo> list, Expression expression)
        {
            if (expression is ParameterExpression)
            {
                return;
            }

            MemberExpression memberExpression = ObjectGraphService.ValidateMemberAccessExpression(expression);

            ObjectGraphService.GetMemberInfo(list, memberExpression.Expression);
            list.Add((PropertyInfo)memberExpression.Member);
        }

        private static MemberExpression ValidateMemberAccessExpression(Expression expression)
        {
            if (expression.NodeType != ExpressionType.MemberAccess)
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            var memberExpression = expression as MemberExpression;

            if (memberExpression != null) return ObjectGraphService.ValidatePropertyInfo(memberExpression);

            var unaryExpression = expression as UnaryExpression;

            if (unaryExpression == null)
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            memberExpression = unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            return ObjectGraphService.ValidatePropertyInfo(memberExpression);
        }

        private static MemberExpression ValidatePropertyInfo(MemberExpression memberExpression)
        {
            if (!(memberExpression.Member is PropertyInfo))
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            return memberExpression;
        }
    }
}
