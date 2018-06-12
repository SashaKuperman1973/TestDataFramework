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

using TestDataFramework;
using TestDataFramework.Populator.Interfaces;

namespace PocoIntegrationTests.TestModels
{
    public class Decorator
    {
        public static void Decorate(IPopulator populator)
        {
            populator.DecorateType<ForeignToAutoPrimaryTable>()
                .AddAttributeToType(new TableAttribute("TestDataFramework", "dbo", "ForeignToAutoPrimaryTable"))
                .AddAttributeToMember(m => m.ForignKey,
                    new ForeignKeyAttribute("dbo", "TertiaryManualKeyForeignTable", "Pk"))
                .AddAttributeToMember(m => m.ForignKey,
                    new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Manual));

            populator.DecorateType<TertiaryManualKeyForeignTable>()
                .AddAttributeToType(new TableAttribute("TestDataFramework", "dbo", "TertiaryManualKeyForeignTable"))
                .AddAttributeToMember(m => m.Pk, new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Auto))
                .AddAttributeToMember(m => m.FkManualKeyForeignTable,
                    new ForeignKeyAttribute("dbo", "ManualKeyForeignTable", "UserId"))
                .AddAttributeToMember(m => m.FkStringForeignKey, new StringLengthAttribute(20))
                .AddAttributeToMember(m => m.FkStringForeignKey,
                    new ForeignKeyAttribute("dbo", "ManualKeyForeignTable", "ForeignKey1"));
        }
    }
}