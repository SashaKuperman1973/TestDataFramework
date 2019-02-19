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
