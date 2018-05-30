using System;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;

namespace TestDataFramework.AttributeDecorator.Interfaces
{
    public interface ITableTypeCacheService
    {
        Type GetCachedTableType(ForeignKeyAttribute foreignAttribute, TableAttribute tableAttribute,
            AssemblyLookupContext assemblyLookupContext);

        TestDataAppDomain CreateDomain();

        void UnloadDomain(TestDataAppDomain domain);
    }
}
