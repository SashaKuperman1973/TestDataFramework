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
    //[Ignore]
    [TestClass]
    public class MemoryTest
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
        public void Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IList<RecordReference<SubjectClass>> subjectReference = populator.Add<SubjectClass>(2);
            RecordReference<ForeignSubjectClass> foreignReference = populator.Add<ForeignSubjectClass>(subjectReference[1]);
            populator.Bind();

            Helpers.Dump(subjectReference[0].RecordObject);
            Helpers.Dump(subjectReference[1].RecordObject);
            Helpers.Dump(foreignReference.RecordObject);
        }

        // This is a test of value types in general.
        [TestMethod]
        public void Multiple_KeyValuePair_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IList<RecordReference<KeyValuePair<SubjectClass, ForeignSubjectClass>>> kvpRefs =
                populator.Add<KeyValuePair<SubjectClass, ForeignSubjectClass>>(10);

            populator.Bind();

            Helpers.Dump(kvpRefs.First().RecordObject.Key);
            Helpers.Dump(kvpRefs.First().RecordObject.Value);
            kvpRefs.ToList().ForEach(r => Helpers.Dump(r.RecordObject));
        }

        [TestMethod]
        public void Dictionary_UniqueValueTypeKeys_Test()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            RecordReference<ClassWithHandledTypes> recordReference = populator.Add<ClassWithHandledTypes>();
            populator.Bind();

            IDictionary<KeyValuePair<int, string>, object> dictionary = recordReference.RecordObject.ADictionary;

            foreach (KeyValuePair<KeyValuePair<int, string>, object> item in dictionary)
            {
                Console.WriteLine(item.Key);
            }
        }

        [TestMethod]
        public void GauranteedValueTest()
        {
            IPopulator populator = this.factory.CreateMemoryPopulator();

            IList<RecordReference<SubjectClass>> subjectReference = populator.Add<SubjectClass>(2);
            OperableList<ForeignSubjectClass> foreignReference =
                populator.Add<ForeignSubjectClass>(20, subjectReference[1]).Guarantee(new[]
                {
                    new ForeignSubjectClass {SecondInteger = 777},
                    new ForeignSubjectClass {SecondInteger = 888},
                    new ForeignSubjectClass {SecondInteger = 999},
                }, 50);

            populator.Bind();

            int i = 1;
            foreignReference.ToList().ForEach(r => Console.WriteLine(i++ + ".\r\n" + r.RecordObject.ToString()));
        }
    }
}
