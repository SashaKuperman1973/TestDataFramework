using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.DeepSetting;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class ValueSetContextServiceTests
    {
        [TestMethod]
        public void SetRecordReference_Test()
        {
            var service = new ValueSetContextService();

            var input = new RecordReference<SubjectClass>(null, null, null, null, null, null);

            service.SetRecordReference(input, new SubjectClass());

            Assert.IsTrue(input.IsPopulated);
        }

        [TestMethod]
        public void FilterInWorkingListOfReferfences_Test()
        {
            var service = new ValueSetContextService();

            var references = new List<RecordReference<SubjectClass>>();

            for (int i = 0; i < 20; i++)
            {
                var reference = new RecordReference<SubjectClass>(null, null, null, null, null, null);
                references.Add(reference);

                if (i == 10 || i == 11 || i == 12)
                {
                    reference.ExplicitPropertySetters.Add(new ExplicitPropertySetter());
                }
            }

            List<RecordReference<SubjectClass>> result = service.FilterInWorkingListOfReferfences(references, null);

            Assert.AreEqual(17, result.Count);
        }
    }
}
