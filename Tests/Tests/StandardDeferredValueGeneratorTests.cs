using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.TypeGenerator;
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
