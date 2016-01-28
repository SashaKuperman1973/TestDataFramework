using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Persistence;
using TestDataFramework.Populator;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class MemoryPersistenceTests
    {
        [TestMethod]
        public void Persist_Test()
        {
            // Arrange

            var deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<ulong>>();

            var persistence = new MemoryPersistence(deferredValueGeneratorMock.Object);

            var primaryTable = new PrimaryTable { Integer = 5, Text = "Text" };

            var primaryRecordReference = new RecordReference<PrimaryTable>(
                Helpers.GetTypeGeneratorMock(primaryTable).Object
                );

            var recordReferenceArray = new RecordReference[] { primaryRecordReference };

            // Act

            persistence.Persist(recordReferenceArray);

            // Assert

            deferredValueGeneratorMock.Verify(
                m => m.Execute(It.Is<IEnumerable<object>>(e => e.First() == recordReferenceArray[0].RecordObject)),
                Times.Once);
        }

        [TestMethod]
        public void Persists_KeyMapping_Test()
        {
            // Arrange

            var primaryTable = new ManualKeyPrimaryTable { Key1 = "ABCD", Key2 = 5 };
            var primaryReference = new RecordReference<ManualKeyPrimaryTable>(Helpers.GetTypeGeneratorMock(primaryTable).Object);
                
            var foreignTable = new ManualKeyForeignTable();
            var foreignReference =
                new RecordReference<ManualKeyForeignTable>(Helpers.GetTypeGeneratorMock(foreignTable).Object);

            foreignReference.AddPrimaryRecordReference(primaryReference);

            var deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<ulong>>();
            var persistence = new MemoryPersistence(deferredValueGeneratorMock.Object);

            // Act

            primaryReference.Populate();
            foreignReference.Populate();
            persistence.Persist(new RecordReference[] { primaryReference, foreignReference});

            // Assert

            Assert.AreEqual(primaryTable.Key1, foreignTable.ForeignKey1);
            Assert.AreEqual(primaryTable.Key2, foreignTable.ForeignKey2);
        }
    }
}
