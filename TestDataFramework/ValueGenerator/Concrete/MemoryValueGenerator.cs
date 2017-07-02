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

using System;
using System.Reflection;
using log4net;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.UniqueValueGenerator.Interfaces;
using TestDataFramework.ValueProvider.Interfaces;

namespace TestDataFramework.ValueGenerator.Concrete
{
    public class MemoryValueGenerator : BaseValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MemoryValueGenerator));

        public MemoryValueGenerator(IValueProvider valueProvider, Func<ITypeGenerator> getTypeGenerator,
            Func<IArrayRandomizer> getArrayRandomizer, IUniqueValueGenerator uniqueValueGenerator, IAttributeDecorator attributeDecorator)
            : base(valueProvider, getTypeGenerator, getArrayRandomizer, uniqueValueGenerator, attributeDecorator)
        {
        }

        protected override object GetGuid(PropertyInfo propertyInfo)
        {
            MemoryValueGenerator.Logger.Debug("Executing GetGuid");

            return Guid.NewGuid();
        }
    }
}
