using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.Exceptions
{
    public static class Messages
    {
        public const string NoDefaultConstructorExceptionMessage = "Type has no public default constructor: ";
        public const string TypeRecursionExceptionMessage = "Circular reference detected generating complex type graph: {0} -> {1}";
        public const string MaxAttributeOutOfRange = "Max attribute value is out of range for {0} property";
        public const string MaxAttributeLessThanZero = "Max attribute value is less than zero";
    }
}
