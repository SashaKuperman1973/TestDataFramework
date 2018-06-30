using System;
using System.Collections.Generic;
using System.Linq;
using TestDataFramework.Populator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class MakeableEnumerable<TListElement, TParentListElement> : List<TListElement>, IMakeableCollectionContainer<TParentListElement>
    {
        internal readonly IMakeableCollectionContainer<TParentListElement> ParentContainer;

        public MakeableEnumerable(IEnumerable<TListElement> collection,
            IMakeableCollectionContainer<TParentListElement> parentContainer) : base(collection)
        {
            this.ParentContainer = parentContainer;
        }

        public IEnumerable<TParentListElement> Make()
        {
            return this.ParentContainer.Make();
        }

        public IEnumerable<TParentListElement> BindAndMake()
        {
            return this.ParentContainer.BindAndMake();
        }

        public MakeableEnumerable<TListElement, TParentListElement> Set<TResultElement>(
            Func<TListElement, TResultElement> selector)
        {
            this.Select(selector).ToList();
            return this;
        }

        public MakeableEnumerable<TListElement, TParentListElement> Take(int count)
        {
            IEnumerable<TListElement> taken = Enumerable.Take(this, count);

            var result = new MakeableEnumerable<TListElement, TParentListElement>(taken, this.ParentContainer);
            return result;
        }

        public MakeableEnumerable<TListElement, TParentListElement> Skip(int count)
        {
            IEnumerable<TListElement> afterSkip = Enumerable.Skip(this, count);

            var result = new MakeableEnumerable<TListElement, TParentListElement>(afterSkip, this.ParentContainer);
            return result;
        }
    }
}
