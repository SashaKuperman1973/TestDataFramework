using TestDataFramework;

namespace Tests.TestModels
{
    public class PrimaryTable
    {
        public int Key { get; set; }
    }

    public class ForeignTable
    {
        public int Key { get; set; }

        [ForeignKey(primaryTable: typeof(PrimaryTable), primaryKeyName: "Key")]
        public int ForeignKey { get; set; }
    }
}
