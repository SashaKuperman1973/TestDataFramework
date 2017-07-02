/*
    Copyright 2016, 2017 Alexander Kuperman

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
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class DefaultInitialCountGenerator : IPropertyDataGenerator<LargeInteger>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DefaultInitialCountGenerator));

        public void FillData(IDictionary<PropertyInfo, Data<LargeInteger>> propertyDataDictionary)
        {
            DefaultInitialCountGenerator.Logger.Debug("Entering FillData");

            DefaultInitialCountGenerator.Logger.Debug($"Initial count: {Helper.DefaultInitalCount}");

            propertyDataDictionary.ToList().ForEach(kvp =>
            {
                DefaultInitialCountGenerator.Logger.Debug($"Setting for property: {kvp.Key.GetExtendedMemberInfoString()}");
                kvp.Value.Item = Helper.DefaultInitalCount;
            });

            DefaultInitialCountGenerator.Logger.Debug("Exiting FillData");
        }
    }
}
