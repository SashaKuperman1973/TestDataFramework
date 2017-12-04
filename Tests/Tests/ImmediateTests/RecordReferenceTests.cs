/*
    Copyright 2016, 2017 Alexander Kuperman

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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class RecordReferenceTests
    {
        private IAttributeDecorator attributeDecorator;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.attributeDecorator = new StandardAttributeDecorator(attributeDecorator => null, null);
        }

        [TestMethod]
        public void ThrowsWhenTypeMismatch_Test()
        {
            this.ThrowsWhenTypeMismatch<TypeMismatchPrimaryTable, TypeMismatchForeignTable>();
            this.ThrowsWhenTypeMismatch<TableTypeMismatchPrimaryTable, TableTypeMismatchForeignTable>();
            this.ThrowsWhenTypeMismatch<PropertyNameMismatchPrimaryTable, PropertyNameMismatchForeignTable>();
        }

        private void ThrowsWhenTypeMismatch<T1, T2>() where T1 : new() where T2 : new()
        {
            // Arrange

            var primaryTable = new T1();
            var foreignTable = new T2();

            var primaryRecordReference = new RecordReference<T1>(Helpers.GetTypeGeneratorMock(primaryTable).Object,
                this.attributeDecorator, null, null);
            var foreignRecordReference = new RecordReference<T2>(Helpers.GetTypeGeneratorMock(foreignTable).Object,
                this.attributeDecorator, null, null);

            // Act

            Helpers.ExceptionTest(() => foreignRecordReference.AddPrimaryRecordReference(primaryRecordReference),
                typeof(NoReferentialIntegrityException),
                string.Format(Messages.NoReferentialIntegrity,
                    Helper.PrintType(typeof(T1)),
                    Helper.PrintType(typeof(T2))));
        }

        [TestMethod]
        public void AddPrimaryRecordReference_Test()
        {
            // Arrange

            var primaryTable = new PrimaryTable();
            var foreignTable = new ForeignTable();

            var primaryRecordReference =
                new RecordReference<PrimaryTable>(Helpers.GetTypeGeneratorMock(primaryTable).Object,
                    this.attributeDecorator, null, null);
            var foreignRecordReference =
                new RecordReference<ForeignTable>(Helpers.GetTypeGeneratorMock(foreignTable).Object,
                    this.attributeDecorator, null, null);

            // Act

            foreignRecordReference.AddPrimaryRecordReference(primaryRecordReference);

            // Assert

            Assert.AreEqual(1, foreignRecordReference.PrimaryKeyReferences.Count());
        }

        [TestMethod]
        public void Set_Test()
        {
            // Arrange

            const int expectedInt = 5;
            const string expectedString = "ABCD";
            int[] expectedArray = new[] {1, 2};

            var objectGraphServiceMock = new Mock<IObjectGraphService>();

            PropertyInfo intPropertyInfo = typeof(PrimaryTable).GetProperty(nameof(PrimaryTable.Integer));
            PropertyInfo stringPropertyInfo = typeof(PrimaryTable).GetProperty(nameof(PrimaryTable.Text));
            PropertyInfo arrayPropertyInfo = typeof(PrimaryTable).GetProperty(nameof(PrimaryTable.Array));

            objectGraphServiceMock.Setup(m => m.GetObjectGraph(It.IsAny<Expression<Func<PrimaryTable, int>>>()))
                .Returns(new List<PropertyInfo> {intPropertyInfo});

            objectGraphServiceMock.Setup(m => m.GetObjectGraph(It.IsAny<Expression<Func<PrimaryTable, string>>>()))
                .Returns(new List<PropertyInfo> {stringPropertyInfo});

            objectGraphServiceMock.Setup(m => m.GetObjectGraph(It.IsAny<Expression<Func<PrimaryTable, int[]>>>()))
                .Returns(new List<PropertyInfo> {arrayPropertyInfo});

            var recordReference =
                new RecordReference<PrimaryTable>(null, null, null, objectGraphServiceMock.Object);

            // Act

            recordReference.Set(r => r.Integer, expectedInt)
                .Set(r => r.Text, expectedString)
                .Set(r => r.Array, () => expectedArray);

            // Assert

            List<ExplicitPropertySetters> setters = recordReference.ExplicitPropertySetters;
            var testRecord = new PrimaryTable();

            Assert.AreEqual(intPropertyInfo.Name, setters[0].PropertyChain[0].Name);
            setters[0].Action(testRecord);
            Assert.AreEqual(expectedInt, testRecord.Integer);

            Assert.AreEqual(stringPropertyInfo.Name, setters[1].PropertyChain[0].Name);
            setters[1].Action(testRecord);
            Assert.AreEqual(expectedString, testRecord.Text);

            Assert.AreEqual(arrayPropertyInfo.Name, setters[2].PropertyChain[0].Name);
            setters[2].Action(testRecord);
            Assert.AreEqual(expectedArray, testRecord.Array);
        }

        [TestMethod]
        public void SetRange_Test()
        {
            // Arrange

            var guids = new Guid[5];
            for (int i = 0; i < guids.Length; i++)
            {
                guids[i] = Guid.NewGuid();
            }

            var objectGraphServiceMock = new Mock<IObjectGraphService>();

            var recordReference = new RecordReference<PrimaryTable>(null,
                null, null, objectGraphServiceMock.Object);

            var setterObjectGraph = new List<PropertyInfo> {typeof(PrimaryTable).GetProperty(nameof(PrimaryTable.Guid))};

            objectGraphServiceMock.Setup(m => m.GetObjectGraph(It.IsAny<Expression<Func<PrimaryTable, Guid>>>()))
                .Returns(setterObjectGraph);

            // Act

            recordReference.SetRange(r => r.Guid, guids);

            // Assert

            ExplicitPropertySetters propertySetter = recordReference.ExplicitPropertySetters.First();

            Assert.AreEqual(setterObjectGraph, propertySetter.PropertyChain);

            var primaryTable = new PrimaryTable();
            propertySetter.Action(primaryTable);

            Assert.IsTrue(guids.Contains(primaryTable.Guid));
        }

        [TestMethod]
        public void Populate_Test()
        {
            var record = new PrimaryTable();

            Mock<ITypeGenerator> typeGeneratorMock = Helpers.GetTypeGeneratorMock(record);

            var recordReference =
                new RecordReference<PrimaryTable>(typeGeneratorMock.Object, this.attributeDecorator, null, null);

            // Act

            recordReference.Populate();

            // Assert

            Assert.AreEqual(record, recordReference.RecordObject);
        }

        [TestMethod]
        public void RecordReference_DeepSet_Test()
        {
            // Arrange

            var objectGraphServiceMock = new Mock<IObjectGraphService>();

            objectGraphServiceMock
                .Setup(m => m.GetObjectGraph(It.IsAny<Expression<Func<ThirdDeepPropertyTable, string>>>()))
                .Returns(new List<PropertyInfo>
                {
                    typeof(FirstDeepPropertyTable).GetProperty(nameof(FirstDeepPropertyTable.Value))
                });

            objectGraphServiceMock
                .Setup(m => m.GetObjectGraph(It.IsAny<Expression<Func<ThirdDeepPropertyTable, FirstDeepPropertyTable>>>()))
                .Returns(new List<PropertyInfo>
                {
                    typeof(SecondDeepPropertyTable).GetProperty(nameof(SecondDeepPropertyTable.Deep1))
                });

            var recordReference = new RecordReference<ThirdDeepPropertyTable>(null,
                null, null, objectGraphServiceMock.Object);

            const string expectedValue1 = "qwerty";
            var expectedValue2 = new FirstDeepPropertyTable();

            // Act

            recordReference.Set(r => r.Deep2.Deep1.Value, expectedValue1);
            recordReference.Set(r => r.Deep2.Deep1, expectedValue2);

            // Assert

            PropertyInfo propertyChain = recordReference.ExplicitPropertySetters[0].PropertyChain.Single();

            Assert.AreEqual(nameof(FirstDeepPropertyTable), propertyChain.DeclaringType.Name);
            Assert.AreEqual(nameof(FirstDeepPropertyTable.Value), propertyChain.Name);

            propertyChain = recordReference.ExplicitPropertySetters[1].PropertyChain.Single();

            Assert.AreEqual(nameof(SecondDeepPropertyTable), propertyChain.DeclaringType.Name);
            Assert.AreEqual(nameof(SecondDeepPropertyTable.Deep1), propertyChain.Name);
        }
    }
}
