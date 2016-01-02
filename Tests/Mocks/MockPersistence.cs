using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.Helpers;
using TestDataFramework.Persistence;
using TestDataFramework.Populator;

namespace Tests.Mocks
{
    public class MockPersistence : IPersistence
    {
        public readonly IList<IDictionary<string, object>> Storage = new List<IDictionary<string, object>>();

        public void Persist(IEnumerable<RecordReference> recordReferences)
        {
            recordReferences.Select(r => r.RecordObject).ToList().ForEach(recordObject =>
            {
                var propertyDictionary = new Dictionary<string, object>();

                this.Storage.Add(propertyDictionary);

                PropertyInfo[] propertyInfoCollection = recordObject.GetType().GetProperties(Helper.PropertyBindingFlags);

                propertyInfoCollection.ToList()
                    .ForEach(
                        propertyInfo => propertyDictionary.Add(propertyInfo.Name, propertyInfo.GetValue(recordObject)));
            });
        }
    }
}