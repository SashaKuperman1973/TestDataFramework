/*
    Copyright 2016 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class RecordReferenceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();
        }

        [TestMethod]
        public void ThrowsWhenTypeMismatch_Test()
        {
            RecordReferenceTests.ThrowsWhenTypeMismatch<TypeMismatchPrimaryTable, TypeMismatchForeignTable>();
            RecordReferenceTests.ThrowsWhenTypeMismatch<TableTypeMismatchPrimaryTable, TableTypeMismatchForeignTable>();
            RecordReferenceTests.ThrowsWhenTypeMismatch<PropertyNameMismatchPrimaryTable, PropertyNameMismatchForeignTable>();
        }

        private static void ThrowsWhenTypeMismatch<T1, T2>() where T1 : new() where T2 : new()
        {
            // Arrange

            var primaryTable = new T1();
            var foreignTable = new T2();

            var primaryRecordReference = new RecordReference<T1>(Helpers.GetTypeGeneratorMock(primaryTable).Object);
            var foreignRecordReference = new RecordReference<T2>(Helpers.GetTypeGeneratorMock(foreignTable).Object);

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

            var primaryRecordReference = new RecordReference<PrimaryTable>(Helpers.GetTypeGeneratorMock(primaryTable).Object);
            var foreignRecordReference = new RecordReference<ForeignTable>(Helpers.GetTypeGeneratorMock(foreignTable).Object);

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

            var typeGeneratorMock = new Mock<ITypeGenerator>();

            var recordReference = new RecordReference<PrimaryTable>(typeGeneratorMock.Object);

            var testRecord = new PrimaryTable();

            typeGeneratorMock.Setup(
                m => m.GetObject<PrimaryTable>(It.IsAny<ConcurrentDictionary<PropertyInfo, Action<PrimaryTable>>>()))
                .Callback<ConcurrentDictionary<PropertyInfo, Action<PrimaryTable>>>(
                    d =>
                    {
                        d[typeof (PrimaryTable).GetProperty("Integer")](testRecord);
                        d[typeof (PrimaryTable).GetProperty("Text")](testRecord);
                    });
            // Act

            recordReference.Set(r => r.Integer, expectedInt).Set(r => r.Text, expectedString);
            recordReference.Populate();

            // Assert

            Assert.AreEqual(expectedInt, testRecord.Integer);
            Assert.AreEqual(expectedString, testRecord.Text);
        }

        [TestMethod]
        public void Populate_Test()
        {
            var record = new PrimaryTable();

            Mock<ITypeGenerator> typeGeneratorMock = Helpers.GetTypeGeneratorMock(record);

            var recordReference = new RecordReference<PrimaryTable>(typeGeneratorMock.Object);

            // Act

            recordReference.Populate();

            // Assert

            Assert.AreEqual(record, recordReference.RecordObject);
        }
    }
}
