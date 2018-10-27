using System.Data.Common;

namespace TestDataFramework.Populator
{
    public class DbClientConnection
    {
        public string ConnectionStringWithDefaultCatalogue { get; set; }
        public DbConnection DbConnection { get; set; }
        public DbTransaction DbTransaction { get; set; }
    }
}
