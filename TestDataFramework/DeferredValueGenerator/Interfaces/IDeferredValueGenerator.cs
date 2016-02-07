using System.Collections.Generic;
using System.Reflection;
using TestDataFramework.Populator;

namespace TestDataFramework.DeferredValueGenerator.Interfaces
{
    public delegate object DeferredValueGetterDelegate<in T>(T input);

    public class Data<T>
    {
        public Data(DeferredValueGetterDelegate<T> valueGetter)
        {
            this.ValueGetter = valueGetter;
        }

        public T Item { get; set; }
        public DeferredValueGetterDelegate<T> ValueGetter { get; }
    }

    public interface IDeferredValueGenerator<out T>
    {
        void AddDelegate(PropertyInfo targetPropertyInfo, DeferredValueGetterDelegate<T> getValue);
        void Execute(IEnumerable<RecordReference> targets);
    }
}
