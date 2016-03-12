/*
    Copyright 2016 Alexander Kuperman

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
using log4net;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.Helpers;
using TestDataFramework.WritePrimitives;
using TestDataFramework.WritePrimitives.Concrete;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class SqlWriterCommandText
    {
        public virtual string GetStringSelect(string catalogueName, string schema, string tableName, string columnName)
        {
            string fullTableName = SqlClientWritePrimitives.BuildFullTableName(catalogueName, schema, tableName);
            columnName = "[" + columnName + "]";

            string result =
                $"Select Max({columnName}) from {fullTableName} where {columnName} not like '%[^A-Z]%' And LEN({columnName}) = (Select Max(Len({columnName})) From {fullTableName} where {columnName} not like '%[^A-Z]%' );";

            return result;
        }

        public virtual string GetNumberSelect(string catalogueName, string schema, string tableName, string columnName)
        {
            string fullTableName = SqlClientWritePrimitives.BuildFullTableName(catalogueName, schema, tableName);

            string result = $"Select MAX([{columnName}]) From {fullTableName};";

            return result;
        }
    }

    public class SqlWriterCommandTextGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlWriterCommandTextGenerator));

        private readonly IAttributeDecorator attributeDecorator;
        private readonly SqlWriterCommandText sqlWriterCommandText;

        public SqlWriterCommandTextGenerator(IAttributeDecorator attributeDecorator, SqlWriterCommandText sqlWriterCommandText)
        {
            this.attributeDecorator = attributeDecorator;
            this.sqlWriterCommandText = sqlWriterCommandText;
        }

        public virtual string WriteString(PropertyInfo propertyInfo)
        {
            SqlWriterCommandTextGenerator.Logger.Debug("Entering WriteString");

            string result = this.Write(propertyInfo, this.sqlWriterCommandText.GetStringSelect);

            SqlWriterCommandTextGenerator.Logger.Debug("Exiting WriteString");
            return result;
        }

        public virtual string WriteNumber(PropertyInfo propertyInfo)
        {
            SqlWriterCommandTextGenerator.Logger.Debug("Entering WriteNumber");

            string result = this.Write(propertyInfo, this.sqlWriterCommandText.GetNumberSelect);

            SqlWriterCommandTextGenerator.Logger.Debug("Exiting WriteNumber");
            return result;
        }

        private delegate string GetSelectDelegate(
            string catalogueName, string schema, string tableName, string columnName);

        private string Write(PropertyInfo propertyInfo, GetSelectDelegate getSelectDelegate)
        {
            SqlWriterCommandTextGenerator.Logger.Debug("Entering Write. propertyInfo:" + propertyInfo.GetExtendedMemberInfoString());

            TableName tableName = Helper.GetTableName(propertyInfo.DeclaringType, this.attributeDecorator);
            string columnName = Helper.GetColumnName(propertyInfo, this.attributeDecorator);

            string result = getSelectDelegate(tableName.CatalogueName, tableName.Schema, tableName.Name, columnName);

            SqlWriterCommandTextGenerator.Logger.Debug($"Result statement: {result}");

            SqlWriterCommandTextGenerator.Logger.Debug("Exiting Write");
            return result;
        }
    }
}