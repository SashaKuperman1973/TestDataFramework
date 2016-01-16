using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.UniqueValueGenerator;
using TestDataFramework.UniqueValueGenerator.Concrete;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class MemoryUniqueValueGeneratorTests
    {
        private MemoryUniqueValueGenerator generator;
        private Mock<IPropertyValueAccumulator> propertyValueAccumulatorMock;
        private Mock<IDeferredValueGenerator<ulong>> deferredValueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.propertyValueAccumulatorMock = new Mock<IPropertyValueAccumulator>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<ulong>>();

            this.generator = new MemoryUniqueValueGenerator(this.propertyValueAccumulatorMock.Object,
                this.deferredValueGeneratorMock.Object);
        }

        [TestMethod]
        public void DeferValue_Test()
        {
            throw new NotImplementedException();

            // Arange

            const int initialCount = 5;

            PropertyInfo keyPropertyInfo = typeof(ClassWithStringAutoPrimaryKey).GetProperty("Key");
            this.propertyValueAccumulatorMock.Setup(m => m.GetValue(It.Is<PropertyInfo>(pi => pi == keyPropertyInfo), initialCount)).Returns(1);

            int i = 0;

            var delegateArray = new DeferredValueGetterDelegate<ulong>[2];

            this.deferredValueGeneratorMock.Setup(
                m => m.AddDelegate(It.IsAny<PropertyInfo>(), It.IsAny<DeferredValueGetterDelegate<ulong>>()))
                .Callback<PropertyInfo, DeferredValueGetterDelegate<ulong>>((pi, d) => delegateArray[i++] = d);

            // Act
            /*
            this.generator.DeferValue(keyPropertyInfo);
            this.generator.DeferValue(keyPropertyInfo);
            */
            // Assert

            Assert.AreEqual(1, delegateArray[0](initialCount));
            Assert.AreEqual(1, delegateArray[1](initialCount));
        }

        [TestMethod]
        public void GetValue_Test()
        {
            // Arrange

            const int expectedValue = 5;

            PropertyInfo keyPropertyInfo = typeof(ClassWithStringAutoPrimaryKey).GetProperty("Key");
            this.propertyValueAccumulatorMock.Setup(m => m.GetValue(It.Is<PropertyInfo>(pi => pi == keyPropertyInfo), 0)).Returns(expectedValue);

            // Act

            object result = this.generator.GetValue(keyPropertyInfo);

            // Assert

            Assert.AreEqual(expectedValue, result);
        }
    }
}
