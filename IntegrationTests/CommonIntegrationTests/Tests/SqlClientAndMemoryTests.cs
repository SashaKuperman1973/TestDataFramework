/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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
using System.Data;
using System.Linq;
using IntegrationTests.CommonIntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.DbClientPopulator;
using TestDataFramework.Populator.Concrete.OperableList;
using TestDataFramework.Populator.Interfaces;

namespace IntegrationTests.CommonIntegrationTests.Tests
{
    [TestClass]
    public class SqlClientAndMemoryTests
    {
        private PopulatorFactory factory;

        public SqlClientAndMemoryTests()
        {
            XmlConfigurator.Configure();
        }

        [TestInitialize]
        public void Initialize()
        {
            this.factory = new PopulatorFactory();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.factory.Dispose();
        }

        [TestMethod]
        public void CompositeMultiple_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            OperableListEx<TestPrimaryTable> primaryA = populator.Add<TestPrimaryTable>(2);
            OperableListEx<PrimaryTableB> primaryB = populator.Add<PrimaryTableB>(2);

            OperableListEx<ForeignTable> foreignA = populator.Add<ForeignTable>(2, primaryA[0], primaryB[0]);

            OperableListEx<ForeignTable> foreignB = populator.Add<ForeignTable>(2);
            foreignB.ToList().ForEach(foreign => foreign.AddPrimaryRecordReference(primaryA[1], primaryB[1]));

            RecordReference<ForeignTable> foreignC = populator.Add<ForeignTable>(primaryA[1], primaryB[0]);

            populator.Bind();

            Console.WriteLine("Here");
        }

        [TestMethod]
        public void Make_With_Reference_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            OperableListEx<TestPrimaryTable> primary = populator.Add<TestPrimaryTable>(2);

            populator.Bind();

            List<ForeignTable> foreignA = populator.Add<ForeignTable>(2, primary[0]).Make().ToList();
            List<ForeignTable> foreignB = populator.Add<ForeignTable>(2, primary[1]).Make().ToList();

            primary.ToList().ForEach(element =>
            {
                Console.WriteLine(element.RecordObject.Key1);
                Console.WriteLine(element.RecordObject.Key2);
            });

            Console.WriteLine(foreignA[0].ForeignKeyA1);
            Console.WriteLine(foreignA[0].ForeignKeyA2);
            Console.WriteLine(foreignA[1].ForeignKeyA1);
            Console.WriteLine(foreignA[1].ForeignKeyA2);

            Console.WriteLine(foreignB[0].ForeignKeyA1);
            Console.WriteLine(foreignB[0].ForeignKeyA2);
            Console.WriteLine(foreignB[1].ForeignKeyA1);
            Console.WriteLine(foreignB[1].ForeignKeyA2);
        }

#if !DBWRITE
        [Ignore]
#endif
        [TestMethod]
        public void Explicit_ForeignKeyToPrimaryKey_Assignment_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                false);

            //IPopulator populator = this.factory.CreateMemoryPopulator();

            var manualKeyPrimaryTableRecords = populator.Add<ManualKeyPrimaryTableClass>(2);
            var primaryTableRecords = populator.Add<TestPrimaryTable>(2);

            var foreignTable = populator.Add<MultiPrimaryRowForeignTable>(primaryTableRecords[1]);

            foreignTable.AddPrimaryRecordReference(manualKeyPrimaryTableRecords[0], p => p.ForeignKey1Alpha);
            foreignTable.AddPrimaryRecordReference(manualKeyPrimaryTableRecords[0], p => p.ForeignKey2Alpha);
            foreignTable.AddPrimaryRecordReference(manualKeyPrimaryTableRecords[0], p => p.TesterForeignKeyAlpha);

            foreignTable.AddPrimaryRecordReference(manualKeyPrimaryTableRecords[1], p => p.ForeignKey1Beta);
            foreignTable.AddPrimaryRecordReference(manualKeyPrimaryTableRecords[1], p => p.ForeignKey2Beta);
            foreignTable.AddPrimaryRecordReference(manualKeyPrimaryTableRecords[1], p => p.TesterForeignKeyBeta);

            populator.Bind();

            Assert.AreEqual(manualKeyPrimaryTableRecords[0].RecordObject.Tester, foreignTable.RecordObject.TesterForeignKeyAlpha);
            Assert.AreEqual(manualKeyPrimaryTableRecords[0].RecordObject.Key1, foreignTable.RecordObject.ForeignKey1Alpha);
            Assert.AreEqual(manualKeyPrimaryTableRecords[0].RecordObject.Key2, foreignTable.RecordObject.ForeignKey2Alpha);

            Assert.AreEqual(manualKeyPrimaryTableRecords[1].RecordObject.Tester, foreignTable.RecordObject.TesterForeignKeyBeta);
            Assert.AreEqual(manualKeyPrimaryTableRecords[1].RecordObject.Key1, foreignTable.RecordObject.ForeignKey1Beta);
            Assert.AreEqual(manualKeyPrimaryTableRecords[1].RecordObject.Key2, foreignTable.RecordObject.ForeignKey2Beta);

            Assert.AreEqual(primaryTableRecords[1].RecordObject.Key1, foreignTable.RecordObject.ForeignToPrimaryKey1);
            Assert.AreEqual(primaryTableRecords[1].RecordObject.Key2, foreignTable.RecordObject.ForeignToPrimaryKey2);

        }

#if !DBWRITE
        [Ignore]
#endif
        [TestMethod]
        public void ManualKeyPrimaryTable_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                false);

            IList<RecordReference<ManualKeyPrimaryTableClass>> result = populator.Add<ManualKeyPrimaryTableClass>(5);

            populator.Bind();

            Console.WriteLine(result[0].RecordObject.Key1);
            Console.WriteLine(result[0].RecordObject.Key2);

            Console.WriteLine(result[1].RecordObject.Key1);
            Console.WriteLine(result[1].RecordObject.Key2);
        }

#if !DBWRITE
        [Ignore]
#endif
        [TestMethod]
        public void SubjectClass_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                false);

            Guid g = Guid.NewGuid();

            IList<RecordReference<SqlSubjectClass>> result = populator.Add<SqlSubjectClass>(2);

            SqlClientAndMemoryTests.Go(result, g);

            populator.Bind();

            Console.WriteLine(g);

            Console.WriteLine(result[0].RecordObject.GuidKey);
            Console.WriteLine(result[1].RecordObject.GuidKey);
        }

        private static void Go<T>(IList<RecordReference<T>> x, Guid g) where T : IGuider
        {
            x.ToList().ForEach(y => y.Set(p => p.GuidKey, g));
        }

#if !DBWRITE
        [Ignore]
#endif
        [TestMethod]
        public void Guid_Test()
        {
            IPopulator populator = true
                ? this.factory.CreateSqlClientPopulator(
                    @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                    mustBeInATransaction: false)
                : this.factory.CreateMemoryPopulator();

            OperableListEx<SqlSubjectClass> result = populator.Add<SqlSubjectClass>(2);
            populator.Bind();
        }

#if !DBWRITE
        [Ignore]
#endif
        [TestMethod]
        public void StringWithQuotes_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                mustBeInATransaction: false);

            RecordReference<SqlSubjectClass> result = populator.Add<SqlSubjectClass>().Set(p => p.Text, "--'AB''CD'");
            populator.Bind();
        }

#if !DBWRITE
        [Ignore]
#endif
        [TestMethod]
        public void StringWithoutQuotes_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                mustBeInATransaction: false);

            RecordReference<SqlSubjectClass> result = populator.Add<SqlSubjectClass>().Set(p => p.Text, "ABCD");
            populator.Bind();
        }
    }
}