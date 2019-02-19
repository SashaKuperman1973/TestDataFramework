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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using TestDataFramework.DeepSetting;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.FieldExpression;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class FieldExpressionHelperTests
    {
        [TestMethod]
        public void GetFuncOrValueBasedExlicitPropertySetter_Null_Input_Test()
        {
            // Arrange

            var objectGraph = new List<PropertyInfo> {typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer))};

            // Act

            ExplicitPropertySetter result =
                FieldExpressionHelper.GetFuncOrValueBasedExlicitPropertySetter<int>(null, objectGraph);

            // Assert

            Assert.AreEqual(objectGraph, result.PropertyChain);

            var subject = new SubjectClass {Integer = 1};
            result.Action(subject);
            Assert.AreEqual(0, subject.Integer);
        }

        [TestMethod]
        public void GetFuncOrValueBasedExlicitPropertySetter_Input_Not_TProperty_Throws()
        {
            // Act

            Helpers.ExceptionTest(() => FieldExpressionHelper.GetFuncOrValueBasedExlicitPropertySetter<int>("X", null), typeof(ValueGuaranteeException),
                "Guaranteed input type not of collection type. Expected type: System.Int32, Actual type: System.String");
        }

        [TestMethod]
        public void GetFuncOrValueBasedExlicitPropertySetter_Func_Input_Test()
        {
            // Arrange

            var objectGraph = new List<PropertyInfo> { typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer)) };

            // Act

            ExplicitPropertySetter result =
                FieldExpressionHelper.GetFuncOrValueBasedExlicitPropertySetter<int>((Func<int>)(() => 5), objectGraph);

            // Assert

            Assert.AreEqual(objectGraph, result.PropertyChain);

            var subject = new SubjectClass();
            result.Action(subject);
            Assert.AreEqual(5, subject.Integer);
        }

        [TestMethod]
        public void GetFuncOrValueBasedExlicitPropertySetter_TProperty_Value_Input_Test()
        {
            // Arrange

            var objectGraph = new List<PropertyInfo> { typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer)) };

            // Act

            ExplicitPropertySetter result =
                FieldExpressionHelper.GetFuncOrValueBasedExlicitPropertySetter<int>(5, objectGraph);

            // Assert

            Assert.AreEqual(objectGraph, result.PropertyChain);

            var subject = new SubjectClass();
            result.Action(subject);
            Assert.AreEqual(5, subject.Integer);
        }
    }
}
