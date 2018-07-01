using System;
using System.Collections.Generic;
using System.Linq;
using TestDataFramework.Populator.Interfaces;

namespace TestDataFramework.Populator.Concrete.MakeableEnumerable
{
    public class ReferenceParentMakeableEnumerable<TListElement, TParent> : List<TListElement>, IMakeable<TParent>
    {
        internal readonly IMakeable<TParent> Parent;

        public ReferenceParentMakeableEnumerable(IEnumerable<TListElement> collection,
            IMakeable<TParent> parent) : base(collection)
        {
            this.Parent = parent;
        }

        public TParent Make()
        {
            return this.Parent.Make();
        }

        public TParent BindAndMake()
        {
            return this.Parent.BindAndMake();
        }

        public ReferenceParentMakeableEnumerable<TListElement, TParent> Set<TResultElement>(
            Func<TListElement, TResultElement> selector)
        {
            this.Select(selector).ToList();
            return this;
        }

        public ReferenceParentMakeableEnumerable<TListElement, TParent> Take(int count)
        {
            IEnumerable<TListElement> taken = Enumerable.Take(this, count);

            var result = new ReferenceParentMakeableEnumerable<TListElement, TParent>(taken, this.Parent);
            return result;
        }

        public ReferenceParentMakeableEnumerable<TListElement, TParent> Skip(int count)
        {
            IEnumerable<TListElement> afterSkip = Enumerable.Skip(this, count);

            var result = new ReferenceParentMakeableEnumerable<TListElement, TParent>(afterSkip, this.Parent);
            return result;
        }
    }
}
