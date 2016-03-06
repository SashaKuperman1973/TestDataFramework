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

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class SqlWriterCommandTextGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlWriterCommandTextGenerator));

        private readonly IAttributeDecorator attributeDecorator;

        public SqlWriterCommandTextGenerator(IAttributeDecorator attributeDecorator)
        {
            this.attributeDecorator = attributeDecorator;
        }

        public virtual string WriteString(PropertyInfo propertyInfo)
        {
            SqlWriterCommandTextGenerator.Logger.Debug("Entering WriteString. propertyInfo:" + propertyInfo.GetExtendedMemberInfoString());

            string tableName = "[" + Helper.GetTableName(propertyInfo.DeclaringType, this.attributeDecorator) + "]";
            string columnName = "[" + Helper.GetColumnName(propertyInfo, this.attributeDecorator) + "]";

            string result =
                $"Select Max({columnName}) from {tableName} where {columnName} not like '%[^A-Z]%' And LEN({columnName}) = (Select Max(Len({columnName})) From {tableName} where {columnName} not like '%[^A-Z]%' )";

            SqlWriterCommandTextGenerator.Logger.Debug($"Result statement: {result}");

            SqlWriterCommandTextGenerator.Logger.Debug("Exiting WriteString");
            return result;
        }

        public virtual string WriteNumber(PropertyInfo propertyInfo)
        {
            SqlWriterCommandTextGenerator.Logger.Debug("Entering WriteNumber. propertyInfo:" + propertyInfo.GetExtendedMemberInfoString());

            string result = $"Select MAX([{Helper.GetColumnName(propertyInfo, this.attributeDecorator)}]) From [{Helper.GetTableName(propertyInfo.DeclaringType, this.attributeDecorator)}]";

            SqlWriterCommandTextGenerator.Logger.Debug($"Result statement: {result}");

            SqlWriterCommandTextGenerator.Logger.Debug("Exiting WriteNumber");
            return result;
        }
    }
}