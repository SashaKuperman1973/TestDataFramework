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
using TestDataFramework.UniqueValueGenerator;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardUniqueValueGeneratorTests
    {
        private StandardUniqueValueGenerator generator;
        private Mock<IDeferredValueGenerator<ulong>> deferredValueGeneratorMock;
        private Mock<StringGenerator> stringGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<ulong>>();
            this.stringGeneratorMock = new Mock<StringGenerator>();

            this.generator = new StandardUniqueValueGenerator(this.stringGeneratorMock.Object,
                this.deferredValueGeneratorMock.Object);
        }

        [TestMethod]
        public void IntegerTest()
        {
            this.IntegerTest(typeof(ClassWithIntAutoPrimaryKey));
            this.IntegerTest(typeof(ClassWithShortAutoPrimaryKey));
            this.IntegerTest(typeof(ClassWithLongAutoPrimaryKey));
            this.IntegerTest(typeof(ClassWithByteAutoPrimaryKey));
        }

        private void IntegerTest(Type inputClass)
        {
            DeferredValueGetterDelegate<ulong>[] result = this.Test(inputClass);

            StandardUniqueValueGeneratorTests.AreEqual(5, result[0](5));
            StandardUniqueValueGeneratorTests.AreEqual(6, result[0](5));
        }

        [TestMethod]
        public void StringTest()
        {
            this.stringGeneratorMock.Setup(m => m.GetValue(5, It.IsAny<int>())).Returns("A");
            this.stringGeneratorMock.Setup(m => m.GetValue(6, It.IsAny<int>())).Returns("B");

            DeferredValueGetterDelegate<ulong>[] result = this.Test(typeof(ClassWithStringAutoPrimaryKey));

            StandardUniqueValueGeneratorTests.AreEqual("A", result[0](5));
            StandardUniqueValueGeneratorTests.AreEqual("B", result[1](5));
        }

        private DeferredValueGetterDelegate<ulong>[] Test(Type inputClass)
        {
            PropertyInfo keyPropertyInfo = inputClass.GetProperty("Key");

            int i = 0;

            var delegateArray = new DeferredValueGetterDelegate<ulong>[2];

            this.deferredValueGeneratorMock.Setup(
                m => m.AddDelegate(It.IsAny<PropertyInfo>(), It.IsAny<DeferredValueGetterDelegate<ulong>>()))
                .Callback<PropertyInfo, DeferredValueGetterDelegate<ulong>>((pi, d) => delegateArray[i++] = d);

            // Act

            object value = this.generator.GetValue(keyPropertyInfo);
            this.generator.GetValue(keyPropertyInfo);

            // Assert

            StandardUniqueValueGeneratorTests.AreEqual(
                keyPropertyInfo.PropertyType.IsValueType
                    ? Activator.CreateInstance(keyPropertyInfo.PropertyType)
                    : null,

                value
                );

            return delegateArray;
        }

        private static void AreEqual(object expected, object actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.AreEqual(Convert.ChangeType(expected, actual.GetType()), actual);
            }
            
        }
    }
}
