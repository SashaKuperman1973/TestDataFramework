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

using IntegrationTests.CommonIntegrationTests;
using IntegrationTests.CommonIntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace IntegrationTests.DeclarativeIntegrationTests.Tests
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
        public void Memory_Declarative_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator(defaultSchema: "dbo");
            SqlClientAndMemoryTests.PrimaryKeyForeignKeyTest(populator, new DeclarativeGeneratorIntegrationTest(),
                () => populator.Bind());
        }

        [TestMethod]
        public void ComnplexChildValueTest()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator(defaultSchema: "dbo");

            var parentReference = populator.Add<ManualParentTable>();
            var childValueReferences = parentReference.SetList(parent => parent.ChildValues, 7)
                .Set(child => child.InChildFieldA, 3);
            populator.Bind();

            Console.WriteLine(childValueReferences[0].RecordObject.Key);
            Console.WriteLine(childValueReferences[0].RecordObject.InChildFieldA);
            Console.WriteLine(childValueReferences[0].RecordObject.InChildFieldB);

            Assert.IsTrue(childValueReferences[0].RecordObject.Key > 0);
            Assert.AreEqual(3, childValueReferences[0].RecordObject.InChildFieldA);

            Console.WriteLine(parentReference.RecordObject.ChildValues[1].Key);
            Console.WriteLine(parentReference.RecordObject.ChildValues[1].InChildFieldA);
            Console.WriteLine(parentReference.RecordObject.ChildValues[1].InChildFieldB);

            Assert.IsTrue(parentReference.RecordObject.ChildValues[1].Key > 0);
            Assert.AreEqual(3, parentReference.RecordObject.ChildValues[1].InChildFieldA);
        }

        [TestMethod]
        public void SetListTest()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator(defaultSchema: "dbo");

            const int setSize = 3;
            var foreignCollection = populator.Add<SetCollectionTable>().SetList(setCollectionTable => setCollectionTable.ForeignSet, setSize);
            var primaryCollection = populator.Add<SetPrimaryTable>(setSize);

            for (int i = 0; i < setSize; i++)
            {
                foreignCollection[i].AddPrimaryRecordReference(primaryCollection[i]);
            }

            populator.Bind();

            Console.WriteLine("Primary:");
            primaryCollection.ToList().ForEach(primary => Console.WriteLine(primary.RecordObject.Key));

            Console.WriteLine();

            Console.WriteLine("Foreign:");
            foreignCollection.ToList().ForEach(foreign => Console.WriteLine(foreign.RecordObject.Foreign));

            Assert.AreEqual(primaryCollection.Count, foreignCollection.Count);
            
            for (int i=0; i< primaryCollection.Count;i++)
                Assert.AreEqual(primaryCollection[i].RecordObject.Key, foreignCollection[i].RecordObject.Foreign);
        }

        [TestMethod]
        public void TestMethod()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator(defaultSchema: "dbo");

            const int setSize = 3;
            var targetNested2 = populator.Add<BaseNestedTable>().SetList(baseNested => baseNested.Target1.TargetNestedTable2, setSize);
            targetNested2.Set(nested2 => nested2.Boolean).GuaranteePropertiesByFixedQuantity(new[] {true, false});

            populator.Bind();

            targetNested2.ToList().ForEach(nested2 => Console.WriteLine(nested2.RecordObject.Boolean));
        }

        [TestMethod]
        public void DeepSetListTest()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator(defaultSchema: "dbo");

            const int setSize = 3;

            var baseNested = populator.Add<BaseNestedTable>();

            RecordReference<TargetNestedTable2>[] targetNested2Collection = baseNested.SetList(baseNestedTable => baseNestedTable.Target1.TargetNestedTable2, setSize).ToArray();

            var nested2Collection = baseNested.SetList(baseNestedTable => baseNestedTable.Nested1.NestedTable2, setSize);

            var outerNested3Colllection = new List<RecordReference<NestedTable3>>();

            foreach (RecordReference<NestedTable2> nested2 in nested2Collection)
            {
                RecordReference<NestedTable3>[] nested3Colllection = nested2.SetList(nestedTable2 => nestedTable2.NestedTable3, setSize).ToArray();
                outerNested3Colllection.AddRange(nested3Colllection);

                for (int i=0; i < setSize; i++)
                    nested3Colllection[i].AddPrimaryRecordReference(targetNested2Collection[i]);
            }

            populator.Bind();

            outerNested3Colllection.ForEach(nested3 => Console.WriteLine(nested3.RecordObject.Id));

            int j = 0;
            outerNested3Colllection.ForEach(nested3 => Assert.AreEqual(targetNested2Collection[j++ % 3].RecordObject.Id, nested3.RecordObject.Id));
        }

#if !DBWRITE
        [Ignore]
#endif
        [TestMethod]
        public void SqlCient_Declarative_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;");

            SqlClientAndMemoryTests.RunPrimaryKeyForeignKeyTest(populator);
        }

#if !DBWRITE
        [Ignore]
#endif
        [TestMethod]
        public void SqlCient_Declarative_Test_With_Deletion()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;");

            SqlClientAndMemoryTests.RunPrimaryKeyForeignKeyTest(populator);

            populator.DeleteAll();
        }

        private static void RunPrimaryKeyForeignKeyTest(IPopulator populator)
        {
            SqlClientAndMemoryTests.PrimaryKeyForeignKeyTest(populator, new DeclarativeGeneratorIntegrationTest(), () =>
            {
                using (IDbClientTransaction transaction = populator.BindInATransaction())
                {
                    transaction.Commit();
                }
            });
        }

        private static void PrimaryKeyForeignKeyTest(IPopulator populator,
            ICodeGeneratorIntegration codeGeneratorIntegration, Action bind)
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

            bind();

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

#if !DBWRITE
        [Ignore]
#endif
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
                transaction.Complete();
            }
        }
    }
}