using System;
using System.Reflection;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;

namespace TestDataFramework.AttributeDecorator.Interfaces
{
    public interface ITableTypeCacheService
    {
        Type GetCachedTableType(ForeignKeyAttribute foreignAttribute, TableAttribute tableAttribute,
            AssemblyLookupContext assemblyLookupContext);

        TestDataAppDomain CreateDomain();

        void UnloadDomain(TestDataAppDomain domain);

        void TryAssociateTypeToTable(TestDataTypeInfo definedType,
            AssemblyLookupContext assemblyLookupContext, GetTableAttribute getTableAttibute,
            string defaultSchema);

        void PopulateAssemblyCache(TestDataAppDomain domain, AssemblyName assemblyName,
            GetTableAttribute getTableAttibute, string defaultSchema,
            TryAssociateTypeToTable tryAssociateTypeToTable,
            AssemblyLookupContext assemblyLookupContext);
    }
}
