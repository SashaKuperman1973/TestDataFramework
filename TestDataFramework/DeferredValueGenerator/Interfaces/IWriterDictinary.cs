using System;
using TestDataFramework.DeferredValueGenerator.Concrete;

namespace TestDataFramework.DeferredValueGenerator.Interfaces
{
    public interface IWriterDictinary
    {
        WriterDelegate this[Type type] { get; }
        object[] Execute();
    }
}
