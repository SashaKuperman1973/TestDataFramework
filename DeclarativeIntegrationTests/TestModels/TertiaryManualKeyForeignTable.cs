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
using TestDataFramework;

namespace DeclarativeIntegrationTests.TestModels
{
    [Table("TestDataFramework", "dbo", "TertiaryManualKeyForeignTable")]
    public class TertiaryManualKeyForeignTable
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Pk { get; set; }

        [ForeignKey("dbo", "ManualKeyForeignTable", "UserId")]
        public Guid FkManualKeyForeignTable { get; set; }

        [StringLength(20)]
        [ForeignKey("dbo", "ManualKeyForeignTable", "ForeignKey1")]
        public string FkStringForeignKey { get; set; }

        public int AnInt { get; set; }
    }
}