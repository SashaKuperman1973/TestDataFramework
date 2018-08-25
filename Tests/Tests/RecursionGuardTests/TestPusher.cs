using System;
using System.Collections.Generic;
using System.Reflection;
using TestDataFramework.DeepSetting;
using TestDataFramework.TypeGenerator.Concrete;

namespace Tests.Tests.RecursionGuardTests
{
    internal class TestPusher
    {
        private readonly List<ExplicitPropertySetter> explicitPropertySetters;
        private ObjectGraphNode objectGraphNode = null;
        private readonly List<PropertyInfo> runningObjectGraph;
        private readonly RecursionGuard recursionGuard;
        private int runningObjectGraphIndex = 0;

        public TestPusher(List<ExplicitPropertySetter> explicitPropertySetters,
            List<PropertyInfo> runningObjectGraph, RecursionGuard recursionGuard)
        {
            this.explicitPropertySetters = explicitPropertySetters;
            this.runningObjectGraph = runningObjectGraph;
            this.recursionGuard = recursionGuard;
        }

        public bool Push(Type forType)
        {
            this.objectGraphNode = this.objectGraphNode == null
                ? new ObjectGraphNode(null, null)
                : new ObjectGraphNode(this.runningObjectGraph[this.runningObjectGraphIndex++], this.objectGraphNode);

            bool result = this.recursionGuard.Push(forType, this.explicitPropertySetters, this.objectGraphNode);

            return result;
        }
    }
}
