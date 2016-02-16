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
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Factories;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace Tests.Tests
{
    [TestClass]
    public class PopulatorFactoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();
        }

        [TestMethod]
        public void GetSqlClientPopulator_Test()
        {
            using (var factory = new PopulatorFactory())
            {
                IPopulator populator = factory.CreateSqlClientPopulator(string.Empty);
                Assert.AreEqual(typeof (StandardPopulator), populator.GetType());
            }
        }

        [TestMethod]
        public void GetSqlClientPopulator_WithTransactionOption()
        {
            using (var factory = new PopulatorFactory())
            {
                IPopulator populator = factory.CreateSqlClientPopulator(string.Empty, false);
                Assert.AreEqual(typeof(StandardPopulator), populator.GetType());
            }
        }

        [TestMethod]
        public void GetMemoryPopulator_Test()
        {
            using (var factory = new PopulatorFactory())
            {
                IPopulator populator = factory.CreateMemoryPopulator();
                Assert.AreEqual(typeof(StandardPopulator), populator.GetType());
            }
        }
    }
}
