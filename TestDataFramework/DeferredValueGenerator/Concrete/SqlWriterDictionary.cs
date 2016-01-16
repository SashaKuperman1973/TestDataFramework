using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public delegate ulong DecoderDelegate(PropertyInfo propertyInfo, object input);

    public delegate DecoderDelegate WriterDelegate(PropertyInfo propertyInfo);

    public class SqlWriterDictionary : IWriterDictinary
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlWriterDictionary));


        private readonly LetterEncoder encoder;
        private readonly IWritePrimitives writePrimitives;

        public SqlWriterDictionary(LetterEncoder encoder, IWritePrimitives writePrimitives)
        {
            this.encoder = encoder;
            this.writePrimitives = writePrimitives;

            this.EnsureStatics();
        }

        private void EnsureStatics()
        {
            if (SqlWriterDictionary.writerDictionary == null)
            {
                SqlWriterDictionary.writerDictionary = new Dictionary<Type, WriterDelegate>()
                {
                    {typeof (int), this.WriteNumberCommand},
                    {typeof (short), this.WriteNumberCommand},
                    {typeof (long), this.WriteNumberCommand},
                    {typeof (byte), this.WriteNumberCommand},
                    {typeof (string), this.WriteStringCommand},
                };
            }
        }

        private static Dictionary<Type, WriterDelegate> writerDictionary;

        public WriterDelegate this[Type type]
        {
            get
            {
                SqlWriterDictionary.Logger.Debug("Entering Type indexer");

                WriterDelegate writer;
                if (!SqlWriterDictionary.writerDictionary.TryGetValue(type, out writer))
                {
                    throw new KeyNotFoundException(string.Format(Messages.PropertyKeyNotFound, type));
                }

                SqlWriterDictionary.Logger.Debug("Exiting Type indexer");

                return writer;
            }
        }

        public object[] Execute()
        {
            object[] result = this.writePrimitives.Execute();
            this.writePrimitives.Reset();

            return result;
        }

        private DecoderDelegate WriteNumberCommand(PropertyInfo propertyInfo)
        {
            SqlWriterDictionary.Logger.Debug("Entering WriteNumberCommand");

            string commandText =
                $"Select MAX([{Helper.GetColunName(propertyInfo)}]) From [{Helper.GetTableName(propertyInfo.DeclaringType)}]";

            this.writePrimitives.AddSqlCommand(commandText);

            SqlWriterDictionary.Logger.Debug("Exiting WriteNumberCommand");

            return SqlWriterDictionary.DecodeNumber;
        }

        private static ulong DecodeNumber(PropertyInfo propertyInfo, object input)
        {
            SqlWriterDictionary.Logger.Debug("Entering DecodeNumber");

            if (input is DBNull)
            {
                return Helper.DefaultInitalCount;
            }

            if (
                !new[] { typeof(byte), typeof(int), typeof(short), typeof(long) }.Contains(input.GetType()))
            {
                throw new UnexpectedTypeException(string.Format(Messages.UnexpectedHandlerType, propertyInfo,
                    input.GetType()));
            }

            ulong result = (ulong)Convert.ChangeType(input, typeof(ulong)) + 1;

            SqlWriterDictionary.Logger.Debug("Exiting DecodeNumber");

            return result;
        }

        private DecoderDelegate WriteStringCommand(PropertyInfo propertyInfo)
        {
            SqlWriterDictionary.Logger.Debug("Entering WriteStringCommand");

            string tableName = "[" + Helper.GetTableName(propertyInfo.DeclaringType) + "]";
            string columnName = "[" + Helper.GetColunName(propertyInfo) + "]";

            string commandText =
                $"Select Max({columnName}) from {tableName} where {columnName} not like '%[^A-Z]%' And LEN({columnName}) = (Select Max(Len({columnName})) From {tableName} where {columnName} not like '%[^A-Z]%' )";

            this.writePrimitives.AddSqlCommand(commandText);

            SqlWriterDictionary.Logger.Debug("Exiting WriteStringCommand");

            return this.DecodeString;
        }

        private ulong DecodeString(PropertyInfo propertyInfo, object input)
        {
            SqlWriterDictionary.Logger.Debug("Entering DecodeString");

            if (input is DBNull)
            {
                return Helper.DefaultInitalCount;
            }

            if (input.GetType() != typeof(string))
            {
                throw new UnexpectedTypeException(string.Format(Messages.UnexpectedHandlerType, propertyInfo,
                    input.GetType()));
            }

            ulong result = this.encoder.Decode((string)input) + 1;

            SqlWriterDictionary.Logger.Debug("Exiting DecodeString");

            return result;
        }
    }
}
