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

            Mock<IDeferredValueGenerator<ulong>> deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<ulong>>();

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
    }
}
