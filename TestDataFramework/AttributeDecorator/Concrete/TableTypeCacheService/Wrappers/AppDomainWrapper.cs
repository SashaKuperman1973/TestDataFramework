using System;
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    public class AppDomainWrapper
    {
        private readonly AppDomain appDomain;

        public AppDomainWrapper(AppDomain appDomain)
        {
            this.appDomain = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
        }

        public AppDomainWrapper()
        {
        }

        public virtual AssemblyWrapper Load(AssemblyNameWrapper assemblyName)
        {
            Assembly assembly = this.appDomain?.Load(assemblyName.Name);
            AssemblyWrapper result = assembly == null ? new AssemblyWrapper() : new AssemblyWrapper(assembly);
            return result;
        }

        public virtual void Unload()
        {
            AppDomain.Unload(this.appDomain);
        }
    }
}