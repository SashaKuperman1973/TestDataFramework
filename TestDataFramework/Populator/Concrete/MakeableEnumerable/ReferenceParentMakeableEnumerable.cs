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
    public class ReferenceParentMakeableEnumerable<TListElement, TRoot, TParentList> : List<TListElement>, IMakeable<TRoot>
    {
        public ReferenceParentMakeableEnumerable(IEnumerable<TListElement> collection,
            RecordReference<TRoot> root, TParentList parentList) : base(collection)
        {
            this.Root = root;
            this.ParentList = parentList;
        }

        public virtual RecordReference<TRoot> Root { get; }

        public virtual TParentList ParentList { get; }

        public virtual TRoot Make()
        {
            return this.Root.Make();
        }

        public virtual TRoot BindAndMake()
        {
            return this.Root.BindAndMake();
        }

        public virtual ReferenceParentMakeableEnumerable<TListElement, TRoot, TParentList> Set<TResultElement>(
            Func<TListElement, TResultElement> selector)
        {
            this.Select(selector).ToList();
            return this;
        }

        public virtual ReferenceParentMakeableEnumerable<TListElement, TRoot, TParentList> Take(int count)
        {
            IEnumerable<TListElement> taken = Enumerable.Take(this, count);

            var result =
                new ReferenceParentMakeableEnumerable<TListElement, TRoot, TParentList>(taken, this.Root,
                    this.ParentList);

            return result;
        }

        public virtual ReferenceParentMakeableEnumerable<TListElement, TRoot, TParentList> Skip(int count)
        {
            IEnumerable<TListElement> afterSkip = Enumerable.Skip(this, count);

            var result =
                new ReferenceParentMakeableEnumerable<TListElement, TRoot, TParentList>(afterSkip, this.Root,
                    this.ParentList);

            return result;
        }
    }
}
