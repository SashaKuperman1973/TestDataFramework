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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Interfaces;

namespace Tests.Tests.PopulatorFactoryTests
{
    [TestClass]
    public class StaticPopulatorFactoryTests
    {
        [TestMethod]
        public void CreateSqlClientPopulator_Test()
        {
            IPopulator populator = StaticPopulatorFactory.CreateSqlClientPopulator("dummyConnectionString");

            Assert.IsNotNull(populator);
        }

        [TestMethod]
        public void CreateMemoryPopulator_Test()
        {
            IPopulator populator = StaticPopulatorFactory.CreateMemoryPopulator();

            Assert.IsNotNull(populator);
        }
    }
}