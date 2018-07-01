using System;
using System.Collections.Generic;
using System.Linq;
using TestDataFramework.Populator.Interfaces;

namespace TestDataFramework.Populator.Concrete.MakeableEnumerable
{
    public class ListParentMakeableEnumerable<TListElement, TParentListElement> : List<TListElement>, IMakeableCollectionContainer<TParentListElement>
    {
        internal readonly IMakeableCollectionContainer<TParentListElement> ParentContainer;

        public ListParentMakeableEnumerable(IEnumerable<TListElement> collection,
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

        public ListParentMakeableEnumerable<TListElement, TParentListElement> Set<TResultElement>(
            Func<TListElement, TResultElement> selector)
        {
            this.Select(selector).ToList();
            return this;
        }

        public ListParentMakeableEnumerable<TListElement, TParentListElement> Take(int count)
        {
            IEnumerable<TListElement> taken = Enumerable.Take(this, count);

            var result = new ListParentMakeableEnumerable<TListElement, TParentListElement>(taken, this.ParentContainer);
            return result;
        }

        public ListParentMakeableEnumerable<TListElement, TParentListElement> Skip(int count)
        {
            IEnumerable<TListElement> afterSkip = Enumerable.Skip(this, count);

            var result = new ListParentMakeableEnumerable<TListElement, TParentListElement>(afterSkip, this.ParentContainer);
            return result;
        }
    }
}
