using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.ValueFormatter.Concrete
{
    public class SqlClientValueFormatter : DbValueFormatter
    {
        public override string Format(object value)
        {
            var variable = value as Variable;

            if (variable != null)
            {
                return "@" + variable.Symbol;
            }

            string result = base.Format(value);

            return result ?? "null";
        }
    }
}
