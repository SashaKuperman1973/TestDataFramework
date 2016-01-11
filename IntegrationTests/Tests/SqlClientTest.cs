using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator;
using Tests.TestModels;

namespace IntegrationTests.Tests
{
    //[Ignore]
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

        //[Ignore]
        [TestMethod]
        public void SubjectClass_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=.\SqlExpress;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                mustBeInATransaction: false);

            IList<RecordReference<SubjectClass>> result = populator.Add<SubjectClass>(2);

            populator.Bind();

            Console.WriteLine(result[0].RecordObject.Key);
            Console.WriteLine(result[1].RecordObject.Key);
        }

        //[Ignore]
        [TestMethod]
        public void ManualKeyPrimaryTable_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=.\SqlExpress;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                mustBeInATransaction: false);

            IList<RecordReference<ManualKeyPrimaryTable>> result = populator.Add<ManualKeyPrimaryTable>(60);

            populator.Bind();

            Console.WriteLine(result[0].RecordObject.Key1);
            Console.WriteLine(result[0].RecordObject.Key2);

            Console.WriteLine(result[1].RecordObject.Key1);
            Console.WriteLine(result[1].RecordObject.Key2);
        }
    }
}
