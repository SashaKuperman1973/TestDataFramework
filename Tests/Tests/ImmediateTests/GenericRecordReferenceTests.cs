using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using Tests.Mocks;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class GenericRecordReferenceTests
    {
        private Mock<BasePopulator> populatorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.populatorMock = new Mock<BasePopulator>(null);
        }

        [TestMethod]
        public void BindAndMake_Test()
        {
            // Arrange

            var recordReference =
                new RecordReference<SubjectClass>(null, null, this.populatorMock.Object, null);

            // Act

            recordReference.BindAndMake();

            // Assert

            this.populatorMock.Verify(m => m.Bind());
        }

        [TestMethod]
        public void Make_Test()
        {
            // Arrange

            var recordReference =
                new RecordReference<SubjectClass>(null, null, this.populatorMock.Object, null);

            // Act

            recordReference.Make();

            // Assert

            this.populatorMock.Verify(m => m.Bind(recordReference));
        }
    }
}
