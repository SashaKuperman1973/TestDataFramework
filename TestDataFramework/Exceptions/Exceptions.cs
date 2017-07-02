/*
    Copyright 2016, 2017 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.Helpers;

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

    public class AttributeDecoratorException : ApplicationException
    {
        public AttributeDecoratorException(string cannotResolveForeignTableMessage, ForeignKeyAttribute foreignAttribute, Type foreignType)
            : base(string.Format(cannotResolveForeignTableMessage, foreignAttribute, foreignType))
        {
        }
    }

    public class TableTypeLookupException : ApplicationException
    {
        public TableTypeLookupException(string assemblyCacheNotPopulatedMessage, Assembly assembly)
            : base(string.Format(assemblyCacheNotPopulatedMessage, assembly))
        {            
        }
    }

    public class TableTypeCacheException : ApplicationException
    {
        public TableTypeCacheException(string duplicateTableNameMessage, IEnumerable<Type> typesInvolved)
            : base(string.Format(duplicateTableNameMessage, string.Join(", ", typesInvolved)))
        {
        }

        public TableTypeCacheException(string errorGettingDefinedTypeMessage, Table table)
            : base(string.Format(errorGettingDefinedTypeMessage, table))
        {
        }

        public TableTypeCacheException(string ambigousTableSearchConditionsMessage, Table table, Type ambigousType1,
            Type ambigousType2)
            : base(string.Format(ambigousTableSearchConditionsMessage, table, ambigousType1, ambigousType2))
        {
        }
    }

    public class WritePrimitivesException : ApplicationException
    {
        public WritePrimitivesException(string catalogueAndNoSchemaMessage, string catalogueName, string tableName)
            : base(string.Format(catalogueAndNoSchemaMessage, catalogueName, tableName))
        {            
        }
    }

    public class TableAttributeException : ApplicationException
    {
        public TableAttributeException(string catalogueAndNoSchemaMessage, string catalogueName, string tableName)
            : base(string.Format(catalogueAndNoSchemaMessage, catalogueName, tableName))
        {
        }
    }

    public class InserRecordServiceException : ApplicationException
    {
        public InserRecordServiceException(string message, string foreignTypeName, string propertyName)
            : base(string.Format(message, foreignTypeName, propertyName))
        {            
        }
    }

    public class ValueGuaranteeException : ApplicationException
    {
        public ValueGuaranteeException(string message) : base(message)
        {
        }
    }
}
