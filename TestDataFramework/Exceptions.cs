using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework
{
    public class UnknownValueGeneratorTypeException : ApplicationException
    {
        public UnknownValueGeneratorTypeException(Type forType)
            : base("Cannot resolve a value generator for type: " + forType)
        {
        }
    }
}
