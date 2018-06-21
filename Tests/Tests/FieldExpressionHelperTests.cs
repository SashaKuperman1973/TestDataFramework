using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using TestDataFramework.DeepSetting;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator.Concrete;
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
