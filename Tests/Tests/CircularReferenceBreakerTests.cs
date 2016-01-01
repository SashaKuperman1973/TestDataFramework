using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.RepositoryOperations;

namespace Tests.Tests
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
