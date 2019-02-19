/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers.FieldExpressionValidator;
using TestDataFramework.Helpers.FieldExpressionValidator.Concrete;
using Tests.TestModels;

namespace Tests.Tests
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