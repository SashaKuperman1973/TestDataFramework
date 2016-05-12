using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeferredValueGenerator.Concrete;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class SqlWriterCommandTextGeneratorTests
    {
        private TestDataFramework.DeferredValueGenerator.Concrete.SqlWriterCommandTextGenerator textGenerator;

        private Mock<SqlWriterCommandText> sqlWriterCommandTextMock;
        private Mock<IAttributeDecorator> attributeDecoratorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.sqlWriterCommandTextMock = new Mock<SqlWriterCommandText>();
            this.attributeDecoratorMock = new Mock<IAttributeDecorator>();

            this.textGenerator = new TestDataFramework.DeferredValueGenerator.Concrete.SqlWriterCommandTextGenerator(this.attributeDecoratorMock.Object, this.sqlWriterCommandTextMock.Object);
        }

        [TestMethod]
        public void WriteString_Test()
        {
            const string testString = "TestString";

            var tableAttribute = new TableAttribute("CatalogueName", "SchemaName", "TableName");

            PropertyInfo propertyInfo = typeof (PrimaryTable).GetProperty("Key");

            this.sqlWriterCommandTextMock.Setup(
                m => m.GetStringSelect(tableAttribute.CatalogueName, tableAttribute.Schema, tableAttribute.Name, "Key"))
                .Returns(testString);

            this.attributeDecoratorMock.Setup(m => m.GetCustomAttributes<TableAttribute>(typeof(PrimaryTable)))
                .Returns(new TableAttribute[] {tableAttribute});

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
                m => m.GetNumberSelect(tableAttribute.CatalogueName, tableAttribute.Schema, tableAttribute.Name, "Key"))
                .Returns(testString);

            this.attributeDecoratorMock.Setup(m => m.GetCustomAttributes<TableAttribute>(typeof(PrimaryTable)))
                .Returns(new TableAttribute[] { tableAttribute });

            string result = this.textGenerator.WriteNumber(propertyInfo);

            Assert.AreEqual(testString, result);
        }
    }
}
