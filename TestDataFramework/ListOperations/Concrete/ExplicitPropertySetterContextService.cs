/*
    Copyright 2016, 2017, 2018, 2019, 2023 Alexander Kuperman

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
using log4net;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Concrete;
using TestDataFramework.Helpers;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Logger;
using TestDataFramework.Populator.Concrete;

namespace TestDataFramework.ListOperations.Concrete
{
    public class ExplicitPropertySetterContextService : IValueGauranteePopulatorContextService
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(ObjectGraphService));

        private static ExplicitPropertySetterContextService instance;

        protected ExplicitPropertySetterContextService()
        {            
        }

        public static ExplicitPropertySetterContextService Instance =>
            ExplicitPropertySetterContextService.instance ?? 
                (ExplicitPropertySetterContextService.instance = new ExplicitPropertySetterContextService());

        public void SetRecordReference<T>(RecordReference<T> reference, object value)
        {
            ExplicitPropertySetterContextService.Logger.Entering(nameof(this.SetRecordReference), typeof(T));

            var setter = (ExplicitPropertySetter) value;
            ExplicitPropertySetterContextService.Logger.Debug($"Setter graph chain: {setter.PropertyChain}");

            reference.ExplicitPropertySetters.Add(setter);

            ExplicitPropertySetterContextService.Logger.Exiting(nameof(this.SetRecordReference));
        }

        public List<RecordReference<T>> FilterInWorkingListOfReferfences<T>(IEnumerable<RecordReference<T>> references,
            IEnumerable<GuaranteedValues> values)
        {
            ExplicitPropertySetterContextService.Logger.Entering(nameof(this.FilterInWorkingListOfReferfences), typeof(T));

            references = references.ToList();

            IEnumerable<ExplicitPropertySetter> explicitPropertySetters =
                values.SelectMany(v => v.Values).Cast<ExplicitPropertySetter>();

            List<RecordReference<T>> result =

                references.Where(reference =>

                    !reference.IsPopulated

                    && !reference.ExplicitPropertySetters.Any(

                        referenceSetter => explicitPropertySetters.Any(

                            valueSetter => ExplicitPropertySetterContextService.AreEqual(

                                referenceSetter.PropertyChain,

                                valueSetter.PropertyChain

                            )))).ToList();

            ExplicitPropertySetterContextService.Logger.Exiting(nameof(this.FilterInWorkingListOfReferfences), $"Result size: {result.Count}");
            return result;
        }

        private static bool AreEqual(IReadOnlyList<PropertyInfoProxy> left, IReadOnlyList<PropertyInfoProxy> right)
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
