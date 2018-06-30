using System;
using System.Collections.Generic;
using System.Linq;
using TestDataFramework.Populator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class MakeableEnumerable<TListElement, TParentListElement> : List<TListElement>, IMakeableCollectionContainer<TParentListElement>
    {
        private readonly IMakeableCollectionContainer<TParentListElement> parentContainer;

        public MakeableEnumerable(IEnumerable<TListElement> collection,
            IMakeableCollectionContainer<TParentListElement> parentContainer) : base(collection)
        {
            this.parentContainer = parentContainer;
        }

        public IEnumerable<TParentListElement> Make()
        {
            return this.parentContainer.Make();
        }

        public IEnumerable<TParentListElement> BindAndMake()
        {
            return this.parentContainer.BindAndMake();
        }

        public MakeableEnumerable<TListElement, TParentListElement> Set<TResultElement>(
            Func<TListElement, TResultElement> selector)
        {
            Enumerable.Select(this, selector).ToList();
            return this;
        }

        public MakeableEnumerable<TListElement, TParentListElement> Take(int count)
        {
            IEnumerable<TListElement> taken = Enumerable.Take(this, count);

            var result = new MakeableEnumerable<TListElement, TParentListElement>(taken, this.parentContainer);
            return result;
        }

        public MakeableEnumerable<TListElement, TParentListElement> Skip(int count)
        {
            IEnumerable<TListElement> afterSkip = Enumerable.Skip(this, count);

            var result = new MakeableEnumerable<TListElement, TParentListElement>(afterSkip, this.parentContainer);
            return result;
        }
    }
}
