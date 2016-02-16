/*
    Copyright 2016 Alexander Kuperman

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
using System.Reflection;
using log4net;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.TypeGenerator.Concrete
{
    public class UniqueValueTypeGenerator : StandardTypeGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UniqueValueTypeGenerator));

        public delegate IValueGenerator GetAccumulatorValueGenerator(ITypeGenerator typeGenerator);

        private readonly IValueGenerator accumulatorValueGenerator;

        public UniqueValueTypeGenerator(GetAccumulatorValueGenerator getAccumulatorValueGenerator, IValueGenerator valueGenerator,
            IHandledTypeGenerator handledTypeGenerator) : base(valueGenerator, handledTypeGenerator)
        {
            this.accumulatorValueGenerator = getAccumulatorValueGenerator(this);
        }

        protected override void SetProperty(object objectToFill, PropertyInfo targetPropertyInfo)
        {
            UniqueValueTypeGenerator.Logger.Debug("Entering SetProperty. targetPropertyInfo: " +
                                                  targetPropertyInfo.GetExtendedMemberInfoString());

            if (!targetPropertyInfo.PropertyType.IsValueLikeType())
            {
                UniqueValueTypeGenerator.Logger.Debug("Property type is value like. Calling base.");

                base.SetProperty(objectToFill, targetPropertyInfo);
                return;
            }

            object targetPropertyValue = this.accumulatorValueGenerator.GetValue(targetPropertyInfo);
            UniqueValueTypeGenerator.Logger.Debug($"targetPropertyValue: {targetPropertyValue}");
            targetPropertyInfo.SetValue(objectToFill, targetPropertyValue);

            UniqueValueTypeGenerator.Logger.Debug("Exiting SetProperty");
        }
    }
}
