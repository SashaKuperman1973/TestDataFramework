using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Concrete;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.TypeGenerator.Concrete;
using Tests.TestModels;

namespace Tests.Tests.RecursionGuardTests
{
    [TestClass]
    public class RecursionGuardTests
    {
        private RecursionGuard recursionGuard;
        private Mock<IObjectGraphService> objectGraphServiceMock;
            
        [TestInitialize]
        public void Initialize()
        {
            this.objectGraphServiceMock = new Mock<IObjectGraphService>();
            this.recursionGuard = new RecursionGuard(this.objectGraphServiceMock.Object);
        }

        [TestMethod]
        public void PushAndPop_ExplicitSetterforCircularReference_Success_Test()
        {
            // Arrange

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

            this.objectGraphServiceMock
                .Setup(m => m.DoesPropertyHaveSetter(It.IsAny<List<PropertyInfo>>(), explicitPropertySetters))
                .Returns(true).Verifiable();

            // Act

            bool result = true;
            var pusher = new TestPusher(explicitPropertySetters, objectGraph, this.recursionGuard);

            result &= pusher.Push(typeof(RecursionRootClass));
            result &= pusher.Push(typeof(InfiniteRecursiveClass1));
            result &= pusher.Push(typeof(InfiniteRecursiveClass2));
            result &= pusher.Push(typeof(InfiniteRecursiveClass1));
            result &= pusher.Push(typeof(InfiniteRecursiveClass2));

            // Assert

            Assert.IsTrue(result);

            this.recursionGuard.Pop();
            this.recursionGuard.Pop();
            this.recursionGuard.Pop();
            this.recursionGuard.Pop();
            this.recursionGuard.Pop();

            Helpers.ExceptionTest(() => this.recursionGuard.Pop(), typeof(InvalidOperationException));

            this.objectGraphServiceMock.Verify();
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

            this.objectGraphServiceMock
                .Setup(m => m.DoesPropertyHaveSetter(It.Is<List<PropertyInfo>>(piList => piList.Count < 5), explicitPropertySetters))
                .Returns(true).Verifiable();

            this.objectGraphServiceMock
                .Setup(m => m.DoesPropertyHaveSetter(It.Is<List<PropertyInfo>>(piList => piList.Count == 5 ), explicitPropertySetters))
                .Returns(false).Verifiable();


            List<PropertyInfo> runningObjectGraph =
                objectGraphService.GetObjectGraph<RecursionRootClass, InfiniteRecursiveClass1>(
                    m => m.RecursionProperty1.InfinietRecursiveObjectA.InfiniteRecursiveObjectB.InfinietRecursiveObjectA
                        .InfiniteRecursiveObjectB);

            bool result = true;
            var pusher = new TestPusher(explicitPropertySetters, runningObjectGraph, this.recursionGuard);

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

            this.objectGraphServiceMock.Verify();
        }

        [TestMethod]
        public void Push_NoExplicitSetterforCircularReference_Test()
        {
            var objectGraphService = new ObjectGraphService();

            List<PropertyInfo> runningObjectGraph =
                objectGraphService.GetObjectGraph<RecursionRootClass, InfiniteRecursiveClass1>(
                    m => m.RecursionProperty1.InfinietRecursiveObjectA.InfiniteRecursiveObjectB);

            bool result = true;
            var pusher = new TestPusher(Enumerable.Empty<ExplicitPropertySetter>().ToList(), runningObjectGraph, this.recursionGuard);

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
