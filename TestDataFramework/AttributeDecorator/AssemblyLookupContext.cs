using System.Collections.Concurrent;
using System.Collections.Generic;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;

namespace TestDataFramework.AttributeDecorator
{
    public class AssemblyLookupContext
    {
        public AssemblyLookupContext()
        {
            this.TypeDictionaryEqualityComparer = new TypeDictionaryEqualityComparer();
            this.CollisionDictionary = new ConcurrentDictionary<Table, IList<TypeInfoWrapper>>(this.TypeDictionaryEqualityComparer);
            this.TypeDictionary = new ConcurrentDictionary<Table, TypeInfoWrapper>(this.TypeDictionaryEqualityComparer);
        }

        public ConcurrentDictionary<Table, TypeInfoWrapper> TypeDictionary { get; }
        public ConcurrentDictionary<Table, IList<TypeInfoWrapper>> CollisionDictionary { get; }
        public TypeDictionaryEqualityComparer TypeDictionaryEqualityComparer { get; }
    }
}