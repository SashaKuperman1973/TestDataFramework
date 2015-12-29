using System;
using TestDataFramework;
using TestDataFramework.Randomizer;

namespace Tests.TestModels
{
    public class SubjectClass
    {
        public const int StringLength = 10;
        public const int Precision = 2;
        public const long Max = 7;

        public int Integer { get; set; }

        [Max(SubjectClass.Max)]
        public int IntegerWithMax { get; set; }

        public int? NullableInteger { get; set; }

        public long LongInteger { get; set; }

        [Max(SubjectClass.Max)]
        public long LongIntegerWithMax { get; set; }

        public long? NullableLong { get; set; }

        public short ShortInteger { get; set; }

        [Max(SubjectClass.Max)]
        public short ShortIntegerWithMax { get; set; }

        public short? NullableShort { get; set; }

        public string Text { get; set; }

        [StringLength(SubjectClass.StringLength)]
        public string TextWithLength { get; set; }

        public char Character { get; set; }

        public decimal Decimal { get; set; }

        [Precision(SubjectClass.Precision)]
        public decimal DecimalWithPrecision { get; set; }

        public bool Boolean { get; set; }

        public DateTime DateTime { get; set; }

        [PastOrFuture(PastOrFuture.Future)]
        public DateTime DateTimeWithTense { get; set; }

        public byte Byte { get; set; }

        public double Double { get; set; }

        [Precision(SubjectClass.Precision)]
        public double DoubleWithPrecision { get; set; }

        [Email]
        public string AnEmailAddress { get; set; }

        [Email]
        public int NotValidForEmail { get; set; }

        public SecondClass SecondObject { get; set; }

        public int[] SimpleArray { get; set; }

        public int[,,] MultiDimensionalArray { get; set; }

        public int[][][] JaggedArray { get; set; }

        public int[,,][][,] MultiDimensionalJaggedArray { get; set; }

        public int[][,,][] JaggedMultiDimensionalArray { get; set; }
    }

    public class SecondClass
    {
        public int SecondInteger { get; set; }
    }

    public class InfiniteRecursiveClass1
    {
        public InfiniteRecursiveClass2 InfinietRecursiveClassA { get; set; }
    }

    public class InfiniteRecursiveClass2
    {
        public InfiniteRecursiveClass3 InfiniteRecursiveClassB { get; set; }
    }

    public class InfiniteRecursiveClass3
    {
        public InfiniteRecursiveClass1 InfiniteRecursiveClassC { get; set; }
    }

    public class ClassWithoutADefaultConstructor
    {
        private ClassWithoutADefaultConstructor()
        {
            
        }
    }

    public class ClassWithMaxInvalidMaxRanges
    {
        [Max(-1)]
        public int IntegerMaxLessThanZero { get; set; }

        [Max(long.MaxValue)]
        public int IntegerMaxOutOfRange { get; set; }

        [Max(-1)]
        public long LongMaxLessThanZero { get; set; }

        [Max(-1)]
        public short ShortMaxLessThanZero { get; set; }

        [Max(int.MaxValue)]
        public short ShortMaxOutOfRange { get; set; }
    }
}
