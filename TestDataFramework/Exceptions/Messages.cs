using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.Exceptions
{
    public static class Messages
    {
        public const string NoDefaultConstructorExceptionMessage = "Type has no default constructor: ";
        public const string TypeRecursionExceptionMessage = "Circular reference for type {0} detected generating complex type graph: {1}";
    }
}
