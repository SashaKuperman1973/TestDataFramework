using System;

namespace TestDataFramework.Exceptions
{
    public class UnknownValueGeneratorTypeException : ApplicationException
    {
        public UnknownValueGeneratorTypeException(Type forType)
            : base(Messages.UnknownValueGeneratorTypeExceptionMessage + forType)
        {
        }
    }
}
