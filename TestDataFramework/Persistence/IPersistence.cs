using System.Collections.Generic;

namespace TestDataFramework.Persistence
{
    public interface IPersistence
    {
        void Persist(IEnumerable<object> recordObjects);
    }
}
