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

using System.Collections.Generic;
using CommonIntegrationTests;
using CommonIntegrationTests.TestModels;
using DeclarativeIntegrationTests.TestModels;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace DeclarativeIntegrationTests.Tests
{
    public class DeclarativeGeneratorIntegrationTest : ICodeGeneratorIntegration
    {
        private IList<RecordReference<TertiaryManualKeyForeignTable>> tertiaryForeignSet;
        private IList<RecordReference<ForeignToAutoPrimaryTable>> foreignToAutoSet;

        public void AddTypes(IPopulator populator, IList<RecordReference<ManualKeyForeignTable>> foreignSet1, IList<RecordReference<ManualKeyForeignTable>> foreignSet2)
        {
            this.tertiaryForeignSet = populator.Add<TertiaryManualKeyForeignTable>(4);

            this.tertiaryForeignSet[0].AddPrimaryRecordReference(foreignSet1[0]);
            this.tertiaryForeignSet[1].AddPrimaryRecordReference(foreignSet1[1]);
            this.tertiaryForeignSet[2].AddPrimaryRecordReference(foreignSet2[0]);
            this.tertiaryForeignSet[3].AddPrimaryRecordReference(foreignSet2[1]);

            this.foreignToAutoSet = populator.Add<ForeignToAutoPrimaryTable>(2);

            this.foreignToAutoSet[0].AddPrimaryRecordReference(this.tertiaryForeignSet[0]);
            this.foreignToAutoSet[1].AddPrimaryRecordReference(this.tertiaryForeignSet[1]);
        }

        public void Dump()
        {
            Helpers.Dump(this.tertiaryForeignSet);
            Helpers.Dump(this.foreignToAutoSet);
        }
    }
}
