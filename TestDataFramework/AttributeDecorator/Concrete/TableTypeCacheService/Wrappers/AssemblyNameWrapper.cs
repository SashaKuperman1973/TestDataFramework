using System;
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    public class AssemblyNameWrapper
    {
        public virtual AssemblyName Name { get; }

        public AssemblyNameWrapper(AssemblyName assemblyName)
        {
            this.Name = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
        }

        public AssemblyNameWrapper()
        {            
        }
    }
}
