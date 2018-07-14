/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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