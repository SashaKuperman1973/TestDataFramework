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
using TestDataFramework.ValueGenerator;

namespace Tests.Tests
{
    [TestClass]
    public class StandardHandledTypeGeneratorTests
    {
        private StandardHandledTypeGenerator typeGenerator;
        private Mock<IValueGenerator> valueGeneratorMock;
        private Mock<Random> randomMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.valueGeneratorMock = new Mock<IValueGenerator>();
            this.randomMock = new Mock<Random>();
            this.typeGenerator = new StandardHandledTypeGenerator(this.valueGeneratorMock.Object, this.randomMock.Object);
        }

        [TestMethod]
        public void GetObject_KeyValuePair_Test()
        {
            // Arrange

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(int))).Returns(5);
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string))).Returns("ABCD");

            // Act

            object result = this.typeGenerator.GetObject(typeof(KeyValuePair<int,string>));

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

            object result = this.typeGenerator.GetObject(typeof(IDictionary<int, string>));

            // Assert


        }
    }
}
