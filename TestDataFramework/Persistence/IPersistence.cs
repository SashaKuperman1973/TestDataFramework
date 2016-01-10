using System.Collections.Generic;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Populator;

namespace TestDataFramework.Persistence
{
    public interface IPersistence
    {
        void Persist(IEnumerable<RecordReference> recordReferences);
    }
}
