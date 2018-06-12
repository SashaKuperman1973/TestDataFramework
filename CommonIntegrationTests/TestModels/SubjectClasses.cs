/*
    Copyright 2016, 2017 Alexander Kuperman

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
using System.Collections.Generic;
using TestDataFramework;
using TestDataFramework.ValueProvider.Interfaces;

namespace CommonIntegrationTests.TestModels
{
    public interface IGuider
    {
        Guid GuidKey { get; set; }
    }

    [Table("Subject")]
    public class SubjectClass : IGuider
    {
        public const int StringLength = 5;
        public const int Precision = 4;
        public const long Max = 55;

        public int Getter => throw new NotImplementedException();

        public int Setter
        {
            set => throw new NotImplementedException();
        }

        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
        public int Key { get; set; }

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
        public string AnotherEmailAddress { get; set; }

        public Guid? ANullableGuid { get; set; }

        public Guid AGuid { get; set; }

        public string GetterOnly { get; }

        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
        public Guid GuidKey { get; set; }
    }

    public class ForeignSubjectClass
    {
        [PrimaryKey]
        public int Key { get; set; }

        [ForeignKey(typeof(SubjectClass), nameof(SubjectClass.Key))]
        public int ForeignIntKey { get; set; }

        [ForeignKey(typeof(SubjectClass), "GuidKey")]
        public Guid ForeignGuidKey { get; set; }

        public int SecondInteger { get; set; }

        public override string ToString()
        {
            var result =
                $"Key \t\t\t{this.Key}\r\n" +
                $"ForeignIntKey \t{this.ForeignIntKey}\r\n" +
                $"ForeignGuidKey \t{this.ForeignGuidKey}\r\n" +
                $"SecondInteger \t{this.SecondInteger}\r\n";

            return result;
        }
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

    public class ClassWithHandledTypes
    {
        public IDictionary<KeyValuePair<int, string>, object> ADictionary { get; set; }
    }

    public struct AStruct
    {
        public string AValue { get; set; }
    }

    public class AClassWithAStructOnConstructor
    {
        public AClassWithAStructOnConstructor(AStruct aStruct)
        {
            this.AStruct = aStruct;
        }

        public AStruct AStruct { get; }
    }

    public enum AnEnum
    {
        Foo,
        Bar
    }

    public class AClassWithAnEnumOnConstructor
    {
        public AClassWithAnEnumOnConstructor(AnEnum anEnum)
        {
            this.AnEnum = anEnum;
        }

        public AnEnum AnEnum { get; }
    }
}