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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.ValueGenerator.Concrete;

namespace Tests.Tests
{
    [TestClass]
    public class MemoryValueGeneratorTests
    {
        [TestMethod]
        public void Generate_Guid_Test()
        {
            var memoryValueGenerator = new MemoryValueGenerator(null, null, null, null, null);

            object result = memoryValueGenerator.GetValue(null, typeof(Guid));
            Assert.IsTrue(result is Guid);
            var guid1 = (Guid) result;
            Assert.AreNotEqual(Guid.Empty, guid1);

            result = memoryValueGenerator.GetValue(null, typeof(Guid));
            Assert.IsTrue(result is Guid);
            var guid2 = (Guid) result;
            Assert.AreNotEqual(Guid.Empty, guid2);

            Assert.AreNotEqual(guid1, guid2);
        }
    }
}