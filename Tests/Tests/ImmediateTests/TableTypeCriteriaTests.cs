using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class TableTypeCriteriaTests
    {
        private class Assertor
        {
            public class TableAttributeWrapper
            {
                public TableAttributeWrapper(string catalogueName)
                {
                    this.TableAttribute = new TableAttribute(catalogueName, "schema", "name");
                }

                public TableAttribute TableAttribute { get; }
            }

            public void CompleteCatalogueMatchCriteria_Assert(string fromSetCatalogueName, string inputCatalogueName, Action<bool> assert)
            {
                Table fromSet = new Table(Assertor.GetTableAttribute(fromSetCatalogueName));
                Table input = new Table(Assertor.GetTableAttribute(inputCatalogueName));

                assert(TableTypeCriteria.CompleteCatalogueMatchCriteria(fromSet, input));
            }

            public void MatchOnWhatIsDecorated_Assert(TableAttributeWrapper fromSetTableAttributeWrapper, string inputCatalogueName, Action<bool> assert)
            {
                Table fromSet = fromSetTableAttributeWrapper == null
                    ? new Table(new ForeignKeyAttribute("tableName", "keyName"), null)
                    : new Table(fromSetTableAttributeWrapper.TableAttribute);
                    
                var input = new Table(Assertor.GetTableAttribute(inputCatalogueName));

                assert(TableTypeCriteria.MatchOnWhatIsDecorated(fromSet, input));
            }

            public void MatchOnEverythingNotAlreadyTried_Assert(string fromSetCatalogueName, string inputCatalogueName, Action<bool> assert)
            {
                Table fromSet = new Table(Assertor.GetTableAttribute(fromSetCatalogueName));
                Table input = new Table(Assertor.GetTableAttribute(inputCatalogueName));

                assert(TableTypeCriteria.MatchOnEverythingNotAlreadyTried(fromSet, input));
            }

            private static TableAttribute GetTableAttribute(string catalogueName)
            {
                return new TableAttribute(catalogueName, "schema", Guid.NewGuid().ToString());
            }
        }

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
    }
}
