/*
    Copyright 2016 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using TestDataFramework;
using TestDataFramework.ValueProvider.Interfaces;

namespace Tests.TestModels
{
    public class SubjectClass
    {
        public const int StringLength = 10;
        public const int Precision = 4;
        public const long Max = 7;

        public int Getter { get { throw new NotImplementedException();} }
        public int Setter { set { throw new NotImplementedException();} }

        [PrimaryKey(KeyType = PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Key { get; set; }

        public int Integer { get; set; }
        public uint UnsignedInteger { get; set; }
        public uint? UnsignedNullableInteger { get; set; }

        [Max(SubjectClass.Max)]
        public int IntegerWithMax { get; set; }

        public int? NullableInteger { get; set; }

        public long LongInteger { get; set; }
        public ulong UnsignedLongInteger { get; set; }
        public ulong? UnsignedNullableLong { get; set; }

        [Max(SubjectClass.Max)]
        public long LongIntegerWithMax { get; set; }

        public long? NullableLong { get; set; }

        public short ShortInteger { get; set; }
        public ushort UnsignedShortInteger { get; set; }
        public ushort? UnsignedNullableShort { get; set; }

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

        public float Float { get; set; }

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
        public InfiniteRecursiveClass1 InfiniteRecursiveClassB { get; set; }
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

    public class ClassWithStringAutoPrimaryKey
    {
        public const int StringLength = 10;

        [StringLength(ClassWithStringAutoPrimaryKey.StringLength)]
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public string Key { get; set; }
    }

    public class ClassWithIntUnsupportedPrimaryKey
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.None)]
        public int Key { get; set; }
    }

    public class ClassWithIntManualPrimaryKey
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
        public int Key { get; set; }
    }

    public class ClassWithIntAutoPrimaryKey
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Key { get; set; }
    }

    public class ClassWithShortAutoPrimaryKey
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public short Key { get; set; }
    }

    public class ClassWithLongAutoPrimaryKey
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public long Key { get; set; }
    }

    public class ClassWithByteAutoPrimaryKey
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public byte Key { get; set; }
    }

    public class ClassWithGuidKeys
    {
        [PrimaryKey]
        public Guid Key1 { get; set; }

        [PrimaryKey]
        public int Key2 { get; set; }

        [PrimaryKey]
        public Guid? Key3 { get; set; }

        public Guid? Key4 { get; set; }
    }

    [Table(Name = "DbTable")]
    public class ClrClass
    {
        
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class MultiAllowedAttribute : Attribute
    {
        public int I { get; set; }
    }

    [MultiAllowed(I = 1)]
    [MultiAllowed(I = 2)]
    public class AttributeReadWriteTestClass
    {
        public int Key1 { get; set; }
        public string Key2 { get; set; }

        [StringLength(20)]
        public string Text { get; set; }

        [MultiAllowed(I = 1)]
        [MultiAllowed(I = 2)]
        public string MultiAllowedProperty { get; set; }

        [StringLength(20)]
        [PrimaryKey]
        public string MultiAtributeProperty { get; set; }
    }
}
