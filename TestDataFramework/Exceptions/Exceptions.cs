using System;
using System.Collections.Generic;
using System.Reflection;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;

namespace TestDataFramework.Exceptions
{
    public class TypeRecursionException : ApplicationException
    {
        public TypeRecursionException(Type currentType, IEnumerable<Type> stack)
            : base(TypeRecursionException.GetStackMessage(currentType, stack))
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
        public NoForeignKeysException(Type recordType)
            : base(string.Format(Messages.NoForeignKeys, Helper.PrintType(recordType)))
        {
        }
    }

    public class NoReferentialIntegrityException : ApplicationException
    {
        public NoReferentialIntegrityException(Type primaryKeyType, Type foreignKeyType)
            : base(NoReferentialIntegrityException.GetMessage(primaryKeyType, foreignKeyType))
        {
        }

        private static string GetMessage(Type primaryKeyType, Type foreignKeyType)
        {
            return string.Format(Messages.NoReferentialIntegrity, Helper.PrintType(primaryKeyType),
                Helper.PrintType(foreignKeyType));
        }
    }

    public class NotInATransactionException : ApplicationException
    {
        public NotInATransactionException() : base(Messages.NotInATransaction)
        {
        }
    }

    public class ContractException : ApplicationException
    {
        public ContractException(string message) : base(message)
        {
        }
    }

    public class UnexpectedTypeException : ApplicationException
    {
        public UnexpectedTypeException(string message) : base(message)
        {
        }
    }

    public class DataLengthMismatchException : ApplicationException
    {
        public DataLengthMismatchException(string message) : base(message)
        {
        }
    }

    public class PrimaryKeyException : ApplicationException
    {
        public PrimaryKeyException(string message, Type type) : base(string.Format(message, type))
        {
        }
    }

    public class SetExpressionException : ApplicationException
    {
        public SetExpressionException(string message) : base(message)
        {
        }
    }

    public class PopulatePrimaryKeyException : ApplicationException
    {
        public PopulatePrimaryKeyException(string message, PropertyInfo propertyInfo)
            : base(string.Format(message, propertyInfo.GetExtendedMemberInfoString()))
        {
        }
    }

    public class UnHandledTypeException : ApplicationException
    {
        public UnHandledTypeException(string message, Type type) : base(string.Format(message, type))
        {            
        }
    }
}
