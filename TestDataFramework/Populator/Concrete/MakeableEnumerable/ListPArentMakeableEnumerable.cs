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
using TestDataFramework.Populator.Concrete.OperableList;
using TestDataFramework.Populator.Interfaces;

namespace TestDataFramework.Populator.Concrete.MakeableEnumerable
{
    public class ListParentMakeableEnumerable<TList, TRootListElement> :
        ListParentMakeableEnumerable<TList, TRootListElement, OperableListEx<TRootListElement>>
    {
        public ListParentMakeableEnumerable(IEnumerable<TList> collection,
            OperableListEx<TRootListElement> rootContainer, OperableListEx<TRootListElement> parentList) : base(
            collection, rootContainer, parentList)
        {
        }
    }

    public class ListParentMakeableEnumerable<TListElement, TParentListElement, TGrandParentList, TRootListElement> :
        ListParentMakeableEnumerable<ListParentOperableList<TListElement,
                ListParentOperableList<TParentListElement, TGrandParentList, TRootListElement>, TRootListElement>,
            TRootListElement,
            ListParentOperableList<TParentListElement, TGrandParentList, TRootListElement>>
    {
        public ListParentMakeableEnumerable(
            IEnumerable<ListParentOperableList<TListElement,
                    ListParentOperableList<TParentListElement, TGrandParentList, TRootListElement>, TRootListElement>>
                collection, OperableListEx<TRootListElement> rootContainer,
            ListParentOperableList<TParentListElement, TGrandParentList, TRootListElement> parentList) : base(
            collection, rootContainer, parentList)
        {
        }
    }

    public class ListParentMakeableEnumerable<TList, TRootListElement, TParentList> : List<TList>, IMakeableCollectionContainer<TRootListElement>
    {
        public ListParentMakeableEnumerable(IEnumerable<TList> collection,
            OperableListEx<TRootListElement> rootContainer,
            TParentList parentList) : base(collection)
        {
            this.Root = rootContainer;
            this.ParentList = parentList;
        }

        public virtual OperableListEx<TRootListElement> Root { get; }

        public virtual TParentList ParentList { get; }

        public virtual IEnumerable<TRootListElement> Make()
        {
            return this.Root.Make();
        }

        public virtual IEnumerable<TRootListElement> BindAndMake()
        {
            return this.Root.BindAndMake();
        }

        public virtual ListParentMakeableEnumerable<TList, TRootListElement, TParentList> Set<TResultElement>(
            Func<TList, TResultElement> selector)
        {
            this.Select(selector).ToList();
            return this;
        }

        public virtual ListParentMakeableEnumerable<TList, TRootListElement, TParentList> Take(int count)
        {
            IEnumerable<TList> taken = Enumerable.Take(this, count);

            var result = new ListParentMakeableEnumerable<TList, TRootListElement, TParentList>(taken, this.Root, this.ParentList);
            return result;
        }

        public virtual ListParentMakeableEnumerable<TList, TRootListElement, TParentList> Skip(int count)
        {
            IEnumerable<TList> afterSkip = Enumerable.Skip(this, count);

            var result = new ListParentMakeableEnumerable<TList, TRootListElement, TParentList>(afterSkip, this.Root, this.ParentList);
            return result;
        }
    }
}
