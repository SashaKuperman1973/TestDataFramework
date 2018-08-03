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
    public class ListParentMakeableEnumerable<TList, TRootListElement> : List<TList>, IMakeableCollectionContainer<TRootListElement>
    {
        internal readonly IMakeableCollectionContainer<TRootListElement> RootContainer;

        public ListParentMakeableEnumerable(IEnumerable<TList> collection,
            IMakeableCollectionContainer<TRootListElement> rootContainer) : base(collection)
        {
            this.RootContainer = rootContainer;
        }

        public IEnumerable<TRootListElement> Make()
        {
            return this.RootContainer.Make();
        }

        public IEnumerable<TRootListElement> BindAndMake()
        {
            return this.RootContainer.BindAndMake();
        }

        public ListParentMakeableEnumerable<TList, TRootListElement> Set<TResultElement>(
            Func<TList, TResultElement> selector)
        {
            this.Select(selector).ToList();
            return this;
        }

        public ListParentMakeableEnumerable<TList, TRootListElement> Take(int count)
        {
            IEnumerable<TList> taken = Enumerable.Take(this, count);

            var result = new ListParentMakeableEnumerable<TList, TRootListElement>(taken, this.RootContainer);
            return result;
        }

        public ListParentMakeableEnumerable<TList, TRootListElement> Skip(int count)
        {
            IEnumerable<TList> afterSkip = Enumerable.Skip(this, count);

            var result = new ListParentMakeableEnumerable<TList, TRootListElement>(afterSkip, this.RootContainer);
            return result;
        }
    }
}
