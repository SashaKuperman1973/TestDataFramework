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

using System;

namespace TestDataFramework.AttributeDecorator
{
    public class Table
    {
        public enum HasTableAttributeEnum
        {
            True,
            False,
            NotSet
        }

        public Table(ForeignKeyAttribute foreignKeyAttribute)
        {
            this.TableName = foreignKeyAttribute.PrimaryTableName;
            this.Schema = foreignKeyAttribute.Schema;
            this.HasTableAttribute = HasTableAttributeEnum.NotSet;
        }

        public Table(Type type)
        {
            this.TableName = type.Name;
            this.HasTableAttribute = HasTableAttributeEnum.False;
        }

        public Table(TableAttribute tableAttribute)
        {
            this.TableName = tableAttribute.Name;
            this.Schema = tableAttribute.Schema;
            this.CatalogueName = tableAttribute.CatalogueName;
            this.HasTableAttribute = HasTableAttributeEnum.True;
        }

        public HasTableAttributeEnum HasTableAttribute { get; }
        public string CatalogueName { get; }
        public string Schema { get; } = "dbo";
        public string TableName { get; }

        public override int GetHashCode()
        {
            int result = (this.Schema?.GetHashCode() ?? 0) ^ (this.CatalogueName?.GetHashCode() ?? 0) ^
                         this.TableName.GetHashCode();

            return result;
        }

        public override bool Equals(object obj)
        {
            var table = obj as Table;

            bool result = table != null &&

                          (table.Schema == null && this.Schema == null ||
                           (table.Schema?.Equals(this.Schema) ?? false)) &&

                          (table.CatalogueName == null && this.CatalogueName == null ||
                           (table.CatalogueName?.Equals(this.CatalogueName) ?? false)) &&

                           table.TableName.Equals(this.TableName);

            return result;
        }

        public override string ToString()
        {
            string result =
                $"Schema: {this.Schema ?? "<null>"}, TableName: {this.TableName}, CatalogueName: {this.CatalogueName ?? "<null>"}, HasTableAttribute: {this.HasTableAttribute}";

            return result;
        }
    }
}
