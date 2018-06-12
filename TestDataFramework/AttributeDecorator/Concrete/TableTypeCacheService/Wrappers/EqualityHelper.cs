namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    internal static class EqualityHelper<TWrapper, TWrapped> where TWrapper : class, IWrapper<TWrapped>
    {
        public static bool Equals(TWrapper wrapper, object obj)
        {
            var toCompare = obj as TWrapper;

            if (toCompare == null)
                return false;

            if (wrapper.Wrapped != null && toCompare.Wrapped != null)
                return wrapper.Wrapped.Equals(toCompare.Wrapped);

            if (wrapper.Wrapped == null && toCompare.Wrapped != null
                || wrapper.Wrapped != null && toCompare.Wrapped == null)
                return false;

            if (wrapper.Wrapped == null && toCompare.Wrapped == null)
                return wrapper == toCompare;

            return wrapper.Wrapped.Equals(toCompare.Wrapped);
        }
    }
}