using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.HandledTypeGenerator
{
    public interface IHandledTypeGenerator
    {
        object GetObject(Type forType);
    }
}
