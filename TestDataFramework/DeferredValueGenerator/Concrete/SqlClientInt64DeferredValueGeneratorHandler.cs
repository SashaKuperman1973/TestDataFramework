using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class SqlClientInt64DeferredValueGeneratorHandler : IDeferredValueGeneratorHandler<ulong>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlClientInt64DeferredValueGeneratorHandler));

        public ulong NumberHandler(PropertyInfo propertyInfo, DbCommand command)
        {
            SqlClientInt64DeferredValueGeneratorHandler.Logger.Debug("Entering NumberHandler");

            string commandText = $"Select MAX([{Helper.GetColunName(propertyInfo)}]) From [{Helper.GetTableName(propertyInfo.DeclaringType)}]";
            ulong result = 1;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    SqlClientInt64DeferredValueGeneratorHandler.Logger.Debug("Row found");

                    object value = reader.GetValue(0);

                    if (!new[] {typeof(byte), typeof(int), typeof (short), typeof (long)}.Contains(value.GetType()))
                    {
                        throw new UnexpectedTypeException(string.Format(Messages.UnexpectedNumberHandlerType, propertyInfo.PropertyType, value.GetType()));
                    }

                    result = (ulong)Convert.ChangeType(value, typeof(ulong));
                }
            }

            SqlClientInt64DeferredValueGeneratorHandler.Logger.Debug("Exiting NumberHandler");
            return result;
        }

        public ulong StringHandler(PropertyInfo propertyInfo, DbCommand command)
        {
            SqlClientInt64DeferredValueGeneratorHandler.Logger.Debug("Entering StringHandler");

            string commandText = $"Select MAX([{Helper.GetColunName(propertyInfo)}]) maxVarchar From [{Helper.GetTableName(propertyInfo.DeclaringType)}] Where maxVarchar like '[A-Z]%'";
            object value = null;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    SqlClientInt64DeferredValueGeneratorHandler.Logger.Debug("Row found");

                    value = reader.GetValue(0);

                    if (value.GetType() != typeof(string))
                    {
                        throw new UnexpectedTypeException(string.Format(Messages.UnexpectedNumberHandlerType, propertyInfo.PropertyType, value.GetType()));
                    }
                }
            }

            ulong result = value != null ? SqlClientInt64DeferredValueGeneratorHandler.GetLongIntFromLetters((string)value) : 0;

            SqlClientInt64DeferredValueGeneratorHandler.Logger.Debug("Exiting StringHandler");
            return result;
        }

        private static ulong GetLongIntFromLetters(string value)
        {
            ulong result = 0;

            for (int i=0; i < value.Length; i++)
            {
                var ascii = (ulong) value[i];
                result += ascii - 65;
            }

            return result;
        }
    }
}
