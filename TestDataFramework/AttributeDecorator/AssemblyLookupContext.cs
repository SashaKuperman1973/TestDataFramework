using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;

namespace TestDataFramework.AttributeDecorator
{
    public class AssemblyLookupContext
    {
        public ConcurrentDictionary<Table, TypeInfoWrapper> TypeDictionary { get; set; }
        public ConcurrentDictionary<Table, List<TypeInfoWrapper>> CollisionDictionary { get; set; }
        public TypeDictionaryEqualityComparer TypeDictionaryEqualityComparer { get; set; }
    }
}
