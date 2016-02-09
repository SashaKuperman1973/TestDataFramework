namespace TestDataFramework.RepositoryOperations.Model
{
    // I need a counter reference to use but Action and Func
    // delegates don't seem to work well with ref parameters.

    public class Counter
    {
        public long Value { get; set; }
    }
}
