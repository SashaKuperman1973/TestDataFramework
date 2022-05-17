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
using System.Linq;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardDeferredValueGeneratorTests
    {
        private IAttributeDecorator attributeDecorator;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.attributeDecorator = new StandardAttributeDecorator(null, new AssemblyWrapper(), new Schema());
        }

        [TestMethod]
        public void DeferredValueGenerator_Test()
        {
            // Arrange

            var typeGeneratorMock = new Mock<ITypeGenerator>();

            typeGeneratorMock.Setup(
                    m => m.GetObject<PrimaryTable>(It.IsAny<TypeGeneratorContext>()))
                .Returns(new PrimaryTable());

            typeGeneratorMock.Setup(
                    m => m.GetObject<ForeignTable>(It.IsAny<TypeGeneratorContext>()))
                .Returns(new ForeignTable());

            var recordObject1 = new RecordReference<PrimaryTable>(typeGeneratorMock.Object, this.attributeDecorator,
                null, null, null, null);
            var recordObject2 = new RecordReference<ForeignTable>(typeGeneratorMock.Object, this.attributeDecorator,
                null, null, null, null);

            var dataSource = new Mock<IPropertyDataGenerator<LargeInteger>>();
            var generator = new StandardDeferredValueGenerator<LargeInteger>(dataSource.Object);

            // Act

            recordObject1.Populate();
            recordObject2.Populate();

            generator.AddDelegate(recordObject1.RecordType.GetPropertyInfoProxy("Text"), ul => "A");
            generator.AddDelegate(recordObject1.RecordType.GetPropertyInfoProxy("Integer"), ul => 1);
            generator.AddDelegate(recordObject2.RecordType.GetPropertyInfoProxy("Text"), ul => "B");
            generator.AddDelegate(recordObject2.RecordType.GetPropertyInfoProxy("Integer"), ul => 2);

            generator.Execute(new RecordReference[] {recordObject2, recordObject1});

            // Assert

            Assert.AreEqual("A", recordObject1.RecordObject.Text);
            Assert.AreEqual(1, recordObject1.RecordObject.Integer);
            Assert.AreEqual("B", recordObject2.RecordObject.Text);
            Assert.AreEqual(2, recordObject2.RecordObject.Integer);
        }

        [TestMethod]
        public void AddDelegate_KeyAlreadyExists_Test()
        {
            PropertyInfoProxy targetPropertyInfo = typeof(SubjectClass).GetPropertyInfoProxy(nameof(SubjectClass.Integer));
            DeferredValueGetterDelegate<LargeInteger> valueGetter = i => new object();

            var generator = new StandardDeferredValueGenerator<LargeInteger>(null);

            // Act
            // Assert - No duplicate key exception means the duplicatre key was ignored.

            generator.AddDelegate(targetPropertyInfo, valueGetter);
            generator.AddDelegate(targetPropertyInfo, valueGetter);
        }

        [TestMethod]
        public void Execute_TargetRecordReference_IsExplicitlySet_Test()
        {
            // Arrange

            var dataSource = new Mock<IPropertyDataGenerator<LargeInteger>>();
            PropertyInfoProxy targetPropertyInfo = typeof(SubjectClass).GetPropertyInfoProxy(nameof(SubjectClass.Integer));

            var generator = new StandardDeferredValueGenerator<LargeInteger>(dataSource.Object);
            generator.AddDelegate(targetPropertyInfo, i => 7);

            var recordObject =
                new RecordReference<SubjectClass>(null, null, null, null, null, null)
                {
                    RecordObjectBase = new SubjectClass()
                };

            var propertySetter = new ExplicitPropertySetter
            {
                PropertyChain = new List<PropertyInfoProxy> {typeof(SubjectClass).GetPropertyInfoProxy(nameof(SubjectClass.Integer))}
            };

            recordObject.ExplicitPropertySetters.Add(propertySetter);

            // Act

            generator.Execute(new[] {recordObject});

            // Assert

            Assert.AreNotEqual(7, recordObject.RecordObject.Integer);
        }

        [TestMethod]
        public void ReferenceRecordObjectEqualityComparer_Equals_AreEqual_Test()
        {
            // Arrange

            var subject = Helpers.GetObject<RecordReference<SubjectClass>>();
            subject.RecordObjectBase = new SubjectClass();

            var comparer = new StandardDeferredValueGenerator<LargeInteger>.ReferenceRecordObjectEqualityComparer();

            // Act

            bool result = comparer.Equals(subject, subject);

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReferenceRecordObjectEqualityComparer_Equals_AreNotEqual_Test()
        {
            // Arrange

            var a = Helpers.GetObject<RecordReference<SubjectClass>>();
            a.RecordObjectBase = new SubjectClass();

            var b = Helpers.GetObject<RecordReference<SubjectClass>>();
            b.RecordObjectBase = new SubjectClass();

            var comparer = new StandardDeferredValueGenerator<LargeInteger>.ReferenceRecordObjectEqualityComparer();

            // Act

            bool result = comparer.Equals(a, b);

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ReferenceRecordObjectEqualityComparer_GetHashCode_Test()
        {
            // Arrange

            var reference = Helpers.GetObject<RecordReference<SubjectClass>>();
            var subject = new SubjectClass();
            reference.RecordObjectBase = subject;

            var comparer = new StandardDeferredValueGenerator<LargeInteger>.ReferenceRecordObjectEqualityComparer();

            int expected = subject.GetHashCode();

            // Act

            int actual = comparer.GetHashCode(reference);

            // Assert

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DirectlyRequestedEnumerable_Test()
        {
            // Arrange

            Mock<RecordReference<DeferredGeneratorTestClass>> referenceMock =
                Helpers.GetMock<RecordReference<DeferredGeneratorTestClass>>();

            var deferredGeneratorPropertyEnumerable = new[] {new DeferredKeyClass(),};

            var deferredGeneratorTestObject =
                new DeferredGeneratorTestClass {AProperty = deferredGeneratorPropertyEnumerable};

            referenceMock.SetupGet(m => m.RecordObject).Returns(deferredGeneratorTestObject);
            referenceMock.SetupGet(m => m.RecordObjectBase).Returns(deferredGeneratorTestObject);

            var dataSource = new Mock<IPropertyDataGenerator<LargeInteger>>();
            var generator = new StandardDeferredValueGenerator<LargeInteger>(dataSource.Object);

            PropertyInfoProxy keyClassKeyProperty = typeof(DeferredKeyClass).GetPropertyInfoProxy(nameof(DeferredKeyClass.Key));

            dataSource.Setup(m => m.FillData(It.IsAny<Dictionary<PropertyInfoProxy, Data<LargeInteger>>>()))
                .Callback<IDictionary<PropertyInfoProxy, Data<LargeInteger>>>(
                    propertyDataDictionary => propertyDataDictionary[keyClassKeyProperty].Item = 1);

            // Act

            generator.AddDelegate(keyClassKeyProperty, input => (int)(uint)input++);

            generator.Execute(new [] {referenceMock.Object});

            // Assert

            IEnumerable<DeferredKeyClass> deferredKeyClassCollectionPropertyValue =
                referenceMock.Object.RecordObject.AProperty.ToList();

            Assert.AreEqual(1, deferredKeyClassCollectionPropertyValue.First().Key);
        }
    }
}