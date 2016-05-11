using System.Collections.Generic;
using CommonIntegrationTests.TestModels;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace CommonIntegrationTests
{
    public interface ICodeGeneratorIntegration
    {
        void AddTypes(IPopulator populator, IList<RecordReference<ManualKeyForeignTable>> foreignSet1,
            IList<RecordReference<ManualKeyForeignTable>> foreignSet2);

        void Dump();
    }
}
