namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    public interface IWrapper<out T>
    {
        T Wrapped { get; }
    }
}