using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;
using Tests.TestModels;

namespace IntegrationTests.Tests
{
    [Ignore]
    [TestClass]
    public class SqlClientAndMemoryTests
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

        [Ignore]
        [TestMethod]
        public void Memory_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();
            SqlClientAndMemoryTests.PrimaryKeyForeignKeyTest(populator);
        }

        [Ignore]
        [TestMethod]
        public void SqlCient_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=.\SqlExpress;Initial Catalog=TestDataFramework;Integrated Security=SSPI;");

            SqlClientAndMemoryTests.PrimaryKeyForeignKeyTest(populator);
        }

        private static void PrimaryKeyForeignKeyTest(IPopulator populator)
        {
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

            IList<RecordReference<ForeignToAutoPrimaryTable>> foreignToAutoSet = populator.Add<ForeignToAutoPrimaryTable>(2);

            foreignToAutoSet[0].AddPrimaryRecordReference(tertiaryForeignSet[0]);
            foreignToAutoSet[1].AddPrimaryRecordReference(tertiaryForeignSet[1]);

            primaries[0].Set(o => o.ADecimal, 112233.445566m).Set(o => o.AString, "AAXX").Set(o => o.Key1, "HummHummHumm");

            foreignSet2[1].Set(o => o.ALong, 11111L).Set(o => o.AShort, (short) 1234);

            using (var transactionScope =
                new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted}))
            {
                populator.Bind();
                
                //transactionScope.Complete();
            }

            Helpers.Dump(primaries);
            Helpers.Dump(foreignSet1);
            Helpers.Dump(foreignSet2);
            Helpers.Dump(tertiaryForeignSet);
            Helpers.Dump(foreignToAutoSet);

            Console.WriteLine();

            Console.WriteLine(foreignToAutoSet[0].RecordObject.ForignKey);
            Console.WriteLine(foreignToAutoSet[1].RecordObject.ForignKey);
            Console.WriteLine(foreignSet1[0].RecordObject.ForeignKey1);
            Console.WriteLine(foreignSet1[1].RecordObject.ForeignKey1);
            Console.WriteLine(foreignSet2[0].RecordObject.ForeignKey1);
            Console.WriteLine(foreignSet2[1].RecordObject.ForeignKey1);
        }
    }
}
