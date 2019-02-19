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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Persistence.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class SqlClientPersistenceServiceTests
    {
        private Mock<IAttributeDecorator> attributeDecoratorMock;
        private Mock<IWritePrimitives> writePrimitivesMock;

        private SqlClientPersistenceService service;

        [TestInitialize]
        public void Initialize()
        {
            this.attributeDecoratorMock = new Mock<IAttributeDecorator>();
            this.writePrimitivesMock = new Mock<IWritePrimitives>();

            this.service = new SqlClientPersistenceService(
                this.attributeDecoratorMock.Object,
                this.writePrimitivesMock.Object);
        }

        [TestMethod]
        public void GetOperations_Test()
        {
            Mock<RecordReference> referenceMockA = Helpers.GetMock<RecordReference>();
            Mock<RecordReference> referenceMockB = Helpers.GetMock<RecordReference>();

            new[] {referenceMockA, referenceMockB}.ToList().ForEach(mock =>
            {
                mock.SetupGet(m => m.RecordType).Returns(typeof(PrimaryTable));
                mock.SetupGet(m => m.RecordObjectBase).Returns(new PrimaryTable());
            });

            var recordReferences = new List<RecordReference> { referenceMockA.Object, referenceMockB.Object };

            IEnumerable<AbstractRepositoryOperation> operations = this.service.GetOperations(true, recordReferences);

            Assert.IsNotNull(operations);
            Assert.AreEqual(2, operations.Count());
            Assert.IsNotNull(operations.ElementAt(0));
            Assert.IsNotNull(operations.ElementAt(1));
        }

        [TestMethod]
        public void WriteOperations_Test()
        {
            var operationA = new Mock<AbstractRepositoryOperation>();
            var operationB = new Mock<AbstractRepositoryOperation>();
            var operations = new[] {operationA.Object, operationB.Object};

            var orderedOperations = new AbstractRepositoryOperation[2];

            this.service.WriteOperations(operations.ToList(), orderedOperations);

            new[] {operationA, operationB}.ToList().ForEach(operation =>
            {
                operation.Verify(m => m.Write(
                    It.IsAny<CircularReferenceBreaker>(), 
                    this.writePrimitivesMock.Object, 
                    It.IsAny<Counter>(), 
                    orderedOperations));
            });
        }

        [TestMethod]
        public void ReadOrderedOperations_Test()
        {
            var operationA = new Mock<AbstractRepositoryOperation>();
            var operationB = new Mock<AbstractRepositoryOperation>();
            var orderedOperations = new[] { operationA.Object, operationB.Object };

            var returnValues = new object[0];
            this.writePrimitivesMock.Setup(m => m.Execute()).Returns(returnValues);

            this.service.ReadOrderedOperations(orderedOperations);

            new[] { operationA, operationB }.ToList().ForEach(operation =>
            {
                operation.Verify(m => m.Read(
                    It.IsAny<Counter>(), returnValues
                ));
            });
        }
    }
}
