namespace TestDataFramework.RepositoryOperations.Model
{
    public class ExtendedColumnSymbol : ColumnSymbol
    {
        public PropertyAttribute<ForeignKeyAttribute> PropertyAttribute { get; set; }

        public override string ToString()
        {
            string result = base.ToString() + ", " + this.PropertyAttribute;
            return result;
        }
    }
}
