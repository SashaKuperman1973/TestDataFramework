using System.Collections.Generic;
using TestDataFramework.Populator;

namespace TestDataFramework.Persistence
{
    public interface IPersistence
    {
        void Persist(IEnumerable<RecordReference> recordReferences);
    }
}
