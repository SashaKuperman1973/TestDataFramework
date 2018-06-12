using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    public class AssemblyWrapper : IWrapper<Assembly>
    {
        private readonly Guid id = Guid.NewGuid();

        public AssemblyWrapper(Assembly assembly)
        {
            this.Wrapped = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public AssemblyWrapper()
        {
        }

        public virtual IEnumerable<TypeInfoWrapper> DefinedTypes => this.Wrapped.DefinedTypes
            .Select(typeInfo => new TypeInfoWrapper(typeInfo));

        public Assembly Wrapped { get; }

        public virtual AssemblyNameWrapper[] GetReferencedAssemblies()
        {
            return this.Wrapped.GetReferencedAssemblies()
                .Select(assembly => new AssemblyNameWrapper(assembly)).ToArray();
        }

        public virtual AssemblyNameWrapper GetName()
        {
            return new AssemblyNameWrapper(this.Wrapped.GetName());
        }

        public override bool Equals(object obj)
        {
            var result = EqualityHelper<AssemblyWrapper, Assembly>.Equals(this, obj);
            return result;
        }

        public override int GetHashCode()
        {
            return this.Wrapped?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return this.Wrapped?.ToString() ?? $"Empty Assembly Wrapper. ID: {this.id}";
        }
    }
}