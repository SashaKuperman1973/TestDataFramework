using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.DeepSetting.Concrete;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class ObjectGraphServiceTest
    {
        [TestMethod]
        public void GetObjectGraph_Test()
        {
            // Act

            var objectGraphService = new ObjectGraphService();

            List<PropertyInfo> objectGraph =
                objectGraphService.GetObjectGraph<SubjectClass, int>(subjectClassParam => subjectClassParam.SecondObject
                    .SecondInteger);

            Assert.AreEqual(nameof(SubjectClass.SecondObject), objectGraph[0].Name);
            Assert.AreEqual(typeof(SecondClass), objectGraph[0].PropertyType);

            Assert.AreEqual(nameof(SecondClass.SecondInteger), objectGraph[1].Name);
            Assert.AreEqual(typeof(int), objectGraph[1].PropertyType);
        }
    }
}