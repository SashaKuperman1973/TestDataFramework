/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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