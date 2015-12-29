using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Persistence;
using TestDataFramework.Populator;
using Tests.TestModels;

namespace Tests
{
    [TestClass]
    public class SqlServerPersistenceTests
    {
        private StandardPersistence persistence;

        [TestInitialize]
        public void Initialize()
        {
            this.persistence = new StandardPersistence();
        }

        [TestMethod]
        public void ForeignKeyBinding_Test()
        {
            var primaryTable = new PrimaryTable();
            var primaryRecordReference = new RecordReference<PrimaryTable>(primaryTable);

            var foreignTable = new ForeignTable();
            var foreignRecordReference = new RecordReference<ForeignTable>(foreignTable);

            primaryRecordReference.AddPrimaryRecordReference(foreignRecordReference);

            // Act

            this.persistence.Persist(new RecordReference[] { primaryRecordReference, foreignRecordReference });

            // Assert

            throw new NotImplementedException();
        }

        [TestMethod]
        public void AutoPrimaryKeyGeneration_Test()
        {
            RecordReference[] recordReferenceList =
            {
                new RecordReference<PrimaryTable>(new PrimaryTable()),

                new RecordReference<ForeignTable>(new ForeignTable()),

                new RecordReference<PrimaryTable>(new PrimaryTable()),

                new RecordReference<ForeignTable>(new ForeignTable()),
            };


            // Act

            this.persistence.Persist(recordReferenceList);

            // Assert

            int initialPrimaryTableKey = ((PrimaryTable) recordReferenceList[0].RecordObject).Key;
            int subsequentPrimaryTableKey = ((PrimaryTable)recordReferenceList[2].RecordObject).Key;

            Assert.IsTrue(subsequentPrimaryTableKey == initialPrimaryTableKey + 1);

            int initialForeignTableKey = ((ForeignTable)recordReferenceList[1].RecordObject).Key;
            int subsequentForeignTableKey = ((ForeignTable)recordReferenceList[3].RecordObject).Key;

            Assert.IsTrue(subsequentForeignTableKey == initialForeignTableKey + 1);
        }
    }
}
