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

using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.RepositoryOperations;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class CircularReferenceBreakerTests
    {
        private void TestMethod(CircularReferenceBreaker breaker, object arg1, object arg2, object arg3)
        {
        }

        [TestMethod]
        public void CircularReferenceBreaker_Test()
        {
            XmlConfigurator.Configure();

            var breaker = new CircularReferenceBreaker();

            Assert.IsFalse(breaker.IsVisited<object, object, object>(this.TestMethod));

            breaker.Push<object, object, object>(this.TestMethod);

            Assert.IsTrue(breaker.IsVisited<object, object, object>(this.TestMethod));

            breaker.Push<object, object, object>(this.TestMethod);

            Assert.IsTrue(breaker.IsVisited<object, object, object>(this.TestMethod));

            breaker.Pop();

            Assert.IsTrue(breaker.IsVisited<object, object, object>(this.TestMethod));

            breaker.Pop();

            Assert.IsFalse(breaker.IsVisited<object, object, object>(this.TestMethod));
        }
    }
}