using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class DefaultInitialCountGeneratorTests
    {
        [TestMethod]
        public void FillData_Test()
        {
            // Arrange

            var generator = new DefaultInitialCountGenerator();

            var propertyDataDictionary = new Dictionary<PropertyInfo, Data<LargeInteger>>();

            var value1 = new Data<LargeInteger>(null) { Item = new LargeInteger() };
            var value2 = new Data<LargeInteger>(null) { Item = new LargeInteger() };

            propertyDataDictionary.Add(typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer)), value1);
            propertyDataDictionary.Add(typeof(SubjectClass).GetProperty(nameof(SubjectClass.Decimal)), value2);

            // Act

            generator.FillData(propertyDataDictionary);

            // Assert

            Assert.AreEqual(new LargeInteger(1), value1.Item);
            Assert.AreEqual(new LargeInteger(1), value2.Item);
        }
    }
}
