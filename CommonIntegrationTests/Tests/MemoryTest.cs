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
    }
}
