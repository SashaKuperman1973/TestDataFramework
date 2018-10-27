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

            using (DbClientTransaction transaction = this.sqlClientPopulator.BindInATransaction())
            {
                // Assert

                Assert.IsNotNull(transaction);
                Assert.IsNotNull(transaction.Options);

                Assert.AreEqual(transaction.Options.IsolationLevel, IsolationLevel.ReadCommitted);
            }
        }

        [TestMethod]
        public void BindInATransaction_With_Options_Returns_A_Transaction_Test()
        {
            // Arrange

            var options = new DbClientTransactionOptions();

            // Act

            using (DbClientTransaction transaction = this.sqlClientPopulator.BindInATransaction(options))
            {

                // Assert

                Assert.IsNotNull(transaction);
                Assert.IsNotNull(transaction.Options);

                Assert.AreEqual(options, transaction.Options);

                Assert.AreEqual(transaction.Options.IsolationLevel, IsolationLevel.ReadCommitted);
            }
        }
    }
}
