using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.ValueGenerator.Concrete;
using TestDataFramework.ValueProvider.Interfaces;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class SqlClientValueGeneratorTests
    {
        private Mock<IAttributeDecorator> attributeDecoratorMock;
        private SqlClientValueGenerator sqlClientValueGenerator;

        private Mock<IValueProvider> valueProviderMock;

        [TestInitialize]
        public void Initialize()
        {
            this.valueProviderMock = new Mock<IValueProvider>();
            this.attributeDecoratorMock = new Mock<IAttributeDecorator>();

            this.sqlClientValueGenerator = new SqlClientValueGenerator(this.valueProviderMock.Object, null, null, null,
                this.attributeDecoratorMock.Object);
        }

        [TestMethod]
        public void GetDateTime_Test()
        {
            // Arrange

            DateTime now = DateTime.Now;

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty(nameof(SubjectClass.DateTime));

            this.valueProviderMock.Setup(m => m.GetDateTime(It.IsAny<PastOrFuture?>(), It.IsAny<Func<long?, long>>(),
                It.IsAny<long?>(), It.IsAny<long?>())).Returns(now);

            this.attributeDecoratorMock.Setup(m => m.GetCustomAttribute<PastOrFutureAttribute>(propertyInfo))
                .Returns(new PastOrFutureAttribute(PastOrFuture.Past));

            // Act

            object result = this.sqlClientValueGenerator.GetValue(propertyInfo, typeof(DateTime));

            // Assert

            Assert.IsNotNull(result);
            Assert.IsTrue(result is DateTime);
            var dateTime = (DateTime) result;
            Assert.AreNotEqual(default(DateTime), dateTime);

            Assert.IsTrue(now >= dateTime && now <= dateTime.AddMilliseconds(1));
        }

        [TestMethod]
        public void GetGuid_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty(nameof(SubjectClass.AGuid));

            // Act

            object result = this.sqlClientValueGenerator.GetValue(propertyInfo, typeof(Guid));

            // Assert

            Assert.AreEqual(default(Guid), result);
        }
    }
}