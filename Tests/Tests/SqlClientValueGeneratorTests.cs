/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.ValueGenerator.Concrete;
using TestDataFramework.ValueProvider.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
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

            object result = this.sqlClientValueGenerator.GetValue(propertyInfo, typeof(DateTime), null);

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

            object result = this.sqlClientValueGenerator.GetValue(propertyInfo, typeof(Guid), null);

            // Assert

            Assert.AreEqual(default(Guid), result);
        }
    }
}