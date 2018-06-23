using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using TestDataFramework;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using Tests.TestModels;

namespace Tests.Tests.TableTypeCacheServiceTests
{
    [TestClass]
    public class TypeInfoWrapperTests
    {
        [TestMethod]
        public void Delegation_Test()
        {
            var subject = new ClrClass();
            TypeInfo typeInfo = subject.GetType().GetTypeInfo();

            var wrapper = new TypeInfoWrapper(subject.GetType());

            // Assertions

            Assert.AreEqual(typeInfo, wrapper.Wrapped);

            Assert.AreEqual(typeInfo.Assembly, wrapper.Assembly.Wrapped);

            Assert.AreEqual(subject.GetType(), wrapper.Type);

            Assert.AreEqual(typeInfo.MemberType, wrapper.MemberType);

            Assert.AreEqual(typeInfo.DeclaringType, wrapper.DeclaringType);

            Assert.AreEqual(typeInfo.ReflectedType, wrapper.ReflectedType);

            Assert.AreEqual(typeInfo.Name, wrapper.Name);

            object[] expectedCustromAttributes = typeInfo.GetCustomAttributes(false);
            object[] actualCustromAttributes = wrapper.GetCustomAttributes(false);
            TypeInfoWrapperTests.AssertElementsAreEqual(expectedCustromAttributes, actualCustromAttributes);

            expectedCustromAttributes = typeInfo.GetCustomAttributes(true);
            actualCustromAttributes = wrapper.GetCustomAttributes(true);
            TypeInfoWrapperTests.AssertElementsAreEqual(expectedCustromAttributes, actualCustromAttributes);

            Assert.AreEqual(typeInfo.IsDefined(typeof(TableAttribute), false),
                wrapper.IsDefined(typeof(TableAttribute), false));

            expectedCustromAttributes = typeInfo.GetCustomAttributes(typeof(TableAttribute), false);
            actualCustromAttributes = wrapper.GetCustomAttributes(typeof(TableAttribute), false);
            TypeInfoWrapperTests.AssertElementsAreEqual(expectedCustromAttributes, actualCustromAttributes);

            expectedCustromAttributes = typeInfo.GetCustomAttributes(typeof(TableAttribute), true);
            actualCustromAttributes = wrapper.GetCustomAttributes(typeof(TableAttribute), true);
            TypeInfoWrapperTests.AssertElementsAreEqual(expectedCustromAttributes, actualCustromAttributes);

            Assert.AreEqual(typeInfo.ToString(), wrapper.ToString());

            var emptyWrapper = new TypeInfoWrapper();
            Assert.AreEqual($"Empty TypeInfo Wrapper. ID: {emptyWrapper.Id}", emptyWrapper.ToString());

            Assert.AreEqual(wrapper, wrapper);
            Assert.AreNotEqual(emptyWrapper, wrapper);

            Assert.AreEqual(typeInfo.GetHashCode(), wrapper.GetHashCode());
        }

        private static void AssertElementsAreEqual(object[] left, object[] right)
        {
            Assert.AreEqual(left.Length, right.Length);
            Assert.IsTrue(left.Length > 0);

            for (int i = 0; i < left.Length; i++)
            {
                Assert.AreEqual(left[i], right[i]);
            }

        }

        [TestMethod]
        public void Null_Constructor_Argument_Throws_Test()
        {
            Helpers.ExceptionTest(() => new TypeInfoWrapper((TypeInfo)null), typeof(ArgumentNullException));
            Helpers.ExceptionTest(() => new TypeInfoWrapper((Type)null), typeof(ArgumentNullException));
        }
    }
}
