using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class HandlerDictionaryTests
    {
        private HandlerDictionary<int> handlerDictionary;
        private Mock<IDeferredValueGeneratorHandler<int>> handlerMock;

        [TestInitialize]
        public void Initialize()
        {
            this.handlerMock = new Mock<IDeferredValueGeneratorHandler<int>>();
            this.handlerDictionary = new HandlerDictionary<int>(this.handlerMock.Object);
        }

        [TestMethod]
        public void KeyNotFoundException_Test()
        {
            // Act. Assert.

            Helpers.ExceptionTest(
                () => { HandlerDelegate<int> value = this.handlerDictionary[typeof (SubjectClass)]; },
                typeof (KeyNotFoundException), string.Format(Messages.PropertyKeyNotFound, typeof (SubjectClass)));
        }

        [TestMethod]
        public void Indexer_Integral_Test()
        {
            // Act. Assert.

            new[] {typeof (int), typeof (short), typeof (long), typeof (byte)}.ToList().ForEach(t =>
            {
                HandlerDelegate<int> handler = this.handlerDictionary[t];

                Assert.AreEqual(this.handlerMock.Object.NumberHandler, handler);
            });
        }

        [TestMethod]
        public void Indexer_String_Test()
        {
            // Act

            HandlerDelegate<int> handler = this.handlerDictionary[typeof(string)];

            // Assert

            Assert.AreEqual(this.handlerMock.Object.StringHandler, handler);
        }
    }
}
