using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.ValueFormatter
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

            string result = this.Format(value);

            return result ?? "null";
        }
    }
}
