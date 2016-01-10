using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.UniqueValueGenerator;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class DbProviderDeferredValueGeneratorTest
    {
        private const string ConnectionString = "cn";
        private DbProviderDeferredValueGenerator<ulong> generator;
        private Mock<IHandlerDictionary<ulong>> handlerDictionaryMock;

        private Mock<DbProviderFactory> dbProviderFactoryMock;
        private Mock<DbConnection> dbConnnectionMock;
        private Mock<DbCommand> dbCommandMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.handlerDictionaryMock = new Mock<IHandlerDictionary<ulong>>();
            this.dbProviderFactoryMock = new Mock<DbProviderFactory>();
            this.dbConnnectionMock = new Mock<DbConnection>();
            this.dbCommandMock = new Mock<DbCommand>();

            this.generator = new DbProviderDeferredValueGenerator<ulong>(this.handlerDictionaryMock.Object,
                this.dbProviderFactoryMock.Object, DbProviderDeferredValueGeneratorTest.ConnectionString);

            this.dbProviderFactoryMock.Setup(m => m.CreateConnection()).Returns(this.dbConnnectionMock.Object);
            this.dbProviderFactoryMock.Setup(m => m.CreateCommand()).Returns(this.dbCommandMock.Object);
        }

        [TestMethod]
        public void FillData_Test()
        {
            // Arrange

            PropertyInfo integerPropertyInfo = typeof(SubjectClass).GetProperty("Integer");
            PropertyInfo longIntegerPropertyInfo = typeof(SubjectClass).GetProperty("LongInteger");

            var dictionary = new Dictionary<PropertyInfo, StandardDeferredValueGenerator<ulong>.Data>
            {
                {integerPropertyInfo, new StandardDeferredValueGenerator<ulong>.Data(null)},
                {longIntegerPropertyInfo, new StandardDeferredValueGenerator<ulong>.Data(null)},
            };

            this.handlerDictionaryMock.Setup(m => m[typeof(int)]).Returns((pi, c) => 1);
            this.handlerDictionaryMock.Setup(m => m[typeof(long)]).Returns((pi, c) => 2);

            // Act

            this.generator.FillData(dictionary);

            // Assert

            this.dbConnnectionMock.VerifySet(m => m.ConnectionString = DbProviderDeferredValueGeneratorTest.ConnectionString);
            this.dbCommandMock.VerifySet(m => m.Connection = this.dbConnnectionMock.Object);

            Assert.AreEqual(1UL, dictionary[integerPropertyInfo].Item);
            Assert.AreEqual(2UL, dictionary[longIntegerPropertyInfo].Item);
        }
    }
}
