using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Persistence.Concrete;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.DbClientPopulator;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.WritePrimitives;

namespace Tests.Tests
{
    [TestClass]
    public class SqlClientPopulatorTests
    {
        private StandardDbClientPopulator sqlClientPopulator;

        private Mock<DbProviderWritePrimitives> writePrimitivesMock;
        private Mock<SqlClientPersistence> persistenceMock;

        [TestInitialize]
        public void Initialize()
        {
            this.writePrimitivesMock = Helpers.GetMock<DbProviderWritePrimitives>();
            this.persistenceMock = Helpers.GetMock<SqlClientPersistence>();

            this.sqlClientPopulator = new StandardDbClientPopulator(
                typeGenerator: null,
                persistence: this.persistenceMock.Object,
                attributeDecorator: null,
                handledTypeGenerator: null,
                valueGenerator: null,
                valueGuaranteePopulator: null,
                objectGraphService: null,
                deepCollectionSettingConverter: null
                );

            this.persistenceMock.Setup(m => m.UseTransaction(It.IsAny<DbClientTransaction>()))
                .Callback<DbClientTransaction>(transaction => transaction.DbTransaction = new Mock<DbTransaction>().Object);
        }

        [TestMethod]
        public void BindInATransaction_Default_Returns_A_Transaction_Test()
        {
            // Act

            using (IDbClientTransaction transaction = this.sqlClientPopulator.BindInATransaction())
            {
                // Assert

                var sqlTransaction = transaction as DbClientTransaction;

                Assert.IsNotNull(sqlTransaction);
                Assert.IsNotNull(sqlTransaction.Options);

                Assert.AreEqual(sqlTransaction.Options.IsolationLevel, IsolationLevel.ReadCommitted);
            }
        }

        [TestMethod]
        public void BindInATransaction_With_Options_Returns_A_Transaction_Test()
        {
            // Arrange

            var options = new DbClientTransactionOptions();
            this.sqlClientPopulator.SetTransationOptions(options);

            // Act

            using (IDbClientTransaction transaction = this.sqlClientPopulator.BindInATransaction())
            {
                var sqlTransaction = transaction as DbClientTransaction;

                // Assert

                Assert.IsNotNull(sqlTransaction);
                Assert.IsNotNull(sqlTransaction.Options);

                Assert.AreEqual(options, sqlTransaction.Options);

                Assert.AreEqual(sqlTransaction.Options.IsolationLevel, IsolationLevel.ReadCommitted);
            }
        }
    }
}
