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
            // Arrange

            var service = new ExplicitPropertySetterContextService();

            var reference = new RecordReference<SubjectClass>(null, null, null, null, null, null);
            var propertySetter = new ExplicitPropertySetter();

            // Act

            service.SetRecordReference(reference, propertySetter);

            // Assert

            Assert.AreEqual(propertySetter, reference.ExplicitPropertySetters.Single());
        }

        [TestMethod]
        public void FilterInWorkingListOfReferfences_Test()
        {
            // Arrange

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

            // Act

            List<RecordReference<SubjectClass>> result = service.FilterInWorkingListOfReferfences(references, guaranteedValuesList);

            // Assert

            Assert.AreEqual(17, result.Count);
        }

        [TestMethod]
        public void ExplicitPropertySetterContextService_PropertyChains_DifferentLength_Test()
        {
            // Arrange

            var service = new ExplicitPropertySetterContextService();

            var propertyChain1 = new List<PropertyInfo>
            {
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer)),
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.AnEmailAddress)),
            };

            var propertyChain2 = new List<PropertyInfo>
            {
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer)),
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.AnEmailAddress)),
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.Decimal)),
            };

            var referencePropertySetter =
                new ExplicitPropertySetter {PropertyChain = propertyChain1};

            var reference = new RecordReference<SubjectClass>(null, null, null, null, null, null);

            reference.ExplicitPropertySetters.Add(referencePropertySetter);

            var references = new List<RecordReference<SubjectClass>>
            {
                reference
            };

            var values = new List<GuaranteedValues>
            {
                new GuaranteedValues
                {
                    Values = new List<object>
                    {
                        new ExplicitPropertySetter
                        {
                            PropertyChain = propertyChain2
                        }
                    }
                }
            };

            // Act

            List<RecordReference<SubjectClass>> result = service.FilterInWorkingListOfReferfences(references, values);

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result.Single());
        }

        [TestMethod]
        public void FilterInWorkingListOfReferfences_PropertyChains_Different_Lengths_Test()
        {
            // Arrange

            var service = new ExplicitPropertySetterContextService();

            var propertyChain1 = new List<PropertyInfo>
            {
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer)),
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.AnEmailAddress)),
            };

            var propertyChain2 = new List<PropertyInfo>
            {
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer)),
                typeof(SubjectClass).GetProperty(nameof(SubjectClass.Decimal)),
            };

            var referencePropertySetter =
                new ExplicitPropertySetter { PropertyChain = propertyChain1 };

            var reference = new RecordReference<SubjectClass>(null, null, null, null, null, null);

            reference.ExplicitPropertySetters.Add(referencePropertySetter);

            var references = new List<RecordReference<SubjectClass>>
            {
                reference
            };

            var guaranteedValuesList = new List<GuaranteedValues>
            {
                new GuaranteedValues
                {
                    Values = new List<ExplicitPropertySetter>
                    {
                        new ExplicitPropertySetter
                        {
                            PropertyChain = propertyChain2
                        }
                    }
                }
            };

            // Act

            List<RecordReference<SubjectClass>> result = service.FilterInWorkingListOfReferfences(references, guaranteedValuesList);

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result.Single());
        }
    }
}
