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
