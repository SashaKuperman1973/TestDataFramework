using System.Collections.Generic;
using System.Reflection;

namespace TestDataFramework.DeferredValueGenerator.Interfaces
{
    public delegate object DeferredValueGetterDelegate<in T>(T input);

    public interface IDeferredValueGenerator<out T>
    {
        void AddDelegate(PropertyInfo propertyInfo, DeferredValueGetterDelegate<T> getValue);
        void Execute(IEnumerable<object> targets);
    }
}
