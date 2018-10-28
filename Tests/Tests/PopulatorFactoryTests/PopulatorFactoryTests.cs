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
    public class PopulatorFactoryTests
    {
        [TestMethod]
        public void CreateMemoryPopulator_Test()
        {
            var factory = new PopulatorFactory();
            IPopulator reference1 = factory.CreateMemoryPopulator();

            IPopulator reference2 = factory.CreateMemoryPopulator();

            Assert.IsNotNull(reference1);
            Assert.IsNotNull(reference2);

            Assert.AreEqual(reference1, reference2);

            factory.Dispose();
        }

        [TestMethod]
        public void CreateMemoryPopulator_Dispose_Test()
        {
            var factory = new PopulatorFactory();
            IPopulator reference1 = factory.CreateMemoryPopulator();
            factory.Dispose();

            IPopulator reference2 = factory.CreateMemoryPopulator();

            Assert.IsNotNull(reference1);
            Assert.IsNotNull(reference2);

            Assert.AreNotEqual(reference1, reference2);

            factory.Dispose();
        }

        [TestMethod]
        public void CreateSqlClientPopulator_Test()
        {
            var factory = new PopulatorFactory();
            IPopulator reference1 = factory.CreateSqlClientPopulator("connection string");

            IPopulator reference2 = factory.CreateSqlClientPopulator("connection string");

            Assert.IsNotNull(reference1);
            Assert.IsNotNull(reference2);

            Assert.AreEqual(reference1, reference2);

            factory.Dispose();
        }

        [TestMethod]
        public void CreateSqlClientPopulator_Dispose_Test()
        {
            var factory = new PopulatorFactory();
            IPopulator reference1 = factory.CreateSqlClientPopulator("connection string");
            factory.Dispose();

            IPopulator reference2 = factory.CreateSqlClientPopulator("connection string");

            Assert.IsNotNull(reference1);
            Assert.IsNotNull(reference2);

            Assert.AreNotEqual(reference1, reference2);

            factory.Dispose();
        }
    }
}
