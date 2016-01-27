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
        private readonly SqlWriterCommandTextGenerator commandTextGenerator;

        public SqlWriterDictionary(LetterEncoder encoder, IWritePrimitives writePrimitives, SqlWriterCommandTextGenerator commandTextGenerator)
        {
            this.encoder = encoder;
            this.writePrimitives = writePrimitives;
            this.commandTextGenerator = commandTextGenerator;

            this.PopulateWriterDictionary();
        }

        private void PopulateWriterDictionary()
        {
            this.writerDictionary = new Dictionary<Type, WriterDelegate>()
            {
                {typeof (int), this.WriteNumberCommand},
                {typeof (short), this.WriteNumberCommand},
                {typeof (long), this.WriteNumberCommand},
                {typeof (byte), this.WriteNumberCommand},
                {typeof (uint), this.WriteNumberCommand},
                {typeof (ushort), this.WriteNumberCommand},
                {typeof (ulong), this.WriteNumberCommand},
                {typeof (string), this.WriteStringCommand},
            };
        }

        private Dictionary<Type, WriterDelegate> writerDictionary;

        public WriterDelegate this[Type type]
        {
            get
            {
                SqlWriterDictionary.Logger.Debug("Entering Type indexer");

                WriterDelegate writer;
                if (!this.writerDictionary.TryGetValue(type, out writer))
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

            return result;
        }

        private DecoderDelegate WriteNumberCommand(PropertyInfo propertyInfo)
        {
            SqlWriterDictionary.Logger.Debug("Entering WriteNumberCommand");

            string commandText = this.commandTextGenerator.WriteNumber(propertyInfo);                

            this.writePrimitives.AddSqlCommand(commandText);

            SqlWriterDictionary.Logger.Debug("Exiting WriteNumberCommand");

            return SqlWriterDictionary.DecodeNumber;
        }

        private static ulong DecodeNumber(PropertyInfo propertyInfo, object input)
        {
            SqlWriterDictionary.Logger.Debug("Entering DecodeNumber");

            if (input is DBNull)
            {
                return 0;
            }

            if (
                !new[] { typeof(byte), typeof(int), typeof(short), typeof(long) }.Contains(input.GetType()))
            {
                throw new UnexpectedTypeException(string.Format(Messages.UnexpectedHandlerType, propertyInfo, input));
            }

            ulong result = (ulong)Convert.ChangeType(input, typeof(ulong));

            SqlWriterDictionary.Logger.Debug("Exiting DecodeNumber");

            return result;
        }

        private DecoderDelegate WriteStringCommand(PropertyInfo propertyInfo)
        {
            SqlWriterDictionary.Logger.Debug("Entering WriteStringCommand");

            string commandText = this.commandTextGenerator.WriteString(propertyInfo);                

            this.writePrimitives.AddSqlCommand(commandText);

            SqlWriterDictionary.Logger.Debug("Exiting WriteStringCommand");

            return this.DecodeString;
        }

        private ulong DecodeString(PropertyInfo propertyInfo, object input)
        {
            SqlWriterDictionary.Logger.Debug("Entering DecodeString");

            if (input is DBNull)
            {
                return 0;
            }

            if (input.GetType() != typeof(string))
            {
                throw new UnexpectedTypeException(string.Format(Messages.UnexpectedHandlerType, propertyInfo, input));
            }

            ulong result = this.encoder.Decode((string)input);

            SqlWriterDictionary.Logger.Debug("Exiting DecodeString");

            return result;
        }
    }
}
