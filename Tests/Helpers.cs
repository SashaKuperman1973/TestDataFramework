using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
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

        public static List<Column> GetColumns<T>(T record)
        {
            List<Column> result = record.GetType()
                .GetProperties()
                .Where(
                    p =>
                        p.GetSingleAttribute<PrimaryKeyAttribute>() == null ||
                        p.GetSingleAttribute<PrimaryKeyAttribute>().KeyType != PrimaryKeyAttribute.KeyTypeEnum.Auto)
                .Select(p => new Column {Name = Helper.GetColumnName(p), Value = p.GetValue(record)}).ToList();

            return result;
        }
    }
}
