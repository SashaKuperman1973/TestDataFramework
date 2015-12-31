using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using Tests.TestModels;

namespace Tests
{
    [TestClass]
    public class RecordReferenceTests
    {
        [TestMethod]
        public void ThrowsWhenTypeMismatch_Test()
        {
            // Arrange

            var primaryTable = new ForeignKeyMismatchPrimaryTable();
            var foreignTable = new ForeignKeyMismatchPrimaryTable();

            var primaryRecordReference = new RecordReference<ForeignKeyMismatchPrimaryTable>(primaryTable);
            var foreignRecordReference = new RecordReference<ForeignKeyMismatchPrimaryTable>(foreignTable);


            // Act

            Helpers.ExceptionTest(() => foreignRecordReference.AddPrimaryRecordReference(primaryRecordReference),
                typeof (NoReferentialIntegrityException),
                string.Format(Messages.NoReferentialIntegrity,
                    Helper.PrintType(typeof (ForeignKeyMismatchPrimaryTable)),
                    Helper.PrintType(typeof (ForeignKeyMismatchPrimaryTable))));
        }
    }
}
