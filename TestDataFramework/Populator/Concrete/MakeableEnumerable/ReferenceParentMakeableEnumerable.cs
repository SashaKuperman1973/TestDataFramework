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
    public class ReferenceParentMakeableEnumerable<TListElement, TRoot> : List<TListElement>, IMakeable<TRoot>
    {
        private readonly IMakeable<TRoot> parent;

        public ReferenceParentMakeableEnumerable(IEnumerable<TListElement> collection,
            IMakeable<TRoot> parent) : base(collection)
        {
            this.parent = parent;
        }

        public virtual TRoot Make()
        {
            return this.parent.Make();
        }

        public virtual TRoot BindAndMake()
        {
            return this.parent.BindAndMake();
        }

        public virtual ReferenceParentMakeableEnumerable<TListElement, TRoot> Set<TResultElement>(
            Func<TListElement, TResultElement> selector)
        {
            this.Select(selector).ToList();
            return this;
        }

        public virtual ReferenceParentMakeableEnumerable<TListElement, TRoot> Take(int count)
        {
            IEnumerable<TListElement> taken = Enumerable.Take(this, count);

            var result = new ReferenceParentMakeableEnumerable<TListElement, TRoot>(taken, this.parent);
            return result;
        }

        public virtual ReferenceParentMakeableEnumerable<TListElement, TRoot> Skip(int count)
        {
            IEnumerable<TListElement> afterSkip = Enumerable.Skip(this, count);

            var result = new ReferenceParentMakeableEnumerable<TListElement, TRoot>(afterSkip, this.parent);
            return result;
        }
    }
}
