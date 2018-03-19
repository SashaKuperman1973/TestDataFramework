using System.Collections.Generic;

namespace TestDataFramework.Populator
{
    public abstract class Populatable
    {
        protected internal abstract void Populate();

        protected internal abstract void AddToReferences(IList<RecordReference> collection);
    }
}
