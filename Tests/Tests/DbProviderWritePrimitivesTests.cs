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
    public class DbProviderWritePrimitivesTests
    {
        private class WritePrimitives : DbProviderWritePrimitives
        {
            public WritePrimitives(string connectionStringWithDefaultCatalogue, DbProviderFactory dbProviderFactory,
                IValueFormatter formatter, bool mustBeInATransaction, NameValueCollection configuration)
                : base(
                    connectionStringWithDefaultCatalogue, dbProviderFactory, formatter, mustBeInATransaction,
                    configuration)
            {
            }

            public override object SelectIdentity(string columnName)
            {
                throw new NotImplementedException();
            }

            public override object WriteGuid(string columnName)
            {
                throw new NotImplementedException();
            }
        }

        private Mock<DbProviderFactory> dbProviderFactoryMock;
        private WritePrimitives primitives;
        private Mock<IValueFormatter> formatterMock;
        private Mock<DbCommand> insertCommandMock;
        private Mock<DbDataReader> readerMock;
        private Mock<DbConnection> connectionMock;

        private const string ConnectionString = "cn";

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.dbProviderFactoryMock = new Mock<DbProviderFactory>();
            this.formatterMock = new Mock<IValueFormatter>();

            this.primitives = new WritePrimitives(DbProviderWritePrimitivesTests.ConnectionString,
                this.dbProviderFactoryMock.Object, this.formatterMock.Object,
                mustBeInATransaction: false,
                configuration: new NameValueCollection {{"TestDataFramework_DumpSqlInput", "true"}});

            this.connectionMock = new Mock<DbConnection>();
            this.insertCommandMock = new Mock<DbCommand>();
            this.readerMock = new Mock<DbDataReader>();
            var mockInsertCommand = new MockDbCommand(this.insertCommandMock.Object, this.readerMock.Object);

            this.dbProviderFactoryMock.Setup(m => m.CreateCommand()).Returns(mockInsertCommand);
            this.dbProviderFactoryMock.Setup(m => m.CreateConnection()).Returns(this.connectionMock.Object);
        }

        [TestMethod]
        public void Execute_Test()
        {
            // Arrange

            // Setup has rows flag to return 2 row for 1st record set and 1 row for second.
            var hasRows = new[] {true, true, false, true, false};
            int hasRowsPosition = 0;

            this.readerMock.Setup(m => m.HasRows).Returns(() => hasRows[hasRowsPosition++]);

            // Setup reader to signal 2 rows in first result set and 1 row in second set.
            var read = new[] {true, true, false, true, false};
            int readPosition = 0;

            this.readerMock.Setup(m => m.Read()).Returns(() => read[readPosition++]);

            // Three rows setup (note: in two result sets), 1st with 2 coluns back, 2nd with 3 columns and 3rd with 2 columns.
            var fieldCount = new[] {2, 3, 2};
            int fieldCountPosition = 0;

            this.readerMock.Setup(m => m.FieldCount).Returns(() => fieldCount[fieldCountPosition++]);

            var expected = new[]
            {
                new object[] {"A", 1},
                new object[] {"B", 2, 3.5d},
                new object[] {"C", 'K'},
            };

            int rowNumber = 0;

            this.readerMock.Setup(m => m.GetValues(It.IsAny<object[]>())).Callback<object[]>(a =>
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

            this.insertCommandMock.VerifySet(m => m.CommandType = CommandType.Text);
            this.connectionMock.VerifySet(m => m.ConnectionString = DbProviderWritePrimitivesTests.ConnectionString);
            this.connectionMock.Verify(m => m.Open());

            Assert.AreEqual(7, results.Length);

            Assert.AreEqual(expected[0][0], results[0]);
            Assert.AreEqual(expected[0][1], results[1]);
            Assert.AreEqual(expected[1][0], results[2]);
            Assert.AreEqual(expected[1][1], results[3]);
            Assert.AreEqual(expected[1][2], results[4]);
            Assert.AreEqual(expected[2][0], results[5]);
            Assert.AreEqual(expected[2][1], results[6]);
        }

        [TestMethod]
        public void Insert_Test()
        {
            // Arrange

            var columns = new[]
            {
                new Column { Name = "Row1", Value = 1},
                new Column { Name = "Row2", Value = "B"},
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
                new StringBuilder("insert into [xx] ([Row1], [Row2]) values (1st Value, 2nd Value);").AppendLine()
                    .AppendLine()
                    .ToString();

            this.insertCommandMock.VerifySet(m => m.CommandText = expectedText);
        }

        [TestMethod]
        public void NotInATransactionException_Test()
        {
            this.primitives = new WritePrimitives(null,
                null, null,
                mustBeInATransaction: true,
                configuration: null);

            Helpers.ExceptionTest(() => this.primitives.Execute(), typeof(NotInATransactionException), Messages.NotInATransaction);
        }

        [TestMethod]
        public void AddSqlCommand_Test()
        {
            this.primitives.AddSqlCommand("ABCD");
            this.primitives.Execute();

            this.insertCommandMock.VerifySet(m => m.CommandText = "ABCD\r\n\r\n");
        }

        [TestMethod]
        public void PrimitivesClearedAfterExecute_Test()
        {
            this.primitives.AddSqlCommand("ABCD");
            this.primitives.Execute();

            this.primitives.AddSqlCommand("XYZR");
            this.primitives.Execute();

            this.insertCommandMock.VerifySet(m => m.CommandText = "ABCD\r\n\r\n");
            this.insertCommandMock.VerifySet(m => m.CommandText = "XYZR\r\n\r\n");
        }
    }
}
