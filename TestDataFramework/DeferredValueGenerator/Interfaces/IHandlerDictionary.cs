using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.DeferredValueGenerator.Interfaces
{
    public interface IHandlerDictionary<out T>
    {
        HandlerDelegate<T> this[Type type] { get; }
    }
}
