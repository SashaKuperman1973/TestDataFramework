using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.Persistence
{
    public interface IPersistence
    {
        void Persist(IEnumerable<object> recordObjects);
    }
}
