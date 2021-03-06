﻿/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;

namespace Tests.Tests
{
    [TestClass]
    public class TableTypeCriteriaTests
    {
        [TestMethod]
        public void CompleteCatalogueMatchCriteria_Test()
        {
            var assertor = new Assertor();

            const string catalogueName = "catalogueName";
            assertor.CompleteCatalogueMatchCriteria_Assert(catalogueName, catalogueName, Assert.IsTrue);

            assertor.CompleteCatalogueMatchCriteria_Assert(null, null, Assert.IsFalse);
            assertor.CompleteCatalogueMatchCriteria_Assert("A", null, Assert.IsFalse);
            assertor.CompleteCatalogueMatchCriteria_Assert(null, "B", Assert.IsFalse);
            assertor.CompleteCatalogueMatchCriteria_Assert("A", "B", Assert.IsFalse);
        }

        [TestMethod]
        public void MatchOnWhatIsDecorated_Test()
        {
            var assertor = new Assertor();

            assertor.MatchOnWhatIsDecorated_Assert(new Assertor.TableAttributeWrapper("A"), "B", Assert.IsFalse);
            assertor.MatchOnWhatIsDecorated_Assert(new Assertor.TableAttributeWrapper(null), "B", Assert.IsTrue);
            assertor.MatchOnWhatIsDecorated_Assert(new Assertor.TableAttributeWrapper(null), null, Assert.IsTrue);
            assertor.MatchOnWhatIsDecorated_Assert(null, "A", Assert.IsFalse);
            assertor.MatchOnWhatIsDecorated_Assert(null, null, Assert.IsFalse);
        }

        [TestMethod]
        public void MatchOnEverythingNotAlreadyTried_Test()
        {
            var assertor = new Assertor();

            assertor.MatchOnEverythingNotAlreadyTried_Assert("A", "B", Assert.IsFalse);
            assertor.MatchOnEverythingNotAlreadyTried_Assert("A", null, Assert.IsTrue);
            assertor.MatchOnEverythingNotAlreadyTried_Assert(null, "B", Assert.IsTrue);
            assertor.MatchOnEverythingNotAlreadyTried_Assert(null, null, Assert.IsTrue);
        }

        private class Assertor
        {
            public void CompleteCatalogueMatchCriteria_Assert(string fromSetCatalogueName, string inputCatalogueName,
                Action<bool> assert)
            {
                var fromSet = new Table(Assertor.GetTableAttribute(fromSetCatalogueName));
                var input = new Table(Assertor.GetTableAttribute(inputCatalogueName));

                assert(TableTypeCriteria.CompleteCatalogueMatchCriteria(fromSet, input));
            }

            public void MatchOnWhatIsDecorated_Assert(TableAttributeWrapper fromSetTableAttributeWrapper,
                string inputCatalogueName, Action<bool> assert)
            {
                Table fromSet = fromSetTableAttributeWrapper == null
                    ? new Table(new ForeignKeyAttribute("tableName", "keyName"), null)
                    : new Table(fromSetTableAttributeWrapper.TableAttribute);

                var input = new Table(Assertor.GetTableAttribute(inputCatalogueName));

                assert(TableTypeCriteria.MatchOnWhatIsDecorated(fromSet, input));
            }

            public void MatchOnEverythingNotAlreadyTried_Assert(string fromSetCatalogueName, string inputCatalogueName,
                Action<bool> assert)
            {
                var fromSet = new Table(Assertor.GetTableAttribute(fromSetCatalogueName));
                var input = new Table(Assertor.GetTableAttribute(inputCatalogueName));

                assert(TableTypeCriteria.MatchOnEverythingNotAlreadyTried(fromSet, input));
            }

            private static TableAttribute GetTableAttribute(string catalogueName)
            {
                return new TableAttribute(catalogueName, "schema", Guid.NewGuid().ToString());
            }

            public class TableAttributeWrapper
            {
                public TableAttributeWrapper(string catalogueName)
                {
                    this.TableAttribute = new TableAttribute(catalogueName, "schema", "name");
                }

                public TableAttribute TableAttribute { get; }
            }
        }
    }
}