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
using System.Linq;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator;
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

            var primaryRecordReference = new RecordReference<T1>(Helpers.GetTypeGeneratorMock(primaryTable).Object, this.attributeDecorator);
            var foreignRecordReference = new RecordReference<T2>(Helpers.GetTypeGeneratorMock(foreignTable).Object, this.attributeDecorator);

            // Act

            Helpers.ExceptionTest(() => foreignRecordReference.AddPrimaryRecordReference(primaryRecordReference),
                typeof (NoReferentialIntegrityException),
                string.Format(Messages.NoReferentialIntegrity,
                    Helper.PrintType(typeof (T1)),
                    Helper.PrintType(typeof (T2))));
        }

        [TestMethod]
        public void AddPrimaryRecordReference_Test()
        {
            // Arrange

            var primaryTable = new PrimaryTable();
            var foreignTable = new ForeignTable();

            var primaryRecordReference = new RecordReference<PrimaryTable>(Helpers.GetTypeGeneratorMock(primaryTable).Object, this.attributeDecorator);
            var foreignRecordReference = new RecordReference<ForeignTable>(Helpers.GetTypeGeneratorMock(foreignTable).Object, this.attributeDecorator);

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

            var typeGeneratorMock = new Mock<ITypeGenerator>();

            var recordReference = new RecordReference<PrimaryTable>(typeGeneratorMock.Object, this.attributeDecorator);

            var testRecord = new PrimaryTable();

            typeGeneratorMock.Setup(
                m => m.GetObject<PrimaryTable>(It.IsAny<ConcurrentDictionary<PropertyInfo, Action<PrimaryTable>>>()))
                .Callback<ConcurrentDictionary<PropertyInfo, Action<PrimaryTable>>>(
                    d =>
                    {
                        d[typeof (PrimaryTable).GetProperty("Integer")](testRecord);
                        d[typeof (PrimaryTable).GetProperty("Text")](testRecord);
                        d[typeof(PrimaryTable).GetProperty("Array")](testRecord);
                    });
            // Act

            recordReference.Set(r => r.Integer, expectedInt)
                .Set(r => r.Text, expectedString)
                .Set(r => r.Array, () => expectedArray);

            recordReference.Populate();

            // Assert

            Assert.AreEqual(expectedInt, testRecord.Integer);
            Assert.AreEqual(expectedString, testRecord.Text);
            Assert.AreEqual(expectedArray, testRecord.Array);
        }

        // SetRange returns random results so there is no deterministic way to test results.
        // This test is for manual inspection.
        [TestMethod]
        public void SetRange_Test()
        {
            // Arrange

            var guids = new Guid[5];
            for (int j = 0; j < guids.Length; j++)
            {
                guids[j] = Guid.NewGuid();
            }

            for (int i = 0; i < 10; i++)
            {

                var typeGeneratorMock = new Mock<ITypeGenerator>();

                var recordReference = new RecordReference<PrimaryTable>(typeGeneratorMock.Object,
                    this.attributeDecorator);

                var testRecord1 = new PrimaryTable();
                var testRecord2 = new PrimaryTable();

                typeGeneratorMock.Setup(
                    m => m.GetObject<PrimaryTable>(It.IsAny<ConcurrentDictionary<PropertyInfo, Action<PrimaryTable>>>()))
                    .Callback<ConcurrentDictionary<PropertyInfo, Action<PrimaryTable>>>(
                        d =>
                        {
                            d[typeof(PrimaryTable).GetProperty("Guid")](testRecord1);
                            d[typeof(PrimaryTable).GetProperty("Guid")](testRecord2);
                        });

                // Act

                recordReference.SetRange(r => r.Guid, guids);

                recordReference.Populate();
            }
        }

        [TestMethod]
        public void Populate_Test()
        {
            var record = new PrimaryTable();

            Mock<ITypeGenerator> typeGeneratorMock = Helpers.GetTypeGeneratorMock(record);

            var recordReference = new RecordReference<PrimaryTable>(typeGeneratorMock.Object, this.attributeDecorator);

            // Act

            recordReference.Populate();

            // Assert

            Assert.AreEqual(record, recordReference.RecordObject);
        }
    }
}
