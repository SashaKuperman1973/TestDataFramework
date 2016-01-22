using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;
using Tests.Mocks;

namespace Tests.Tests
{
    [TestClass]
    public class SqlClientWritePrimitivesTests
    {
        private Mock<DbProviderFactory> dbProviderFactoryMock;
        private SqlClientWritePrimitives primitives;
        private Mock<IValueFormatter> formatterMock;
        private Mock<IRandomSymbolStringGenerator> symbolGeneratorMock;

        private const string ConnectionString = "cn";

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.dbProviderFactoryMock = new Mock<DbProviderFactory>();
            this.formatterMock = new Mock<IValueFormatter>();
            this.symbolGeneratorMock = new Mock<IRandomSymbolStringGenerator>();

            this.primitives = new SqlClientWritePrimitives(SqlClientWritePrimitivesTests.ConnectionString,
                this.dbProviderFactoryMock.Object, this.formatterMock.Object, this.symbolGeneratorMock.Object,
                mustBeInATransaction: false,
                configuration: new NameValueCollection {{"TestDataFramework_DumpSqlInput", "true"}});
        }

        [TestMethod]
        public void SelectIdentity_Test()
        {
            // Arrange

            var connectionMock = new Mock<DbConnection>();
            var insertCommandMock = new Mock<DbCommand>();
            var readerMock = new Mock<DbDataReader>();
            var mockInsertCommand = new MockDbCommand(insertCommandMock.Object, readerMock.Object);

            this.dbProviderFactoryMock.Setup(m => m.CreateCommand()).Returns(mockInsertCommand);
            this.dbProviderFactoryMock.Setup(m => m.CreateConnection()).Returns(connectionMock.Object);

            const string variableSymbol = "ABCD";
            this.symbolGeneratorMock.Setup(m => m.GetRandomString(It.IsAny<int?>())).Returns(variableSymbol);
            const string columnName = "col1";

            // Act

            this.primitives.SelectIdentity(columnName);
            this.primitives.Execute();

            // Assert

            string expectedText =
                new StringBuilder($"declare @{variableSymbol} bigint;").AppendLine()
                    .AppendLine($"select @{variableSymbol} = @@identity;")
                    .AppendLine($"select '{columnName}'")
                    .AppendLine($"select @{variableSymbol}")
                    .AppendLine()
                    .ToString();

            insertCommandMock.VerifySet(m => m.CommandText = expectedText);
        }

        [TestMethod]
        public void WriteGuid_Test()
        {
            throw new NotImplementedException();
        }
    }
}
