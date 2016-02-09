using System.Collections.Generic;
using TestDataFramework.Populator.Concrete;

namespace TestDataFramework.Populator.Interfaces
{
    public interface IPopulator
    {
        void Bind();

        IList<RecordReference<T>> Add<T>(int copies, RecordReference primaryRecordReference = null) where T : new();

        RecordReference<T> Add<T>(RecordReference primaryRecordReference = null) where T : new();
    }
}