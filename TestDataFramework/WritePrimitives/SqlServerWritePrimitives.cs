using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.WritePrimitives
{
    public class SqlServerWritePrimitives : IWritePrimitives
    {
        public void Insert(List<Column> columns)
        {
            throw new NotImplementedException();
        }

        public string SelectIdentity()
        {
            throw new NotImplementedException();
        }
    }
}
