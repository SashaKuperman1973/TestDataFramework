/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeferredValueGenerator.Concrete;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class SqlWriterCommandTextGeneratorTests
    {
        private Mock<IAttributeDecorator> attributeDecoratorMock;

        private Mock<SqlWriterCommandText> sqlWriterCommandTextMock;
        private SqlWriterCommandTextGenerator textGenerator;

        [TestInitialize]
        public void Initialize()
        {
            this.sqlWriterCommandTextMock = new Mock<SqlWriterCommandText>();
            this.attributeDecoratorMock = new Mock<IAttributeDecorator>();

            this.textGenerator = new SqlWriterCommandTextGenerator(this.attributeDecoratorMock.Object,
                this.sqlWriterCommandTextMock.Object);
        }

        [TestMethod]
        public void WriteString_Test()
        {
            const string testString = "TestString";

            var tableAttribute = new TableAttribute("CatalogueName", "SchemaName", "TableName");

            PropertyInfo propertyInfo = typeof(PrimaryTable).GetProperty("Key");

            this.sqlWriterCommandTextMock.Setup(
                    m => m.GetStringSelect(tableAttribute.CatalogueName, tableAttribute.Schema, tableAttribute.Name,
                        "Key"))
                .Returns(testString);

            this.attributeDecoratorMock.Setup(m => m.GetCustomAttributes<TableAttribute>(typeof(PrimaryTable)))
                .Returns(new[] {tableAttribute});

            string result = this.textGenerator.WriteString(propertyInfo);

            Assert.AreEqual(testString, result);
        }

        [TestMethod]
        public void WriteNumber_Test()
        {
            const string testString = "TestString";

            var tableAttribute = new TableAttribute("CatalogueName", "SchemaName", "TableName");

            PropertyInfo propertyInfo = typeof(PrimaryTable).GetProperty("Key");

            this.sqlWriterCommandTextMock.Setup(
                    m => m.GetNumberSelect(tableAttribute.CatalogueName, tableAttribute.Schema, tableAttribute.Name,
                        "Key"))
                .Returns(testString);

            this.attributeDecoratorMock.Setup(m => m.GetCustomAttributes<TableAttribute>(typeof(PrimaryTable)))
                .Returns(new[] {tableAttribute});

            string result = this.textGenerator.WriteNumber(propertyInfo);

            Assert.AreEqual(testString, result);
        }
    }
}