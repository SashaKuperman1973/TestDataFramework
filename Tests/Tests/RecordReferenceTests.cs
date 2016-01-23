using System;
using System.Linq;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
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

            var primaryRecordReference = new RecordReference<T1>(primaryTable, null);
            var foreignRecordReference = new RecordReference<T2>(foreignTable, null);

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

            var primaryRecordReference = new RecordReference<PrimaryTable>(primaryTable, null);
            var foreignRecordReference = new RecordReference<ForeignTable>(foreignTable, null);

            // Act

            foreignRecordReference.AddPrimaryRecordReference(primaryRecordReference);

            // Assert

            Assert.AreEqual(1, foreignRecordReference.PrimaryKeyReferences.Count());
        }

        [TestMethod]
        public void Set_Test()
        {
            var record = new PrimaryTable();
            var recordReference = new RecordReference<PrimaryTable>(record, null);

            throw new NotImplementedException();
        }

        [TestMethod]
        public void Set_ToMethodThrows_Test()
        {
            var record = new PrimaryTable();
            var recordReference = new RecordReference<PrimaryTable>(record, null);

            throw new NotImplementedException();
        }

        [TestMethod]
        public void Set_ToFieldThrows_Test()
        {
            var record = new PrimaryTable();
            var recordReference = new RecordReference<PrimaryTable>(record, null);

            throw new NotImplementedException();
        }

    }
}
