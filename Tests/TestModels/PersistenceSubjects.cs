using TestDataFramework;

namespace Tests.TestModels
{
    public class PrimaryTable
    {
        [PrimaryKey(KeyType = PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Key { get; set; }

        public string Text { get; set; }

        public int Integer { get; set; }
    }

    public class ForeignTable
    {
        [PrimaryKey(KeyType = PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Key { get; set; }

        [ForeignKey(primaryTable: typeof (PrimaryTable), primaryKeyName: "Key")]
        public int ForeignKey { get; set; }

        public string Text { get; set; }

        public int Integer { get; set; }
    }

    public class ManualKeyPrimaryTable
    {
        [PrimaryKey]
        public string Key1 { get; set; }

        [PrimaryKey]
        public int Key2 { get; set; }
    }

    public class ManualKeyForeignTable
    {
        [ForeignKey(typeof(ManualKeyPrimaryTable), "Key1")]
        public string ForeignKey1 { get; set; }

        [ForeignKey(typeof(ManualKeyPrimaryTable), "Key2")]
        public int ForeignKey2 { get; set; }
    }
}
