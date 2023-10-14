/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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
using IntegrationTests.CommonIntegrationTests;
using IntegrationTests.CommonIntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace IntegrationTests.PocoIntegrationTests.Tests
{
    [TestClass]
    public class SqlClientAndMemoryTests
    {
        private PopulatorFactory factory;

        [TestInitialize]
        public void Initialize()
        {
            global::Tests.Helpers.ConfigureLogger();

            this.factory = new PopulatorFactory();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.factory.Dispose();
        }

        [TestMethod]
        public void Memory_POCO_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator(defaultSchema: "dbo");
            SqlClientAndMemoryTests.PrimaryKeyForeignKeyTest(populator, new PocoGeneratorIntegrationTest());
        }

#if !DBWRITE
        [Ignore]
#endif
        [TestMethod]
        public void SqlCient_POCO_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Integrated Security=SSPI;");

            SqlClientAndMemoryTests.PrimaryKeyForeignKeyTest(populator, new PocoGeneratorIntegrationTest());
        }

        private static void PrimaryKeyForeignKeyTest(IPopulator populator,
            ICodeGeneratorIntegration codeGeneratorIntegration)
        {
            IList<RecordReference<ManualKeyPrimaryTableClass>> primaries = populator.Add<ManualKeyPrimaryTableClass>(2);

            IList<RecordReference<ManualKeyForeignTable>> foreignSet1 = populator.Add<ManualKeyForeignTable>(2, primaries[0]);

            IList<RecordReference<ManualKeyForeignTable>> foreignSet2 = populator.Add<ManualKeyForeignTable>(2);
            foreignSet2.ToList().ForEach(f => f.AddPrimaryRecordReference(primaries[1]));

            codeGeneratorIntegration.AddTypes(populator, foreignSet1, foreignSet2);

            primaries[0].Set(o => o.ADecimal, 112233.445566m).Set(o => o.AString, "AAXX")
                .Set(o => o.Key1, "HummHummHumm");

            foreignSet2[1].Set(o => o.ALong, 11111L).Set(o => o.AShort, (short) 1234);

            using (var transactionScope =
                new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted}))
            {
                populator.Bind();

                transactionScope.Complete();
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
    }
}