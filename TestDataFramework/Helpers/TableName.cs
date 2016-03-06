using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.Helpers
{
    public class TableName : TableAttribute
    {
        public TableName(string catalogueName, string schema, string name) : base(catalogueName, schema, name)
        { }

        public TableName(string name) : base(name)
        { }
    }
}
