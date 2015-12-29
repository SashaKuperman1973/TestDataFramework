using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.RepositoryOperations.Model
{
    public class ColumnSymbol
    {
        public Type TableType { get; set; }
        public string ColumnName { get; set; }
        public object Value { get; set; }
    }
}

