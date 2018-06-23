using System;
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    public class AppDomainWrapper
    {
        internal readonly AppDomain AppDomain;

        public AppDomainWrapper(AppDomain appDomain)
        {
            this.AppDomain = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
        }

        public AppDomainWrapper()
        {
        }

        public virtual AssemblyWrapper LoadAssembly(AssemblyNameWrapper assemblyName)
        {
            Assembly assembly = this.AppDomain?.Load(assemblyName.Name);
            AssemblyWrapper result = assembly == null ? new AssemblyWrapper() : new AssemblyWrapper(assembly);
            return result;
        }

        public virtual void Unload()
        {
            AppDomain.Unload(this.AppDomain);
        }
    }
}