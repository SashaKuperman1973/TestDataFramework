using System.Collections.Generic;
using System.Linq;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;

namespace TestDataFramework.ListOperations.Concrete
{
    public class ValueSetContextService : IValueGauranteePopulatorContextService
    {
        public void SetRecordReference<T>(RecordReference<T> reference, object value)
        {
            ((RecordReference)reference).RecordObject = value;
            reference.IsPopulated = true;
        }

        public List<RecordReference<T>> FilterInWorkingListOfReferfences<T>(IEnumerable<RecordReference<T>> references,
            IEnumerable<GuaranteedValues> values)
        {
            List<RecordReference<T>> result = references.Where(reference => !reference.ExplicitPropertySetters.Any())
                .ToList();
            return result;
        }
    }
}
