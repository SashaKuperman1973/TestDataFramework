using System;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;

namespace TestDataFramework.AttributeDecorator.Interfaces
{
    public interface ITableTypeCacheService
    {
        TypeInfoWrapper GetCachedTableType(ForeignKeyAttribute foreignAttribute, TableAttribute tableAttribute,
            AssemblyLookupContext assemblyLookupContext);

        AppDomainWrapper CreateDomain();

        void TryAssociateTypeToTable(TypeInfoWrapper definedType,
            AssemblyLookupContext assemblyLookupContext, GetTableAttribute getTableAttibute,
            string defaultSchema);

        void PopulateAssemblyCache(AppDomainWrapper domain, AssemblyNameWrapper assemblyName,
            GetTableAttribute getTableAttibute, string defaultSchema,
            TryAssociateTypeToTable tryAssociateTypeToTable,
            AssemblyLookupContext assemblyLookupContext);
    }
}
