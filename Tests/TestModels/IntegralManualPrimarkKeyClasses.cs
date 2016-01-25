using TestDataFramework;

namespace Tests.TestModels
{
    public class ByteKeyClass
    {
        [PrimaryKey]
        public byte Key { get; set; }
    }

    public class IntKeyClass
    {
        [PrimaryKey]
        public int Key { get; set; }
    }

    public class ShortKeyClass
    {
        [PrimaryKey]
        public short Key { get; set; }
    }

    public class LongKeyClass
    {
        [PrimaryKey]
        public long Key { get; set; }
    }

    public class UIntKeyClass
    {
        [PrimaryKey]
        public uint Key { get; set; }
    }

    public class UShortKeyClass
    {
        [PrimaryKey]
        public ushort Key { get; set; }
    }

    public class ULongKeyClass
    {
        [PrimaryKey]
        public ulong Key { get; set; }
    }

    public class StringKeyClass
    {
        [PrimaryKey]
        public string Key { get; set; }
    }

    public class AutoKeyClass
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Key { get; set; }
    }

    public class NoneKeyClass
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.None)]
        public int Key { get; set; }
    }

    public class UnhandledKeyClass
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
        public decimal Key { get; set; }
    }
}

