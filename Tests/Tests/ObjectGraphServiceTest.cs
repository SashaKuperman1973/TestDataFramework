/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Concrete;
using Tests.TestModels;

namespace Tests.Tests
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

        [TestMethod]
        public void DoesPropertyHaveSetter_Test()
        {
            var objectGraphService = new ObjectGraphService();

            List<PropertyInfo> objectGraph =
                objectGraphService.GetObjectGraph<SubjectClass, int>(subjectClassParam => subjectClassParam.SecondObject
                    .SecondInteger);

            var setters = new List<ExplicitPropertySetter>
            {
                new ExplicitPropertySetter
                {
                    PropertyChain = objectGraph
                }
            };

            // Act

            bool result = objectGraphService.DoesPropertyHaveSetter(objectGraph, setters);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DoesPropertyHaveSetter_ObjectGraph_IsOn_PropertyChain_Test()
        {
            var objectGraphService = new ObjectGraphService();

            List<PropertyInfo> objectGraph =
                objectGraphService.GetObjectGraph<SubjectClass, ThirdClass>(subjectClassParam => subjectClassParam.SecondObject
                    .ThirdObject);

            List<PropertyInfo> propertyChain =
                objectGraphService.GetObjectGraph<SubjectClass, int>(subjectClassParam => subjectClassParam.SecondObject
                    .ThirdObject.ThirdInteger);

            var setters = new List<ExplicitPropertySetter>
            {
                new ExplicitPropertySetter
                {
                    PropertyChain = propertyChain
                }
            };

            // Act

            bool result = objectGraphService.DoesPropertyHaveSetter(objectGraph, setters);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DoesPropertyHaveSetter_ObjectGraph_DivergesFrom_SetterPropertyChain_Test()
        {
            var objectGraphService = new ObjectGraphService();

            List<PropertyInfo> objectGraph =
                objectGraphService.GetObjectGraph<SubjectClass, int>(subjectClassParam => subjectClassParam.SecondObject
                    .SecondInteger);

            List<PropertyInfo> propertyChain =
                objectGraphService.GetObjectGraph<SubjectClass, int>(subjectClassParam => subjectClassParam.SecondObject
                    .ThirdObject.ThirdInteger);

            var setters = new List<ExplicitPropertySetter>
            {
                new ExplicitPropertySetter
                {
                    PropertyChain = propertyChain
                }
            };

            // Act

            bool result = objectGraphService.DoesPropertyHaveSetter(objectGraph, setters);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DoesPropertyHaveSetter_ObjectGraph_IsPassed_SetterPropertyChain_Test()
        {
            var objectGraphService = new ObjectGraphService();

            List<PropertyInfo> objectGraph =
                objectGraphService.GetObjectGraph<SubjectClass, int>(subjectClassParam => subjectClassParam.SecondObject
                    .ThirdObject.ThirdInteger);

            List<PropertyInfo> propertyChain =
                objectGraphService.GetObjectGraph<SubjectClass, ThirdClass>(subjectClassParam => subjectClassParam.SecondObject
                    .ThirdObject);

            var setters = new List<ExplicitPropertySetter>
            {
                new ExplicitPropertySetter
                {
                    PropertyChain = propertyChain
                }
            };

            // Act

            bool result = objectGraphService.DoesPropertyHaveSetter(objectGraph, setters);

            Assert.IsFalse(result);
        }
    }
}