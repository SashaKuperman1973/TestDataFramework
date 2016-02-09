namespace TestDataFramework.RepositoryOperations.Model
{
    public class Column
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            string result = $"Name: {this.Name}, Value: {this.Value}";
            return result;
        }
    }
}
