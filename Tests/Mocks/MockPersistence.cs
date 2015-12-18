using System;
using TestDataFramework.Persistence;

namespace Tests.Mocks
{
    public class MockPersistence : IPersistence
    {
        public MockPersistence()
        {
        }

        public void Persist(object[] recordObjects)
        {
        }

        #region Hard Coded Area

        public int Persisted => 5;

        #endregion Hard Coded Area
    }
}