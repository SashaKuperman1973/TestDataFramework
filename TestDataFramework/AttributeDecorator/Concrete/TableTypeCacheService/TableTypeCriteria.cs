namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService
{
    internal static class TableTypeCriteria
    {
        // Note: If HasCatlogueName then HasTableAttribute

        internal static bool CompleteMatchCriteria(Table fromSet, Table input) =>
            fromSet.HasCatalogueName && input.HasCatalogueName &&
            fromSet.CatalogueName.Equals(input.CatalogueName);

        internal static bool MatchOnWhatIsDecorated(Table fromSet, Table input) =>
            (!input.HasCatalogueName || !fromSet.HasCatalogueName) && fromSet.HasTableAttribute;

        internal static bool MatchOnEverythingNotAlreadyTried(Table fromSet, Table input) =>
            !input.HasCatalogueName || !fromSet.HasCatalogueName;
    }
}
