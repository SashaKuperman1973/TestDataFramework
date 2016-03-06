/*
    Copyright 2016 Alexander Kuperman

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
using System.Linq;
using System.Reflection;
using TestDataFramework.Helpers;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator;

namespace Tests.Mocks
{
    public class MockPersistence : IPersistence
    {
        public readonly IList<IDictionary<string, object>> Storage = new List<IDictionary<string, object>>();

        public void Persist(IEnumerable<RecordReference> recordReferences)
        {
            this.Storage.Clear();

            recordReferences.Select(r => r.RecordObject).ToList().ForEach(recordObject =>
            {
                var propertyDictionary = new Dictionary<string, object>();

                this.Storage.Add(propertyDictionary);

                PropertyInfo[] propertyInfoCollection = recordObject.GetType().GetPropertiesHelper();

                propertyInfoCollection.ToList()
                    .ForEach(
                        propertyInfo => propertyDictionary.Add(propertyInfo.Name, propertyInfo.GetValue(recordObject)));
            });
        }
    }
}