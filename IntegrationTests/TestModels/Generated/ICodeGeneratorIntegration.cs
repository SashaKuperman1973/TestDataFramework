using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace IntegrationTests.TestModels.Generated
{
    public interface ICodeGeneratorIntegration
    {
        void AddTypes(IPopulator populator, IList<RecordReference<ManualKeyForeignTable>> foreignSet1,
            IList<RecordReference<ManualKeyForeignTable>> foreignSet2);

        void Dump();
    }
}
