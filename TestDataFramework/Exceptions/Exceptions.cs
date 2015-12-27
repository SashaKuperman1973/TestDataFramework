using System;
using System.Collections.Generic;

namespace TestDataFramework.Exceptions
{
    public class NoDefaultConstructorException : ApplicationException
    {
        public NoDefaultConstructorException(Type forType)
            : base(Messages.NoDefaultConstructorExceptionMessage + forType)
        {
        }
    }

    public class TypeRecursionException : ApplicationException
    {
        public TypeRecursionException(Type currentType, IEnumerable<Type> stack) : base(TypeRecursionException.GetStackMessage(currentType, stack))
        {
        }

        private static string GetStackMessage(Type currentType, IEnumerable<Type> stack)
        {
            string message = string.Format(Messages.TypeRecursionExceptionMessage, 
                string.Join(" -> ", stack), currentType);

            return message;
        }
    }
}
