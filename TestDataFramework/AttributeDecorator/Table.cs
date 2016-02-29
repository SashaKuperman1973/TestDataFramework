using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.HasTableAttribute = HasTableAttributeEnum.True;
        }

        public HasTableAttributeEnum HasTableAttribute { get; }
        public string Schema { get; } = "dbo";
        public string TableName { get; }

        public override int GetHashCode()
        {
            int result = (this.Schema?.GetHashCode() ?? 0) ^ this.TableName.GetHashCode();

            return result;
        }

        public override bool Equals(object obj)
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
            string result = $"Schema: {this.Schema}, TableName: {this.TableName}, HasTableAttribute: {this.HasTableAttribute}";
            return result;
        }
    }
}
