using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Populator.Concrete.DbClientPopulator;

namespace TestDataFramework.Populator.Interfaces
{
    public interface IDbClientPopulator : IPopulator
    {
        DbClientTransaction BindInATransaction(DbClientTransactionOptions options = null);
    }
}
