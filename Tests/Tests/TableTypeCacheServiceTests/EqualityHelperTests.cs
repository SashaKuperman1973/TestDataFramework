using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using Tests.TestModels;

namespace Tests.Tests.TableTypeCacheServiceTests
{
    [TestClass]
    public class EqualityHelperTests
    {
        [TestMethod]
        public void Wrapper_Null_Returns_False_Test()
        {
            var left = new Wrapper();

            bool result = EqualityHelper.Equals<string>(left, null);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Are_Equal_Test()
        {
            var left = new Wrapper {Wrapped = "aString"};
            var right = new Wrapper {Wrapped = "aString"};

            bool result = EqualityHelper.Equals<string>(left, right);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Are_Unequal_Test()
        {
            var left = new Wrapper { Wrapped = "X" };
            var right = new Wrapper { Wrapped = "Y" };

            bool result = EqualityHelper.Equals<string>(left, right);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Left_Wrapped_Is_Null_Test()
        {
            var left = new Wrapper {Wrapped = null};
            var right = new Wrapper {Wrapped = "aString"};

            bool result = EqualityHelper.Equals<string>(left, right);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Right_Wrapped_Is_Null_Test()
        {
            var left = new Wrapper {Wrapped = "aString"};
            var right = new Wrapper {Wrapped = null};

            bool result = EqualityHelper.Equals<string>(left, right);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Left_And_Right_Wrapped_Are_Null_Returns_Wrapper_Reference_Equality_Succeeds_Test()
        {
            var wrapper = new Wrapper();

            bool result = EqualityHelper.Equals<string>(wrapper, wrapper);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Left_And_Right_Wrapped_Are_Null_Returns_Wrapper_Reference_Equality_Fails_Test()
        {
            var left = new Wrapper();
            var right = new Wrapper();

            bool result = EqualityHelper.Equals<string>(left, right);

            Assert.IsFalse(result);
        }

        private class Wrapper : IWrapper<string>
        {
            public string Wrapped { get; set; }
        }
    }
}