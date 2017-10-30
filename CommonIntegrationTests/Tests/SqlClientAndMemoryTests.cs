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
using CommonIntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace CommonIntegrationTests.Tests
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

            OperableList<PrimaryTable> primaryA = populator.Add<PrimaryTable>(2);
            OperableList<PrimaryTableB> primaryB = populator.Add<PrimaryTableB>(2);

            OperableList<ForeignTable> foreignA = populator.Add<ForeignTable>(2, primaryA[0], primaryB[0]);

            OperableList<ForeignTable> foreignB = populator.Add<ForeignTable>(2);
            foreignB.ToList().ForEach(foreign => foreign.AddPrimaryRecordReference(primaryA[1], primaryB[1]));

            RecordReference<ForeignTable> foreignC = populator.Add<ForeignTable>(primaryA[1], primaryB[0]);

            populator.Bind();
            
            Console.WriteLine("Here");
        }

        [TestMethod]
        public void Make_With_Reference_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            OperableList<PrimaryTable> primary = populator.Add<PrimaryTable>(2);

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

        [Ignore]
        [TestMethod]
        public void ManualKeyPrimaryTable_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                mustBeInATransaction: false);

            IList<RecordReference<ManualKeyPrimaryTableClass>> result = populator.Add<ManualKeyPrimaryTableClass>(5);

            populator.Bind();

            Console.WriteLine(result[0].RecordObject.Key1);
            Console.WriteLine(result[0].RecordObject.Key2);

            Console.WriteLine(result[1].RecordObject.Key1);
            Console.WriteLine(result[1].RecordObject.Key2);
        }

        [Ignore]
        [TestMethod]
        public void SubjectClass_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=localhost;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                mustBeInATransaction: false);

            Guid g = Guid.NewGuid();

            IList<RecordReference<SubjectClass>> result = populator.Add<SubjectClass>(2);

            Go(result, g);

            populator.Bind();

            Console.WriteLine(g);

            Console.WriteLine(result[0].RecordObject.GuidKey);
            Console.WriteLine(result[1].RecordObject.GuidKey);
        }

        private static void Go<T>(IList<RecordReference<T>> x, Guid g) where T : IGuider
        {
            x.ToList().ForEach(y => y.Set(p => p.GuidKey, g));
        }
    }
}
