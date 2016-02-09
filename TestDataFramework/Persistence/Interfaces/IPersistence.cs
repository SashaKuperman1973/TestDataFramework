using System.Collections.Generic;
using TestDataFramework.Populator;

namespace TestDataFramework.Persistence.Interfaces
{
    public interface IPersistence
    {
        void Persist(IEnumerable<RecordReference> recordReferences);
    }
}
