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

namespace Tests.TestModels.A
{
    public class NoTableAttributeCollisionClass
    {
        [PrimaryKey]
        public int Key { get; set; }
    }

    [Table("TableNameClass")]
    public class TableNameClass
    {
        [PrimaryKey]
        public int Key { get; set; }
    }
}

namespace Tests.TestModels.B
{
    public class NoTableAttributeCollisionClass
    {
        [PrimaryKey]
        public int Key { get; set; }
    }

    public class TableNameClass
    {
        [PrimaryKey]
        public int Key { get; set; }
    }
}

namespace Tests.TestModels.Collision
{
    public class TableNameClass
    {
        [PrimaryKey]
        public int Key { get; set; }
    }
}

namespace Tests.TestModels.DecoratedCollision.A
{
    [Table("DecoratedCollisionTable")]
    public class DecoratedCollisionClass
    {
        [PrimaryKey]
        public int Key { get; set; }
    }

    [Table("DecoratedCollisionTable3Way")]
    public class DecoratedCollisionClass3Way
    {
        [PrimaryKey]
        public int Key { get; set; }
    }
}

namespace Tests.TestModels.DecoratedCollision.B
{
    [Table("DecoratedCollisionTable")]
    public class DecoratedCollisionClass
    {
        [PrimaryKey]
        public int Key { get; set; }
    }

    [Table("DecoratedCollisionTable3Way")]
    public class DecoratedCollisionClass3Way
    {
        [PrimaryKey]
        public int Key { get; set; }
    }
}

namespace Tests.TestModels.DecoratedCollision.ThreeWay
{
    [Table("DecoratedCollisionTable3Way")]
    public class DecoratedCollisionClass3Way
    {
        [PrimaryKey]
        public int Key { get; set; }
    }
}

namespace Tests.TestModels.DecoratedCollisionWithDifferentClassName.A
{
    [Table("DecoratedCollisionWithDifferentClassName")]
    public class DecoratedCollisionWithDifferentClassName_A
    {
        [PrimaryKey]
        public int Key { get; set; }
    }

    [Table("DecoratedCollisionWithDifferentClassName3Way")]
    public class DecoratedCollisionWithDifferentClassName_3Way_A
    {
        [PrimaryKey]
        public int Key { get; set; }
    }
}

namespace Tests.TestModels.DecoratedCollisionWithDifferentClassName.B
{
    [Table("DecoratedCollisionWithDifferentClassName")]
    public class DecoratedCollisionWithDifferentClassName_B
    {
        [PrimaryKey]
        public int Key { get; set; }
    }

    [Table("DecoratedCollisionWithDifferentClassName3Way")]
    public class DecoratedCollisionWithDifferentClassName_3Way_B
    {
        [PrimaryKey]
        public int Key { get; set; }
    }
}

namespace Tests.TestModels.DecoratedCollisionWithDifferentClassName.ThreeWay
{
    [Table("DecoratedCollisionWithDifferentClassName3Way")]
    public class DecoratedCollisionWithDifferentClassName_3Way_ThreeWay
    {
        [PrimaryKey]
        public int Key { get; set; }
    }
}

namespace Tests.TestModels.Foreign
{
    public class ForeignClass
    {
        [ForeignKey("NoTableAttributeCollisionClass", "Key")]
        public int NoTableAttributeCollision_ForeignKey { get; set; }

        [ForeignKey("TableNameClass", "Key")]
        public int TableName_ForeignKey { get; set; }

        [ForeignKey("DecoratedCollisionTable", "Key")]
        public int DecoratedCollision_ForeignKey { get; set; }

        [ForeignKey("DecoratedCollisionTable3Way", "Key")]
        public int DecoratedCollision3Way_ForeignKey { get; set; }

        [ForeignKey("DecoratedCollisionWithDifferentClassName", "Key")]
        public int DecoratedCollisionWithDifferentClassName_ForeignKey { get; set; }

        [ForeignKey("DecoratedCollisionWithDifferentClassName3Way", "Key")]
        public int DecoratedCollisionWithDifferentClassName3Way_ForeignKey { get; set; }
    }
}

namespace Tests.TestModels.Simple
{
    public class PrimaryClass
    {
        [PrimaryKey]
        public int Key { get; set; }
    }

    public class ForeignClass
    {
        [ForeignKey("PrimaryClass", "Key")]
        public int ForeignKey { get; set; }
    }
}