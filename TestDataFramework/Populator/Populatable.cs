using System.Collections.Generic;

namespace TestDataFramework.Populator
{
    public abstract class Populatable
    {
        public virtual bool IsPopulated { get; protected internal set; }

        protected internal abstract void Populate();

        internal abstract void AddToReferences(IList<RecordReference> collection);
    }
}