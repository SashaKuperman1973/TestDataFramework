/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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
