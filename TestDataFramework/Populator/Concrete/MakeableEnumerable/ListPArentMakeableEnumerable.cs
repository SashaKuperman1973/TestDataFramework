/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

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
