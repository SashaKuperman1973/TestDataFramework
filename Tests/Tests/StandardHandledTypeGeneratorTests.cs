using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.UniqueValueGenerator.Interface;
using TestDataFramework.ValueGenerator;

namespace Tests.Tests
{
    [TestClass]
    public class StandardHandledTypeGeneratorTests
    {
        private StandardHandledTypeGenerator handledTypeGenerator;
        private Mock<IValueGenerator> valueGeneratorMock;
        private Mock<IValueGenerator> accumulatorValueGeneratorMock;
        private Mock<Random> randomMock;
        private Mock<IUniqueValueGenerator> uniqueValueGenerator;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.valueGeneratorMock = new Mock<IValueGenerator>();
            this.accumulatorValueGeneratorMock = new Mock<IValueGenerator>();
            this.randomMock = new Mock<Random>();
            this.uniqueValueGenerator = new Mock<IUniqueValueGenerator>();

            this.handledTypeGenerator = new StandardHandledTypeGenerator(this.valueGeneratorMock.Object,
                () => this.accumulatorValueGeneratorMock.Object, this.randomMock.Object,
                this.uniqueValueGenerator.Object);
        }

        [TestMethod]
        public void GetObject_KeyValuePair_Test()
        {
            // Arrange

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(int))).Returns(5);
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string))).Returns("ABCD");

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(2);

            // Act

            object result = this.handledTypeGenerator.GetObject(typeof(KeyValuePair<int,string>));

            // Assert

            Assert.AreEqual(new KeyValuePair<int, string>(5, "ABCD"), result);
        }

        [TestMethod]
        public void GetObject_IDictionary_Test()
        {
            // Arrange

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(int))).Returns(5);
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string))).Returns("ABCD");

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(0);

            // Act

            object result = this.handledTypeGenerator.GetObject(typeof(IDictionary<int, string>));

            // Assert

            var dictionary = result as Dictionary<int, string>;

            Assert.IsNotNull(dictionary);
        }
    }
}
