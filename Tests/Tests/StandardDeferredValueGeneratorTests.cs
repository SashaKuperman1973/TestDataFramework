/*
    Copyright 2016 Alexander Kuperman

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
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardDeferredValueGeneratorTests
    {
        [TestMethod]
        public void DeferredValueGenerator_Test()
        {
            // Arrange

            var typeGeneratorMock = new Mock<ITypeGenerator>();

            typeGeneratorMock.Setup(
                m => m.GetObject<PrimaryTable>(It.IsAny<ConcurrentDictionary<PropertyInfo, Action<PrimaryTable>>>()))
                .Returns(new PrimaryTable());

            typeGeneratorMock.Setup(
                m => m.GetObject<ForeignTable>(It.IsAny<ConcurrentDictionary<PropertyInfo, Action<ForeignTable>>>()))
                .Returns(new ForeignTable());

            var recordObject1 = new RecordReference<PrimaryTable>(typeGeneratorMock.Object);
            var recordObject2 = new RecordReference<ForeignTable>(typeGeneratorMock.Object);

            var dataSource = new Mock<IPropertyDataGenerator<LargeInteger>>();
            var generator = new StandardDeferredValueGenerator<LargeInteger>(dataSource.Object);

            // Act

            recordObject1.Populate();
            recordObject2.Populate();

            generator.AddDelegate(recordObject1.RecordType.GetProperty("Text"), ul => "A");
            generator.AddDelegate(recordObject1.RecordType.GetProperty("Integer"), ul => 1);
            generator.AddDelegate(recordObject2.RecordType.GetProperty("Text"), ul => "B");
            generator.AddDelegate(recordObject2.RecordType.GetProperty("Integer"), ul => 2);

            generator.Execute(new RecordReference[] {recordObject2, recordObject1});

            // Assert

            Assert.AreEqual("A", recordObject1.RecordObject.Text);
            Assert.AreEqual(1, recordObject1.RecordObject.Integer);
            Assert.AreEqual("B", recordObject2.RecordObject.Text);
            Assert.AreEqual(2, recordObject2.RecordObject.Integer);
        }
    }
}
