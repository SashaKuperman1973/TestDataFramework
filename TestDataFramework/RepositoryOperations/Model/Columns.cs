using System.Collections.Generic;
using System.Linq;

namespace TestDataFramework.RepositoryOperations.Model
{
    public class Columns
    {
        public IEnumerable<Column> RegularColumns { get; set; }

        public IEnumerable<Column> ForeignKeyColumns { get; set; }

        public IEnumerable<Column> AllColumns => this.RegularColumns.Concat(this.ForeignKeyColumns);
    }
}