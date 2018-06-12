/*
    Copyright 2016, 2017 Alexander Kuperman

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
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.TypeGenerator.Interfaces;

namespace Tests
{
    public static class Helpers
    {
        public static IEnumerable<Column> ColumnSymbolToColumn(IEnumerable<ExtendedColumnSymbol> columnsSymbol)
        {
            return columnsSymbol.Select(fkc => new Column {Name = fkc.ColumnName, Value = fkc.Value});
        }

        public static void ExceptionTest(Action action, Type exceptionType, string message = null)
        {
            // Act

            Exception exception = null;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert

            Assert.IsNotNull(exception);
            Assert.AreEqual(exceptionType, exception.GetType());

            if (message != null)
                Assert.AreEqual(message, exception.Message);
        }

        public static Mock<ITypeGenerator> GetTypeGeneratorMock<T>(T returnObject)
        {
            var typeGeneratorMock = new Mock<ITypeGenerator>();

            Helpers.SetupTypeGeneratorMock(typeGeneratorMock, returnObject);

            return typeGeneratorMock;
        }

        public static void SetupTypeGeneratorMock<T>(Mock<ITypeGenerator> typeGeneratorMock, T returnObject)
        {
            typeGeneratorMock.Setup(
                    m => m.GetObject<T>(It.IsAny<IEnumerable<ExplicitPropertySetters>>()))
                .Returns(returnObject);
        }

        public static List<Column> GetColumns<T>(T record, IAttributeDecorator attributeDecorator)
        {
            PrimaryKeyAttribute primaryKeyAttribute;

            List<Column> result = record.GetType()
                .GetProperties()
                .Where(
                    p =>
                        (primaryKeyAttribute = attributeDecorator.GetSingleAttribute<PrimaryKeyAttribute>(p)) == null ||
                        primaryKeyAttribute.KeyType != PrimaryKeyAttribute.KeyTypeEnum.Auto)
                .Select(p => new Column
                {
                    Name = Helper.GetColumnName(p, attributeDecorator),
                    Value = p.GetValue(record)
                }).ToList();

            return result;
        }

        public static void SetTypeGeneratorMock<T>(Mock<ITypeGenerator> typeGeneratorMock, T objectToSet,
            params string[] propertyNamesToSet)
        {
            typeGeneratorMock.Setup(
                    m => m.GetObject<T>(It.IsAny<IEnumerable<ExplicitPropertySetters>>()))
                .Callback<IEnumerable<ExplicitPropertySetters>>(
                    setters =>
                    {
                        propertyNamesToSet.ToList().ForEach(propertyName =>
                            setters.FirstOrDefault(
                                    setter => setter.PropertyChain?.SingleOrDefault()?.Name == propertyName)
                                ?.Action(objectToSet));
                    });
        }

        public class ObjectGraphMockSetup<T>
        {
            private readonly Mock<IObjectGraphService> service;

            public ObjectGraphMockSetup(Mock<IObjectGraphService> service)
            {
                this.service = service;
            }

            public ObjectGraphMockSetup<T> Setup<TPropertyType>(string propertyName)
            {
                Func<Expression<Func<T, TPropertyType>>, string, bool> evaluatePropertyInfo =
                    (expression, propertyNameToEvaluate) => ObjectGraphMockSetup<T>
                        .ValidateMemberAccessExpression(expression).Member
                        .Name
                        .Equals(propertyNameToEvaluate, StringComparison.Ordinal);

                this.service
                    .Setup(m => m.GetObjectGraph(
                        It.Is<Expression<Func<T, TPropertyType>>>(
                            expression => evaluatePropertyInfo(expression, propertyName))))
                    .Returns(new List<PropertyInfo>(new[] {typeof(T).GetProperty(propertyName)}));

                return this;
            }

            private static MemberExpression ValidateMemberAccessExpression(Expression expression)
            {
                var lambdaExpression = expression as LambdaExpression;

                expression = lambdaExpression?.Body ?? expression;

                if (expression.NodeType != ExpressionType.MemberAccess)
                    throw new MemberAccessException();

                var memberExpression = expression as MemberExpression;

                if (memberExpression != null) return ObjectGraphMockSetup<T>.ValidatePropertyInfo(memberExpression);

                var unaryExpression = expression as UnaryExpression;

                if (unaryExpression == null)
                    throw new MemberAccessException();

                memberExpression = unaryExpression.Operand as MemberExpression;

                if (memberExpression == null)
                    throw new MemberAccessException();

                return ObjectGraphMockSetup<T>.ValidatePropertyInfo(memberExpression);
            }

            private static MemberExpression ValidatePropertyInfo(MemberExpression memberExpression)
            {
                if (!(memberExpression.Member is PropertyInfo))
                    throw new MemberAccessException();

                return memberExpression;
            }

            internal class MemberAccessException : Exception
            {
            }
        }
    }
}