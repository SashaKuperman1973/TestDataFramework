using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class UniqueValueTypeGeneratorTests
    {
        private Mock<IValueGenerator> accumulatorValueGeneratorMock;
        private Mock<IHandledTypeGenerator> handledTypeGeneratorMock;
        private Mock<ITypeGeneratorService> typeGeneratorServiceMock;
        private UniqueValueTypeGenerator uniqueValueTypeGenerator;
        private Mock<IValueGenerator> valueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.accumulatorValueGeneratorMock = new Mock<IValueGenerator>();
            this.valueGeneratorMock = new Mock<IValueGenerator>();
            this.handledTypeGeneratorMock = new Mock<IHandledTypeGenerator>();
            this.typeGeneratorServiceMock = new Mock<ITypeGeneratorService>();

            this.uniqueValueTypeGenerator = new UniqueValueTypeGenerator(
                typeGenerator => this.accumulatorValueGeneratorMock.Object,
                this.valueGeneratorMock.Object,
                this.handledTypeGeneratorMock.Object,
                this.typeGeneratorServiceMock.Object);
        }

        [TestMethod]
        public void Unique_SetProperties_Test()
        {
            // Arrange

            this.typeGeneratorServiceMock.Setup(
                    m => m.GetExplicitlySetPropertySetters(It.IsAny<IEnumerable<ExplicitPropertySetter>>(),
                        It.IsAny<ObjectGraphNode>()))
                .Returns(Enumerable.Empty<ExplicitPropertySetter>());

            int uniqueReturnValue = 5;

            this.accumulatorValueGeneratorMock.Setup(
                m => m.GetValue(
                    It.Is<PropertyInfo>(propertyInfo => propertyInfo.Name ==
                                                        nameof(ClassWithValueAndRefernceTypeProperties.AnInteger)),
                    It.IsAny<ObjectGraphNode>())).Returns(uniqueReturnValue);

            // Act

            object result =
                this.uniqueValueTypeGenerator.GetObject<ClassWithValueAndRefernceTypeProperties>(
                    new List<ExplicitPropertySetter>());

            // Assert

            var subject = result as ClassWithValueAndRefernceTypeProperties;
            Assert.IsNotNull(subject);
            Assert.AreEqual(uniqueReturnValue, subject.AnInteger);
        }

        [TestMethod]
        public void Base_SetProperties_Test()
        {
            // Arrange

            var referenceReturnValue = new object();

            this.typeGeneratorServiceMock.Setup(
                    m => m.GetExplicitlySetPropertySetters(It.IsAny<IEnumerable<ExplicitPropertySetter>>(),
                        It.IsAny<ObjectGraphNode>()))
                .Returns(Enumerable.Empty<ExplicitPropertySetter>());

            this.valueGeneratorMock.Setup(
                m => m.GetValue(
                    It.Is<PropertyInfo>(propertyInfo => propertyInfo.Name ==
                                                        nameof(ClassWithValueAndRefernceTypeProperties.ARefernce)),
                    It.IsAny<ObjectGraphNode>())).Returns(referenceReturnValue);

            // Act

            object result =
                this.uniqueValueTypeGenerator.GetObject<ClassWithValueAndRefernceTypeProperties>(
                    new List<ExplicitPropertySetter>());

            // Assert

            var subject = result as ClassWithValueAndRefernceTypeProperties;
            Assert.IsNotNull(subject);
            Assert.AreEqual(referenceReturnValue, subject.ARefernce);
        }
    }
}