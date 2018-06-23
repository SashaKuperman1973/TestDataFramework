namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    internal static class EqualityHelper
    {
        public static bool Equals<TWrapper, TWrapped>(TWrapper wrapper, object obj)
            where TWrapper : class, IWrapper<TWrapped>
        {
            var toCompare = obj as TWrapper;

            if (toCompare == null)
                return false;

            if (wrapper.Wrapped == null && toCompare.Wrapped != null
                || wrapper.Wrapped != null && toCompare.Wrapped == null)
                return false;

            if (wrapper.Wrapped == null && toCompare.Wrapped == null)
                return wrapper == toCompare;

            return wrapper.Wrapped.Equals(toCompare.Wrapped);
        }
    }
}