using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;
using Tests.Mocks;

namespace Tests.Tests
{
    [TestClass]
    public class DbProviderWritePrimitivesTests
    {
        private Mock<DbProviderFactory> dbProviderFactoryMock;
        private DbProviderWritePrimitives primitives;
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

            this.primitives = new DbProviderWritePrimitives(DbProviderWritePrimitivesTests.ConnectionString,
                this.dbProviderFactoryMock.Object, this.formatterMock.Object, this.symbolGeneratorMock.Object,
                mustBeInATransaction: false,
                configuration: new NameValueCollection {{"TestDataFramework_DumpSqlInput", "true"}});
        }

        [TestMethod]
        public void Execute_Test()
        {
            // Arrange

            var connectionMock = new Mock<DbConnection>();
            var insertCommandMock = new Mock<DbCommand>();
            var readerMock = new Mock<DbDataReader>();
            var mockInsertCommand = new MockDbCommand(insertCommandMock.Object, readerMock.Object);

            this.dbProviderFactoryMock.Setup(m => m.CreateCommand()).Returns(mockInsertCommand);
            this.dbProviderFactoryMock.Setup(m => m.CreateConnection()).Returns(connectionMock.Object);

            // Setup reader to return true twice then false.
            readerMock.Setup(m => m.Read())
                .Returns(true)
                .Callback(
                    () =>
                        readerMock.Setup(m => m.Read())
                            .Returns(true)
                            .Callback(() => readerMock.Setup(m => m.Read()).Returns(false)));

            // Two rows setup, 1st with 2 coluns back and 2nd with 3 column back.
            readerMock.Setup(m => m.FieldCount)
                .Returns(2)
                .Callback(() => readerMock.Setup(m => m.FieldCount).Returns(3));

            var expected = new[]
            {
                new object[] {"A", 1},
                new object[] {"B", 2, 3.5d}
            };

            int rowNumber = 0;

            readerMock.Setup(m => m.GetValues(It.IsAny<object[]>())).Callback<object[]>(a =>
            {
                for (int i = 0; i < a.Length; i++)
                {
                    a[i] = expected[rowNumber][i];
                }

                rowNumber++;
            });

            // Act

            object[] results = this.primitives.Execute();

            // Assert

            insertCommandMock.VerifySet(m => m.CommandType = CommandType.Text);
            connectionMock.VerifySet(m => m.ConnectionString = DbProviderWritePrimitivesTests.ConnectionString);
            connectionMock.Verify(m => m.Open());

            Assert.AreEqual(5, results.Length);

            Assert.AreEqual(expected[0][0], results[0]);
            Assert.AreEqual(expected[0][1], results[1]);
            Assert.AreEqual(expected[1][0], results[2]);
            Assert.AreEqual(expected[1][1], results[3]);
            Assert.AreEqual(expected[1][2], results[4]);
        }

        [TestMethod]
        public void Insert_Test()
        {
            // Arrange

            var connectionMock = new Mock<DbConnection>();
            var insertCommandMock = new Mock<DbCommand>();
            var readerMock = new Mock<DbDataReader>();
            var mockInsertCommand = new MockDbCommand(insertCommandMock.Object, readerMock.Object);

            this.dbProviderFactoryMock.Setup(m => m.CreateCommand()).Returns(mockInsertCommand);
            this.dbProviderFactoryMock.Setup(m => m.CreateConnection()).Returns(connectionMock.Object);

            const string variableSymbol = "ABCD";

            var columns = new[]
            {
                new Column { Name = "Row1", Value = 1},
                new Column { Name = "Row2", Value = "B"},
                new Column { Name = "RowWithVariable", Value = new Variable(variableSymbol)}
            };

            this.formatterMock.Setup(m => m.Format(columns[0].Value)).Returns("1st Value");
            this.formatterMock.Setup(m => m.Format(columns[1].Value)).Returns("2nd Value");

            const string tableName = "xx";

            // Act

            this.primitives.Insert(tableName, columns);
            this.primitives.Execute();

            // Assert

            this.formatterMock.Verify();

            string expectedText =
                new StringBuilder("insert into [xx] ([Row1], [Row2], [RowWithVariable]) values (1st Value, 2nd Value, @ABCD);").AppendLine()
                    .AppendLine()
                    .ToString();

            insertCommandMock.VerifySet(m => m.CommandText = expectedText);
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

            // Act

            this.primitives.SelectIdentity();
            this.primitives.Execute();

            // Assert

            string expectedText =
                new StringBuilder($"declare @{variableSymbol} bigint;").AppendLine()
                    .AppendLine($"select @{variableSymbol} = @@identity;")
                    .AppendLine()
                    .ToString();

            insertCommandMock.VerifySet(m => m.CommandText = expectedText);
        }
    }
}
