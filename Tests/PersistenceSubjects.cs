using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework;

namespace Tests
{
    public class PrimaryTable
    {
        public int Key { get; set; }
    }

    public class ForeignTable
    {
        public int Key { get; set; }

        [ForeignKey(primaryTable: typeof(PrimaryTable), primaryKey: "Key")]
        public int ForeignKey { get; set; }
    }
}
