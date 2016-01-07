using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;

namespace TestDataFramework.Helpers
{
    public static class AssertionExtension
    {
        public static void IsNotNull(this object value, string name)
        {
            if (value == null)
            {
                throw new ContractException(string.Format(Messages.NonNullExpected, name));
            }
        }
    }
}
