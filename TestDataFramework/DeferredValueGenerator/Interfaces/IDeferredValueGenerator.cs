/*
    Copyright 2016 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;
using System.Reflection;
using TestDataFramework.Populator;

namespace TestDataFramework.DeferredValueGenerator.Interfaces
{
    public delegate object DeferredValueGetterDelegate<in T>(T input);

    public class Data<T>
    {
        public Data(DeferredValueGetterDelegate<T> valueGetter)
        {
            this.ValueGetter = valueGetter;
        }

        public T Item { get; set; }
        public DeferredValueGetterDelegate<T> ValueGetter { get; }

        public override string ToString()
        {
            string result = $"Item: {this.Item}, ValueGetter: {this.ValueGetter}";
            return result;
        }
    }

    public interface IDeferredValueGenerator<out T>
    {
        void AddDelegate(PropertyInfo targetPropertyInfo, DeferredValueGetterDelegate<T> getValue);
        void Execute(IEnumerable<RecordReference> targets);
    }
}
