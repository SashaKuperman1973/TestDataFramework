/*
    Copyright 2016, 2017 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CommonIntegrationTests;
using CommonIntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace DeclarativeIntegrationTests.Tests
{
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

        [TestMethod]
        public void Memory_Declarative_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator(defaultSchema: "dbo");
            SqlClientAndMemoryTests.PrimaryKeyForeignKeyTest(populator, new DeclarativeGeneratorIntegrationTest());
        }

        [Ignore]
        [TestMethod]
        public void SqlCient_Declarative_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;");

            SqlClientAndMemoryTests.PrimaryKeyForeignKeyTest(populator, new DeclarativeGeneratorIntegrationTest());
        }

        private static void PrimaryKeyForeignKeyTest(IPopulator populator,
            ICodeGeneratorIntegration codeGeneratorIntegration)
        {
            IList<RecordReference<ManualKeyPrimaryTableClass>> primaries = populator.Add<ManualKeyPrimaryTableClass>(2);

            IList<RecordReference<ManualKeyForeignTable>> foreignSet1 = populator.Add<ManualKeyForeignTable>(2);
            foreignSet1.ToList().ForEach(f => f.AddPrimaryRecordReference(primaries[0]));

            IList<RecordReference<ManualKeyForeignTable>> foreignSet2 =
                populator.Add<ManualKeyForeignTable>(2, primaries[1]);

            codeGeneratorIntegration.AddTypes(populator, foreignSet1, foreignSet2);

            primaries[0].Set(o => o.ADecimal, 112233.445566m).Set(o => o.AString, "AAXX")
                .Set(o => o.Key1, "HummHummHumm");

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
            codeGeneratorIntegration.Dump();

            Console.WriteLine();

            Console.WriteLine(foreignSet1[0].RecordObject.ForeignKey1);
            Console.WriteLine(foreignSet1[1].RecordObject.ForeignKey1);
            Console.WriteLine(foreignSet2[0].RecordObject.ForeignKey1);
            Console.WriteLine(foreignSet2[1].RecordObject.ForeignKey1);
        }

        [Ignore]
        [TestMethod]
        public void EmptyForeignReference_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;");

            RecordReference<UnresolvedKeyTable> unresolvedKeyTableRecord = populator.Add<UnresolvedKeyTable>();
            unresolvedKeyTableRecord.Set(p => p.DoesntExist, (int?) null);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted}))
            {
                populator.Bind();
                //transaction.Complete();
            }
        }
    }
}