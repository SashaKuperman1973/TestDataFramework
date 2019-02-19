/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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

using System.Collections.Generic;
using log4net;
using TestDataFramework.Logger;

namespace TestDataFramework.Populator
{
    public abstract class Populatable
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(Populatable));

        internal virtual bool IsPopulated { get; set; }

        protected void PopulateChildren()
        {
            Populatable.Logger.Entering(nameof(this.PopulateChildren), $"Count: {this.children.Count}");
            this.children.ForEach(c =>
            {
                c.Populate();
            });
            Populatable.Logger.Exiting(nameof(this.PopulateChildren), $"Count: {this.children.Count}");
        }

        internal abstract void Populate();

        internal abstract void AddToReferences(IList<RecordReference> collection);

        private readonly List<Populatable> children = new List<Populatable>();

        protected internal void AddChild(Populatable populatable)
        {
            Populatable.Logger.Entering(nameof(this.AddChild));
            this.children.Add(populatable);
            Populatable.Logger.Exiting(nameof(this.AddChild));
        }
    }
}