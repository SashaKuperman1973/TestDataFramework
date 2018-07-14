/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.Persistence.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.RepositoryOperations.Model;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class MemoryPersistenceTests
    {
        private IAttributeDecorator attributeDecorator;

        [TestInitialize]
        public void Initialize()
        {
            this.attributeDecorator = new StandardAttributeDecorator(null, new AssemblyWrapper(), new Schema());
        }

        [TestMethod]
        public void Persist_Test()
        {
            // Arrange

            var deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<LargeInteger>>();

            var persistence = new MemoryPersistence(deferredValueGeneratorMock.Object, this.attributeDecorator);

            var primaryTable = new PrimaryTable {Integer = 5, Text = "Text"};

            var primaryRecordReference = new RecordReference<PrimaryTable>(
                Helpers.GetTypeGeneratorMock(primaryTable).Object,
                this.attributeDecorator, null, null, null, null);

            var recordReferenceArray = new RecordReference[] {primaryRecordReference};

            // Act

            persistence.Persist(recordReferenceArray);

            // Assert

            deferredValueGeneratorMock.Verify(
                m => m.Execute(It.Is<IEnumerable<RecordReference>>(e => e.First() == recordReferenceArray[0])),
                Times.Once);
        }

        [TestMethod]
        public void Persists_KeyMapping_Test()
        {
            // Arrange

            var primaryTable = new ManualKeyPrimaryTable {Key1 = "ABCD", Key2 = 5};
            var primaryReference =
                new RecordReference<ManualKeyPrimaryTable>(Helpers.GetTypeGeneratorMock(primaryTable).Object,
                    this.attributeDecorator, null, null, null, null);

            var foreignTable = new ManualKeyForeignTable();
            var foreignReference =
                new RecordReference<ManualKeyForeignTable>(Helpers.GetTypeGeneratorMock(foreignTable).Object,
                    this.attributeDecorator, null, null, null, null);

            foreignReference.AddPrimaryRecordReference(primaryReference);

            var deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<LargeInteger>>();
            var persistence = new MemoryPersistence(deferredValueGeneratorMock.Object, this.attributeDecorator);

            // Act

            primaryReference.Populate();
            foreignReference.Populate();
            persistence.Persist(new RecordReference[] {primaryReference, foreignReference});

            // Assert

            Assert.AreEqual(primaryTable.Key1, foreignTable.ForeignKey1);
            Assert.AreEqual(primaryTable.Key2, foreignTable.ForeignKey2);
        }

        [TestMethod]
        public void Persist_NoPrimaryKey_ForForeignKey_Test()
        {
            var deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<LargeInteger>>();
            var attributeDecoratorMock = new Mock<IAttributeDecorator>();

            var persistence = new MemoryPersistence(deferredValueGeneratorMock.Object, attributeDecoratorMock.Object);

            var recordReference = new RecordReference<SubjectClass>(null, null, null, null, null, null);
            var primaryKeyReference = new RecordReference<SecondClass>(null, null, null, null, null, null);
            recordReference.PrimaryKeyReferences.Add(primaryKeyReference);

            attributeDecoratorMock.Setup(m => m.GetPropertyAttributes<PrimaryKeyAttribute>(typeof(SecondClass)))
                .Returns(new[]
                {
                    new PropertyAttribute<PrimaryKeyAttribute>
                    {
                        PropertyInfo = typeof(SecondClass).GetProperty(nameof(SecondClass.SecondInteger))
                    }
                });

            var foreignKeyPropertyAttribute = new PropertyAttribute<ForeignKeyAttribute>();

            attributeDecoratorMock.Setup(m => m.GetPropertyAttributes<ForeignKeyAttribute>(typeof(SubjectClass)))
                .Returns(new[] { foreignKeyPropertyAttribute });

            persistence.Persist(new [] { recordReference });

            Assert.IsNull(foreignKeyPropertyAttribute.PropertyInfo);
        }
    }
}