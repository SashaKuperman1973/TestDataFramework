using System;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;

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
            this.dbProviderFactoryMock = new Mock<DbProviderFactory>();
            this.formatterMock = new Mock<IValueFormatter>();
            this.symbolGeneratorMock = new Mock<IRandomSymbolStringGenerator>();

            this.primitives = new DbProviderWritePrimitives(DbProviderWritePrimitivesTests.ConnectionString,
                this.dbProviderFactoryMock.Object, this.formatterMock.Object, this.symbolGeneratorMock.Object);
        }

        [TestMethod]
        public void Insert_Test()
        {
            // Arrange

            var commandBuilderMock = new Mock<DbCommandBuilder>();
            this.dbProviderFactoryMock.Setup(m => m.CreateCommandBuilder()).Returns(commandBuilderMock.Object);
            var insertCommandMock = new Mock<DbCommand>();
            commandBuilderMock.Setup(m => m.GetInsertCommand()).Returns(insertCommandMock.Object);

            var columns = new[]
            {
                new Column { Name = "Row1", Value = 1},
                new Column { Name = "Row2", Value = "B"},
            };

            const string tableName = "xx";

            // Act

            this.primitives.Insert(tableName, columns);
            object[] results = this.primitives.Execute();

            // Assert

            insertCommandMock.VerifySet(m => m.CommandType = CommandType.Text);
            insertCommandMock.VerifySet(m => m.CommandText = "insert into xx ([Row1], [Row2]) values (1, 'B')");
        }
    }
}
