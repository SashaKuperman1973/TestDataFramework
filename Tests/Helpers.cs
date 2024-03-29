﻿/*
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;

namespace Tests
{
    public enum MessageOption
    {
        FullMessage,
        MessageStartsWith
    }

    public static class Helpers
    {
        public static void ConfigureLogger()
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()));
        }

        public static IEnumerable<Column> ColumnSymbolToColumn(IEnumerable<ExtendedColumnSymbol> columnsSymbol)
        {
            return columnsSymbol.Select(fkc => new Column {Name = fkc.ColumnName, Value = fkc.Value});
        }

        public static void ExceptionTest(
            Action action, 
            Type exceptionType, 
            string message = null, 
            MessageOption messageOption = MessageOption.FullMessage,
            string failureMessage = null
            )
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

            Assert.IsNotNull(exception, failureMessage);
            Assert.AreEqual(exceptionType, exception.GetType(), failureMessage);

            if (message != null && messageOption == MessageOption.FullMessage)
            {
                Assert.AreEqual(message, exception.Message, failureMessage);
                return;
            }

            if (message != null && messageOption == MessageOption.MessageStartsWith)
            {
                Assert.IsTrue(exception.Message.StartsWith(message), failureMessage);
            }
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
                    m => m.GetObject<T>(It.IsAny<TypeGeneratorContext>()))
                .Returns(returnObject);
        }

        public static List<Column> GetColumns<T>(T record, IAttributeDecorator attributeDecorator)
        {
            PrimaryKeyAttribute primaryKeyAttribute;

            List<Column> result = record.GetType()
                .GetPropertyInfoProxies()
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
                    m => m.GetObject<T>(It.IsAny<TypeGeneratorContext>()))
                .Callback<IEnumerable<ExplicitPropertySetter>>(
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
                    .Returns(new List<PropertyInfoProxy>(new[] {typeof(T).GetPropertyInfoProxy(propertyName)}));

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
                if (!(memberExpression.Member is PropertyInfo || memberExpression.Member is FieldInfo))
                    throw new MemberAccessException();

                return memberExpression;
            }

            internal class MemberAccessException : Exception
            {
            }
        }

        public static void AssertSetsAreEqual(IEnumerable<object> left, IEnumerable<object> right)
        {
            object[] leftArray = left.ToArray();
            object[] rightArray = right.ToArray();

            Assert.AreEqual(leftArray.Length, rightArray.Length);

            for (int i = 0; i < leftArray.Length; i++)
                Assert.AreEqual(leftArray[i], rightArray[i]);
        }

        private static Tuple<IEnumerable<object>, ConstructorInfo> GetArguments<T>() where T : class
        {
            ConstructorInfo[] constructors =
                typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            ConstructorInfo constructor = constructors.OrderBy(ci => ci.GetParameters().Length).First();

            IEnumerable<object> arguments = constructor.GetParameters()
                .Select(parameter => parameter.ParameterType.IsValueType
                    ? Activator.CreateInstance(parameter.ParameterType)
                    : null);

            return new Tuple<IEnumerable<object>, ConstructorInfo>(arguments, constructor);
        }

        public static Mock<T> GetMock<T>() where T : class
        {
            object[] arguments = Helpers.GetArguments<T>().Item1.ToArray();

            var result = new Mock<T>(arguments);
            return result;
        }

        public static T GetObject<T>() where T : class
        {
            Tuple<IEnumerable<object>, ConstructorInfo> arguments = Helpers.GetArguments<T>();

            object result = arguments.Item2.Invoke(arguments.Item1.ToArray());

            return (T)result;
        }

        public const ushort ShortMax = 64513;
    }
}