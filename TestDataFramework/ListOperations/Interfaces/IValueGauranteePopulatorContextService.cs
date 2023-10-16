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
using TestDataFramework.ListOperations.Concrete;
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
