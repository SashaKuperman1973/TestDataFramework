/*
    Copyright 2016, 2017 Alexander Kuperman

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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.DeferredValueGenerator.Concrete;

namespace Tests.Tests
{
    [TestClass]
    public class SqlWriterCommandTextGeneratorTest
    {
        private const string CatalogueName = "CatalogueName";
        private const string Schema = "Schema";
        private const string TableName = "TableName";
        private const string ColumnName = "ColumnName";

        [TestMethod]
        public void GetStringSelect_Test()
        {
            var sqlWriterCommandText = new SqlWriterCommandText();

            var result = sqlWriterCommandText.GetStringSelect(SqlWriterCommandTextGeneratorTest.CatalogueName,
                SqlWriterCommandTextGeneratorTest.Schema, SqlWriterCommandTextGeneratorTest.TableName,
                SqlWriterCommandTextGeneratorTest.ColumnName);

            var expected = $"Select Max([{SqlWriterCommandTextGeneratorTest.ColumnName}])" +
                           $" from [{SqlWriterCommandTextGeneratorTest.CatalogueName}].[{SqlWriterCommandTextGeneratorTest.Schema}].[{SqlWriterCommandTextGeneratorTest.TableName}]" +
                           $" where [{SqlWriterCommandTextGeneratorTest.ColumnName}] not like '%[^A-Z]%' And LEN([{SqlWriterCommandTextGeneratorTest.ColumnName}])" +
                           $" = (Select Max(Len([{SqlWriterCommandTextGeneratorTest.ColumnName}]))" +
                           $" From [{SqlWriterCommandTextGeneratorTest.CatalogueName}].[{SqlWriterCommandTextGeneratorTest.Schema}].[{SqlWriterCommandTextGeneratorTest.TableName}]" +
                           $" where [{SqlWriterCommandTextGeneratorTest.ColumnName}] not like '%[^A-Z]%' );";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetNumberSelect_Test()
        {
            var sqlWriterCommandText = new SqlWriterCommandText();

            var result = sqlWriterCommandText.GetNumberSelect(SqlWriterCommandTextGeneratorTest.CatalogueName,
                SqlWriterCommandTextGeneratorTest.Schema, SqlWriterCommandTextGeneratorTest.TableName,
                SqlWriterCommandTextGeneratorTest.ColumnName);

            var expected = $"Select MAX([{SqlWriterCommandTextGeneratorTest.ColumnName}]) From" +
                           $" [{SqlWriterCommandTextGeneratorTest.CatalogueName}].[{SqlWriterCommandTextGeneratorTest.Schema}].[{SqlWriterCommandTextGeneratorTest.TableName}];";

            Assert.AreEqual(expected, result);
        }
    }
}