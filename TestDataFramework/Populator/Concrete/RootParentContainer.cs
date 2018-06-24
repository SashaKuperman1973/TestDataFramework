using System.Collections.Generic;

namespace TestDataFramework.Populator.Concrete
{
    public abstract class RootParentContainer<TRoot, TParent> : Populatable
    {
        public TRoot Root { get; protected set; }

        public TParent Parent { get; protected set; }
    }
}
