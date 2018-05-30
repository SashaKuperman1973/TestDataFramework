using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService
{
    public class TestDataAssembly
    {
        private readonly Assembly assembly;

        public TestDataAssembly(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public virtual IEnumerable<TestDataTypeInfo> DefinedTypes => this.assembly.DefinedTypes
            .Select(typeInfo => new TestDataTypeInfo(typeInfo));
    }
}