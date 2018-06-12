using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    public class AssemblyWrapper
    {
        internal readonly Assembly Assembly;

        public AssemblyWrapper(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            this.Assembly = assembly;
        }

        public AssemblyWrapper()
        {            
        }

        public virtual IEnumerable<TypeInfoWrapper> DefinedTypes => this.Assembly.DefinedTypes
            .Select(typeInfo => new TypeInfoWrapper(typeInfo));

        public virtual AssemblyNameWrapper[] GetReferencedAssemblies() => this.Assembly.GetReferencedAssemblies()
            .Select(assembly => new AssemblyNameWrapper(assembly)).ToArray();

        public virtual AssemblyNameWrapper GetName() => new AssemblyNameWrapper(this.Assembly.GetName());

        public override bool Equals(object obj)
        {
            if (this.Assembly == null || obj == null)
            {
                return false;
            }

            var toCompare = obj as AssemblyWrapper;

            return toCompare != null && this.Assembly.Equals(toCompare.Assembly);
        }

        public override int GetHashCode()
        {
            return this.Assembly? .GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return this.Assembly?.ToString() ?? "Empty Assembly Wrapper";
        }
    }
}