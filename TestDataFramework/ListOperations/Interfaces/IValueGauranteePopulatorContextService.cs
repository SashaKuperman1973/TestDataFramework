using System.Collections.Generic;
using TestDataFramework.Populator.Concrete;

namespace TestDataFramework.ListOperations.Interfaces
{
    public interface IValueGauranteePopulatorContextService
    {
        void SetRecordReference<T>(RecordReference<T> reference, object value);

        List<RecordReference<T>> FilterInWorkingListOfReferfences<T>(IEnumerable<RecordReference<T>> references,
            IEnumerable<GuaranteedValues> values);
    }
}
