using System;
using System.Collections.Generic;
using TestDataFramework.Populator;

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

    public class CircularForeignKeyReferenceException : ApplicationException
    {
        public CircularForeignKeyReferenceException(RecordReference current, RecordReference headOfList)
            : base(CircularForeignKeyReferenceException.GetListMessage(current, headOfList))
        {
        }

        private static string GetListMessage(RecordReference current, RecordReference headOfList)
        {
            var typeList = new List<Type>();

            RecordReference processingCurrent = headOfList;

            do
            {
                typeList.Add(processingCurrent.RecordType);

                processingCurrent = processingCurrent.PrimaryKeyReference;

                if (headOfList == processingCurrent)
                {
                    throw new InternalErrorException(string.Format(Messages.CircularReferenceInRecordReferenceList,
                        string.Join(" -> ", typeList)));
                }
            } while (processingCurrent != null);

            string message =
                string.Format(Messages.CircularForeignKeyReferenceException, current.RecordType,
                    string.Join(" -> ", typeList));

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
        public NoForeignKeysException(Type recordType) : base(string.Format(Messages.NoForeignKeysException, recordType))
        { }
    }
}
