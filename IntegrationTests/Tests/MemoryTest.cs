using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationTests.TestModels;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator;

namespace IntegrationTests.Tests
{
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

            this.Dump(subjectReference[0].RecordObject);
            this.Dump(subjectReference[1].RecordObject);
            this.Dump(foreignReference.RecordObject);
        }

        private void Dump(object target)
        {
            Console.WriteLine();
            Console.WriteLine(target.GetType().Name);
            target.GetType().GetProperties().ToList().ForEach(p =>
            {
                try
                {
                    Console.WriteLine(p.Name + " = " + p.GetValue(target));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(p.Name + ": " + MemoryTest.GetMessage(ex));
                }
            });
        }

        private static string GetMessage(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return MemoryTest.GetMessage(ex.InnerException);
            }

            return ex.Message;
        }
    }
}
