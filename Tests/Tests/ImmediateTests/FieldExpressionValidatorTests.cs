using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers.FieldExpressionValidator;
using TestDataFramework.Helpers.FieldExpressionValidator.Concrete;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class FieldExpressionValidatorTests
    {
        [TestMethod]
        public void FieldExpressionValidatorBase_Test()
        {
            var fieldExpressionValidator = new TestFieldExpressionValidator();

            var expression = (Expression<Func<SubjectClass, SecondClass>>) (subject => subject.SecondObject);

            MemberExpression result = fieldExpressionValidator.ValidateMemberAccessExpression(expression);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AddAttributeFieldExpressionValidator_NotMemberAccessExpression_Throws_Test()
        {
            var fieldExpressionValidator = new AddAttributeFieldExpressionValidator();

            var expression = (Expression<Action<SubjectClass>>) (subject => subject.Integer.ToString());

            Helpers.ExceptionTest(() => fieldExpressionValidator.ValidateMemberAccessExpression(expression),
                typeof(MemberAccessExpressionException), Messages.AddAttributeExpressionMustBePropertyAccess);
        }

        [TestMethod]
        public void PropertySetFieldExpressionValidator_NotMemberAccessExpression_Throws_Test()
        {
            var fieldExpressionValidator = new PropertySetFieldExpressionValidator();

            var expression = (Expression<Action<SubjectClass>>) (subject => subject.Integer.ToString());

            Helpers.ExceptionTest(() => fieldExpressionValidator.ValidateMemberAccessExpression(expression),
                typeof(MemberAccessExpressionException), Messages.PropertySetExpressionMustBePropertyAccess);
        }

        [TestMethod]
        public void CannotBeField_Test()
        {
            var fieldExpressionValidator = new AddAttributeFieldExpressionValidator();

            var expression = (Expression<Func<SubjectClass, int>>) (subject => subject.AField);

            Helpers.ExceptionTest(() => fieldExpressionValidator.ValidateMemberAccessExpression(expression),
                typeof(MemberAccessExpressionException), Messages.AddAttributeExpressionMustBePropertyAccess);
        }

        private class TestFieldExpressionValidator : FieldExpressionValidatorBase
        {
            protected override string ErrorMessage { get; }
        }
    }
}