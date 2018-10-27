using System.Data;

namespace TestDataFramework.Populator.Concrete.DbClientPopulator
{
    public class DbClientTransactionOptions
    {
        public virtual IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;
    }
}
