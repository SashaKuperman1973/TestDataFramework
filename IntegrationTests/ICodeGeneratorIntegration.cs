using System.Collections.Generic;
using IntegrationTests.TestModels;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace IntegrationTests
{
    public interface ICodeGeneratorIntegration
    {
        void AddTypes(IPopulator populator, IList<RecordReference<ManualKeyForeignTable>> foreignSet1,
            IList<RecordReference<ManualKeyForeignTable>> foreignSet2);

        void Dump();
    }
}
