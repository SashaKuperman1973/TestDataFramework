/*
    Copyright 2016 Alexander Kuperman

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

        //[Ignore]
        [TestMethod]
        public void ManualKeyPrimaryTable_Test()
        {
            IPopulator populator = this.factory.CreateSqlClientPopulator(
                @"Data Source=.\SqlExpress;Initial Catalog=TestDataFramework;Integrated Security=SSPI;",
                mustBeInATransaction: false);

            IList<RecordReference<ManualKeyPrimaryTableClass>> result = populator.Add<ManualKeyPrimaryTableClass>(5);

            populator.Bind();

            Console.WriteLine(result[0].RecordObject.Key1);
            Console.WriteLine(result[0].RecordObject.Key2);

            Console.WriteLine(result[1].RecordObject.Key1);
            Console.WriteLine(result[1].RecordObject.Key2);
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
            Console.WriteLine(result[0].RecordObject.GuidKey);

            Console.WriteLine(result[1].RecordObject.Key);
            Console.WriteLine(result[1].RecordObject.GuidKey);
        }
    }
}
