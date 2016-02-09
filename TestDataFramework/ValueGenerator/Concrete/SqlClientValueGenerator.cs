/*
    Copyright 2016 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Reflection;
using log4net;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.UniqueValueGenerator.Interfaces;
using TestDataFramework.ValueProvider.Interfaces;

namespace TestDataFramework.ValueGenerator.Concrete
{
    public class SqlClientValueGenerator : BaseValueGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlClientValueGenerator));

        public SqlClientValueGenerator(IValueProvider valueProvider, GetTypeGeneratorDelegate getTypeGenerator,
            Func<IArrayRandomizer> getArrayRandomizer, IUniqueValueGenerator uniqueValueGenerator)
            : base(valueProvider, getTypeGenerator, getArrayRandomizer, uniqueValueGenerator)
        {
        }

        protected override object GetGuid(PropertyInfo propertyInfo)
        {
            SqlClientValueGenerator.Logger.Debug("Executing GetGuid");

            return default(Guid);
        }
    }
}
