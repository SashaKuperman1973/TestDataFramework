using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Populator;

namespace TestDataFramework.Persistence
{
    public class MemoryPersistence : IPersistence
    {
        private readonly IDeferredValueGenerator<ulong> deferredValueGenerator;

        public MemoryPersistence(IDeferredValueGenerator<ulong> deferredValueGenerator)
        {
            this.deferredValueGenerator = deferredValueGenerator;
        }

        public void Persist(IEnumerable<RecordReference> recordReferences)
        {
            this.deferredValueGenerator.Execute(recordReferences.Select(r => r.RecordObject));
        }
    }
}
