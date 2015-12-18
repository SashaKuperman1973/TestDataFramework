using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.Persistence;

namespace Tests.Mocks
{
    public class MockPersistence : IPersistence
    {
        public readonly IList<IDictionary<string, object>> Storage = new List<IDictionary<string, object>>();

        public MockPersistence()
        {
        }

        public void Persist(IEnumerable<object> recordObjects)
        {
            recordObjects.ToList().ForEach(recordObject =>
            {
                var propertyDictionary = new Dictionary<string, object>();

                this.Storage.Add(propertyDictionary);

                PropertyInfo[] propertyInfoCollection = recordObject.GetType().GetProperties();

                propertyInfoCollection.ToList()
                    .ForEach(
                        propertyInfo => propertyDictionary.Add(propertyInfo.Name, propertyInfo.GetValue(recordObject)));
            });
        }

        #region Hard Coded Area

        #endregion Hard Coded Area
    }
}