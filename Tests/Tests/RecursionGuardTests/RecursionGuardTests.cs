using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Concrete;
using TestDataFramework.TypeGenerator.Concrete;
using Tests.TestModels;

namespace Tests.Tests.RecursionGuardTests
{
    [TestClass]
    public class RecursionGuardTests
    {
        private RecursionGuard recursionGuard;

        [TestInitialize]
        public void Initialize()
        {
            this.recursionGuard = new RecursionGuard();
        }

        [TestMethod]
        public void PushAndPop_ExplicitSetterforCircularReference_Success_Test()
        {
            var objectGraphService = new ObjectGraphService();

            List<PropertyInfo> objectGraph = objectGraphService.GetObjectGraph<RecursionRootClass, InfiniteRecursiveClass2>(
                m => m.RecursionProperty1.InfinietRecursiveObjectA.InfiniteRecursiveObjectB.InfinietRecursiveObjectA);

            var explicitPropertySetters = new List<ExplicitPropertySetter>
            {
                new ExplicitPropertySetter
                {
                    PropertyChain = objectGraph,
                }
            };

            bool result = true;
            var pusher = new Pusher(explicitPropertySetters, objectGraph, this.recursionGuard);

            result &= pusher.Push(typeof(RecursionRootClass));
            result &= pusher.Push(typeof(InfiniteRecursiveClass1));
            result &= pusher.Push(typeof(InfiniteRecursiveClass2));
            result &= pusher.Push(typeof(InfiniteRecursiveClass1));
            result &= pusher.Push(typeof(InfiniteRecursiveClass2));

            Assert.IsTrue(result);

            this.recursionGuard.Pop();
            this.recursionGuard.Pop();
            this.recursionGuard.Pop();
            this.recursionGuard.Pop();
            this.recursionGuard.Pop();

            Helpers.ExceptionTest(() => this.recursionGuard.Pop(), typeof(InvalidOperationException));
        }

        [TestMethod]
        public void Push_ExplicitSetterforCircularReference_Fail_Test()
        {
            var objectGraphService = new ObjectGraphService();

            List<PropertyInfo> setterObjectGraph = objectGraphService.GetObjectGraph<RecursionRootClass, InfiniteRecursiveClass2>(
                m => m.RecursionProperty1.InfinietRecursiveObjectA.InfiniteRecursiveObjectB.InfinietRecursiveObjectA);

            var explicitPropertySetters = new List<ExplicitPropertySetter>
            {
                new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                }
            };

            List<PropertyInfo> runningObjectGraph =
                objectGraphService.GetObjectGraph<RecursionRootClass, InfiniteRecursiveClass1>(
                    m => m.RecursionProperty1.InfinietRecursiveObjectA.InfiniteRecursiveObjectB.InfinietRecursiveObjectA
                        .InfiniteRecursiveObjectB);

            bool result = true;
            var pusher = new Pusher(explicitPropertySetters, runningObjectGraph, this.recursionGuard);

            result &= pusher.Push(typeof(RecursionRootClass));
            Assert.IsTrue(result);

            result &= pusher.Push(typeof(InfiniteRecursiveClass1));
            Assert.IsTrue(result);

            result &= pusher.Push(typeof(InfiniteRecursiveClass2));
            Assert.IsTrue(result);

            result &= pusher.Push(typeof(InfiniteRecursiveClass1));
            Assert.IsTrue(result);

            result &= pusher.Push(typeof(InfiniteRecursiveClass2));
            Assert.IsTrue(result);

            result &= pusher.Push(typeof(InfiniteRecursiveClass1));
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Push_NoExplicitSetterforCircularReference_Test()
        {
            var objectGraphService = new ObjectGraphService();

            List<PropertyInfo> runningObjectGraph =
                objectGraphService.GetObjectGraph<RecursionRootClass, InfiniteRecursiveClass1>(
                    m => m.RecursionProperty1.InfinietRecursiveObjectA.InfiniteRecursiveObjectB);

            bool result = true;
            var pusher = new Pusher(Enumerable.Empty<ExplicitPropertySetter>().ToList(), runningObjectGraph, this.recursionGuard);

            result &= pusher.Push(typeof(RecursionRootClass));
            Assert.IsTrue(result);

            result &= pusher.Push(typeof(InfiniteRecursiveClass1));
            Assert.IsTrue(result);

            result &= pusher.Push(typeof(InfiniteRecursiveClass2));
            Assert.IsTrue(result);

            result &= pusher.Push(typeof(InfiniteRecursiveClass1));
            Assert.IsFalse(result);
        }
    }
}
