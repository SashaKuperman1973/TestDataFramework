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

using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.Helpers;

namespace TestDataFramework.AttributeDecorator
{
    public class Table
    {
        // Constructs input to check against dictionary contents when looking up an input.
        public Table(ForeignKeyAttribute foreignKeyAttribute, TableAttribute tableAttribute)
        {
            foreignKeyAttribute.IsNotNull(nameof(foreignKeyAttribute));

            this.HasTableAttribute = tableAttribute != null;
            this.CatalogueName = tableAttribute?.CatalogueName;
            this.TableName = foreignKeyAttribute.PrimaryTableName;
            this.Schema = foreignKeyAttribute.Schema;
        }

        // Constructs a value to add to dictionary when type has no TableAttribute.
        public Table(TypeInfoWrapper type, string defaultSchema)
        {
            type.IsNotNull(nameof(type));

            this.TableName = type.Name;
            this.Schema = defaultSchema;
            this.HasTableAttribute = false;
        }

        // Constructs a value to add to dictionary.
        public Table(TableAttribute tableAttribute)
        {
            tableAttribute.IsNotNull(nameof(tableAttribute));

            this.TableName = tableAttribute.Name;
            this.Schema = tableAttribute.Schema;
            this.CatalogueName = tableAttribute.CatalogueName;
            this.HasTableAttribute = true;
        }

        public bool HasTableAttribute { get; }
        public bool HasCatalogueName => this.CatalogueName != null;

        public string CatalogueName { get; }
        public string Schema { get; }
        public string TableName { get; }

        public override int GetHashCode()
        {
            int result = (this.Schema?.GetHashCode() ?? 0) ^ this.TableName.GetHashCode();

            return result;
        }

        /// <summary>
        ///     Equals only operates on TableName and Schema
        /// </summary>
        public bool BasicFieldsEqual(object obj)
        {
            var table = obj as Table;

            bool result = table != null &&
                          (table.Schema == null && this.Schema == null ||
                           (table.Schema?.Equals(this.Schema) ?? false)) &&
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