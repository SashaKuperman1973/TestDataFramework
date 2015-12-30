using System;
using System.Collections.Generic;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;

namespace TestDataFramework.Exceptions
{
    public class NoDefaultConstructorException : ApplicationException
    {
        public NoDefaultConstructorException(Type forType)
            : base(Messages.NoDefaultConstructor + Helper.PrintType(forType))
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
            string message = string.Format(Messages.TypeRecursion, 
                string.Join(" -> ", stack), Helper.PrintType(currentType));

            return message;
        }
    }

    public class InternalErrorException : ApplicationException
    {
        public InternalErrorException(string message) : base(message)
        {            
        }
    }

    public class NoForeignKeysException : ApplicationException
    {
        public NoForeignKeysException(Type recordType) : base(string.Format(Messages.NoForeignKeys, Helper.PrintType(recordType)))
        { }
    }

    public class NoReferentialIntegrityException : ApplicationException
    {
        public NoReferentialIntegrityException(Type primaryKey, Type foreignKey)
            : base(NoReferentialIntegrityException.GetMessage(primaryKey, foreignKey))
        {            
        }

        private static string GetMessage(Type primaryKey, Type foreignKey)
        {
            return string.Format(Messages.NoReferentialIntegrity, Helper.PrintType(primaryKey), Helper.PrintType(foreignKey));
        }
    }
}
