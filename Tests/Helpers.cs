/*
    Copyright 2016 Alexander Kuperman

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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
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

        public static void ExceptionTest(Action getValue, Type exceptionType, string message)
        {
            // Act

            Exception exception = null;

            try
            {
                getValue();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert

            Assert.IsNotNull(exception);
            Assert.AreEqual(exceptionType, exception.GetType());
            Assert.AreEqual(message, exception.Message);
        }

        public static Mock<ITypeGenerator> GetTypeGeneratorMock<T>(T returnObject)
        {
            var typeGeneratorMock = new Mock<ITypeGenerator>();

            Helpers.SetupTypeGeneratorMock<T>(typeGeneratorMock, returnObject);

            return typeGeneratorMock;
        }

        public static void SetupTypeGeneratorMock<T>(Mock<ITypeGenerator> typeGeneratorMock, T returnObject)
        {
            typeGeneratorMock.Setup(
                m => m.GetObject<T>(It.IsAny<ConcurrentDictionary<PropertyInfo, Action<T>>>()))
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

                .Select(p => new Column {Name = Helper.GetColumnName(p, attributeDecorator), Value = p.GetValue(record)}).ToList();

            return result;
        }
    }
}
