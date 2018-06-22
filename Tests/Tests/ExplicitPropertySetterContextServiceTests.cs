using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.DeepSetting;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class ExplicitPropertySetterContextServiceTests
    {
        [TestMethod]
        public void SetRecordReference_Test()
        {
            var service = new ExplicitPropertySetterContextService();

            var reference = new RecordReference<SubjectClass>(null, null, null, null, null, null);
            var propertySetter = new ExplicitPropertySetter();

            service.SetRecordReference(reference, propertySetter);

            Assert.AreEqual(propertySetter, reference.ExplicitPropertySetters.Single());
        }

        [TestMethod]
        public void FilterInWorkingListOfReferfences_Test()
        {
            var service = new ExplicitPropertySetterContextService();

            var references = new List<RecordReference<SubjectClass>>();

            var propertyChain = new List<PropertyInfo>
            {
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer)),
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.AnEmailAddress)),
            };

            for (int i = 0; i < 20; i++)
            {
                var reference = new RecordReference<SubjectClass>(null, null, null, null, null, null);
                references.Add(reference);

                if (i == 10 || i == 11 || i == 12)
                {
                    reference.ExplicitPropertySetters.Add(new ExplicitPropertySetter {PropertyChain = propertyChain});
                }
            }

            var guaranteedValuesList = new List<GuaranteedValues>
            {
                new GuaranteedValues
                {
                    Values = new List<ExplicitPropertySetter>
                    {
                        new ExplicitPropertySetter
                        {
                            PropertyChain = propertyChain
                        }
                    }
                }
            };

            List<RecordReference<SubjectClass>> result = service.FilterInWorkingListOfReferfences(references, guaranteedValuesList);

            Assert.AreEqual(17, result.Count);
        }
    }
}
