using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.DeepSetting;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Populator.Concrete;

namespace TestDataFramework.ListOperations.Concrete
{
    public class ExplicitPropertySetterContextService : IValueGauranteePopulatorContextService
    {
        public void SetRecordReference<T>(RecordReference<T> reference, object value)
        {
            var setter = (ExplicitPropertySetter) value;
            reference.ExplicitPropertySetters.Add(setter);
        }

        public List<RecordReference<T>> FilterInWorkingListOfReferfences<T>(IEnumerable<RecordReference<T>> references,
            IEnumerable<GuaranteedValues> values)
        {
            references = references.ToList();

            var explicitPropertySetters = values.SelectMany(v => v.Values).Cast<ExplicitPropertySetter>();

            IEnumerable<RecordReference<T>> result =

                references.Where(reference => !reference.ExplicitPropertySetters.Any(

                    referenceSetter => explicitPropertySetters.Any(

                        valueSetter => ExplicitPropertySetterContextService.AreEqual(referenceSetter.PropertyChain,

                            valueSetter.PropertyChain))));

            return result.ToList();
        }

        private static bool AreEqual(IReadOnlyList<PropertyInfo> left, IReadOnlyList<PropertyInfo> right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            for (int i = 0; i < left.Count; i++)
            {
                if (left[i].DeclaringType != right[i].DeclaringType || left[i].Name != right[i].Name)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
