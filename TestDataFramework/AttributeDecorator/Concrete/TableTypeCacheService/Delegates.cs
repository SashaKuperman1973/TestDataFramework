namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService
{
    public delegate TableAttribute GetTableAttribute(TestDataTypeInfo typeInfo);

    public delegate void TryAssociateTypeToTable(TestDataTypeInfo definedType,
        AssemblyLookupContext assemblyLookupContext, GetTableAttribute getTableAttibute,
        string defaultSchema);
}
