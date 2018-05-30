using System;
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService
{
    public class TestDataAppDomain
    {
        internal readonly AppDomain AppDomain;

        public TestDataAppDomain(AppDomain appDomain)
        {
            this.AppDomain = appDomain;
        }

        public virtual TestDataAssembly Load(AssemblyName assemblyName)
        {
            Assembly assembly = this.AppDomain.Load(assemblyName);
            var result = new TestDataAssembly(assembly);
            return result;
        }
    }
}