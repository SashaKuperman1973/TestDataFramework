using System.Linq.Expressions;
using System.Reflection;
using TestDataFramework.Exceptions;

namespace TestDataFramework.Helpers.FieldExpressionValidator
{
    public abstract class FieldExpressionValidatorBase
    {
        protected abstract string ErrorMessage { get; }

        public MemberExpression ValidateMemberAccessExpression(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            expression = lambdaExpression?.Body ?? expression;

            if (expression.NodeType != ExpressionType.MemberAccess)
            {
                throw new MemberAccessExpressionException(this.ErrorMessage);
            }

            var memberExpression = expression as MemberExpression;

            if (memberExpression != null) return this.ValidatePropertyInfo(memberExpression);

            var unaryExpression = expression as UnaryExpression;

            if (unaryExpression == null)
            {
                throw new MemberAccessExpressionException(this.ErrorMessage);
            }

            memberExpression = unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new MemberAccessExpressionException(this.ErrorMessage);
            }

            return this.ValidatePropertyInfo(memberExpression);
        }

        private MemberExpression ValidatePropertyInfo(MemberExpression memberExpression)
        {
            if (!(memberExpression.Member is PropertyInfo))
            {
                throw new MemberAccessExpressionException(this.ErrorMessage);
            }

            return memberExpression;
        }
    }
}
