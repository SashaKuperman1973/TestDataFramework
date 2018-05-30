using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;

namespace TestDataFramework.AttributeDecorator
{
    public class AssemblyLookupContext
    {
        public ConcurrentDictionary<Table, TestDataTypeInfo> TypeDictionary { get; set; }
        public ConcurrentDictionary<Table, List<TestDataTypeInfo>> CollisionDictionary { get; set; }
        public TypeDictionaryEqualityComparer TypeDictionaryEqualityComparer { get; set; }
    }
}
