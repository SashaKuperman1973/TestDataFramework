using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator;

namespace IntegrationTests.Tests
{
    [TestClass]
    public class SqlClientTest
    {
        private PopulatorFactory factory;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.factory = new PopulatorFactory();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.factory.Dispose();
        }

        [TestMethod]
        public void Run_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=.\SqlExpress;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                mustBeInATransaction: false);

            populator.Add<SubjectClass>();

            populator.Bind();
        }
    }
}
