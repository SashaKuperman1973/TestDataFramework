using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TestDataFramework.Exceptions;

namespace TestDataFramework.RepositoryOperations.Operations
{
    public abstract class AbstractInsertRecord : AbstractRepositoryOperation
    {
        public enum KeyTypeEnum
        {
            Auto,
            Manual,
            None,
        }

        protected KeyTypeEnum KeyType;
    }
}
