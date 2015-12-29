using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.RepositoryOperations.KindsOfInformation
{
    public interface IOrderInformation
    {
        long GetOrder(CircularReferenceBreaker breaker);
        bool Done { get; }
    }
}
