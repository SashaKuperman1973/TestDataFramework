using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.DeferredValueGenerator.Concrete;

namespace TestDataFramework.DeferredValueGenerator.Interfaces
{
    public interface IWriterDictinary
    {
        WriterDelegate this[Type type] { get; }
        object[] Execute();
    }
}
