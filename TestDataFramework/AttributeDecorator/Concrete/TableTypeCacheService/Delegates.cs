using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService
{
    public delegate TableAttribute GetTableAttribute(TypeInfoWrapper typeInfo);

    public delegate void TryAssociateTypeToTable(TypeInfoWrapper definedType,
        AssemblyLookupContext assemblyLookupContext, GetTableAttribute getTableAttibute,
        string defaultSchema);
}
