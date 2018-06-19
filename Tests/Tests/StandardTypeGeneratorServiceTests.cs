using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.DeepSetting;
using TestDataFramework.TypeGenerator.Concrete;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardTypeGeneratorServiceTests
    {
        private StandardTypeGeneratorService service;

        [TestInitialize]
        public void Initialize()
        {
            this.service = new StandardTypeGeneratorService();
        }

        [TestMethod]
        public void IsPropertyExplicitlySet_Test()
        {
            PropertyInfo[] properties = typeof(SubjectClass).GetProperties();

            ObjectGraphNode objectGraphNode =
                properties.Aggregate<PropertyInfo, ObjectGraphNode>(null,
                    (current, property) => new ObjectGraphNode(property, current));

            var explicitPropertySetter = new ExplicitPropertySetter
            {
                PropertyChain = properties.ToList()
            };

            var propertySetters =
                new List<ExplicitPropertySetter>
                {
                    new ExplicitPropertySetter {PropertyChain = new List<PropertyInfo>()},
                    explicitPropertySetter
                };

            // Act

            IEnumerable<ExplicitPropertySetter> result =
                this.service.GetExplicitlySetPropertySetters(propertySetters, objectGraphNode);

            Assert.IsNotNull(result);
            List<ExplicitPropertySetter> resultList = result.ToList();
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(explicitPropertySetter, resultList.Single());
        }

        [TestMethod]
        public void ObjectGraphNode_Is_Null_Test()
        {
            var propertySetters = new List<ExplicitPropertySetter> {null};
            IEnumerable<ExplicitPropertySetter> result =
                this.service.GetExplicitlySetPropertySetters(propertySetters, null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void ExplicitPropertySetter_Are_FilteredIn_By_ObjectGraph_Test()
        {
            var objectGraphMatch = new ExplicitPropertySetter
            {
                PropertyChain = new List<PropertyInfo>
                {
                    typeof(SubjectClass).GetProperty(nameof(SubjectClass.SecondObject))
                }
            };

            var propertySetters = new List<ExplicitPropertySetter>
            {
                new ExplicitPropertySetter
                {
                    PropertyChain = new List<PropertyInfo>
                    {
                        typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer)),
                    }
                },

                objectGraphMatch
            };

            var objectGraphNode = new ObjectGraphNode(
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.SecondObject)),
                null);

            IEnumerable<ExplicitPropertySetter> result =
                this.service.GetExplicitlySetPropertySetters(propertySetters, objectGraphNode);

            Assert.IsNotNull(result);
            List<ExplicitPropertySetter> resultList = result.ToList();
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(objectGraphMatch, resultList.Single());
        }
    }
}