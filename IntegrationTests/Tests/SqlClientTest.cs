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

        [Ignore]
        [TestMethod]
        public void SubjectClass_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=.\SqlExpress;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                mustBeInATransaction: false);

            IList<RecordReference<SubjectClass>> result = populator.Add<SubjectClass>(2);

            populator.Bind();

            Console.WriteLine(result[0].RecordObject.Key);
            Console.WriteLine(result[0].RecordObject.GuidKey);

            Console.WriteLine(result[1].RecordObject.Key);
            Console.WriteLine(result[1].RecordObject.GuidKey);
        }

        [Ignore]
        [TestMethod]
        public void ManualKeyPrimaryTable_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=.\SqlExpress;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                mustBeInATransaction: false);

            IList<RecordReference<ManualKeyPrimaryTable>> result = populator.Add<ManualKeyPrimaryTable>(5);

            populator.Bind();

            Console.WriteLine(result[0].RecordObject.Key1);
            Console.WriteLine(result[0].RecordObject.Key2);

            Console.WriteLine(result[1].RecordObject.Key1);
            Console.WriteLine(result[1].RecordObject.Key2);
        }

        [TestMethod]
        public void PrimaryKeyForeignKey_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=.\SqlExpress;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                mustBeInATransaction: false);

            IList<RecordReference<ManualKeyPrimaryTable>> primaries = populator.Add<ManualKeyPrimaryTable>(2);

            IList<RecordReference<ManualKeyForeignTable>> foreignSet1 = populator.Add<ManualKeyForeignTable>(2);
            foreignSet1.ToList().ForEach(f => f.AddPrimaryRecordReference(primaries[0]));

            IList<RecordReference<ManualKeyForeignTable>> foreignSet2 = populator.Add<ManualKeyForeignTable>(2);
            foreignSet2.ToList().ForEach(f => f.AddPrimaryRecordReference(primaries[1]));

            IList<RecordReference<TertiaryManualKeyForeignTable>> tertiaryForeignSet =
                populator.Add<TertiaryManualKeyForeignTable>(4);

            tertiaryForeignSet[0].AddPrimaryRecordReference(foreignSet1[0]);
            tertiaryForeignSet[1].AddPrimaryRecordReference(foreignSet1[1]);
            tertiaryForeignSet[2].AddPrimaryRecordReference(foreignSet2[0]);
            tertiaryForeignSet[3].AddPrimaryRecordReference(foreignSet2[1]);

            populator.Bind();
        }
    }
}
