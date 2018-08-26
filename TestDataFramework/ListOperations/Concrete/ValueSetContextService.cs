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
using System.Runtime.CompilerServices;
using log4net;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Logger;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;

namespace TestDataFramework.ListOperations.Concrete
{
    public class ValueSetContextService : IValueGauranteePopulatorContextService
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(ValueSetContextService));

        private static ValueSetContextService instance;

        protected ValueSetContextService()
        {
        }

        public static ValueSetContextService Instance =>
            ValueSetContextService.instance ?? (ValueSetContextService.instance = new ValueSetContextService());

        public void SetRecordReference<T>(RecordReference<T> reference, object value)
        {
            ValueSetContextService.Logger.Entering(nameof(this.SetRecordReference), typeof(T));

            ((RecordReference)reference).RecordObjectBase = value;
            reference.IsPopulated = true;

            ValueSetContextService.Logger.Exiting(nameof(this.SetRecordReference));
        }

        public List<RecordReference<T>> FilterInWorkingListOfReferfences<T>(IEnumerable<RecordReference<T>> references,
            IEnumerable<GuaranteedValues> values)
        {
            ValueSetContextService.Logger.Entering(nameof(this.SetRecordReference), typeof(T));

            List<RecordReference<T>> result = references.Where(reference =>
                !reference.ExplicitPropertySetters.Any() && !reference.IsPopulated
            ).ToList();

            ValueSetContextService.Logger.Exiting(nameof(this.SetRecordReference));
            return result;
        }
    }
}
