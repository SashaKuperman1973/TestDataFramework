using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.RepositoryOperations.Model
{
    public class ExtendedColumnSymbol : ColumnSymbol
    {
        public PropertyAttribute<ForeignKeyAttribute> PropertyAttribute { get; set; }

        public override string ToString()
        {
            string result = base.ToString() + ", " + this.PropertyAttribute;
            return result;
        }
    }
}
