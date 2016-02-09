using System;
using System.Collections.Generic;
using IntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace IntegrationTests.Tests
{
    [Ignore]
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
