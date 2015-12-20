using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework;


namespace Tests
{
    public class SubjectClass
    {
        public const int StringLength = 10;
        public const int Precision = 2;

        public int Integer { get; set; }

        public long LongInteger { get; set; }

        public short ShortInteger { get; set; }

        public string Text { get; set; }

        [StringLength(SubjectClass.StringLength)]
        public string TextWithLength { get; set; }

        public UnresolvableType UnresolvableTypeMember { get; set; }

        public char Character { get; set; }

        public decimal Decimal { get; set; }

        [Precision(SubjectClass.Precision)]
        public decimal DecimalWithPrecision { get; set; }
    }

    public class SecondClass
    {        
    }

    public class UnresolvableType
    {
    }
}
