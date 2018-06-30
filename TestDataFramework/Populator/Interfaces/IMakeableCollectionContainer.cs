using System.Collections.Generic;

namespace TestDataFramework.Populator.Interfaces
{
    public interface IMakeableCollectionContainer<out TListElement>
    {
        IEnumerable<TListElement> Make();

        IEnumerable<TListElement> BindAndMake();
    }
}
