/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.WritePrimitives.Interfaces;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public delegate LargeInteger DecoderDelegate(PropertyInfo propertyInfo, object input);

    public delegate DecoderDelegate WriterDelegate(PropertyInfo propertyInfo);

    public class SqlWriterDictionary : IWriterDictinary
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(SqlWriterDictionary));
        private readonly SqlWriterCommandTextGenerator commandTextGenerator;

        private readonly LetterEncoder encoder;
        private readonly IWritePrimitives writePrimitives;

        private Dictionary<Type, WriterDelegate> writerDictionary;

        public SqlWriterDictionary(LetterEncoder encoder, IWritePrimitives writePrimitives,
            SqlWriterCommandTextGenerator commandTextGenerator)
        {
            SqlWriterDictionary.Logger.Debug("Entering constructor");

            this.encoder = encoder;
            this.writePrimitives = writePrimitives;
            this.commandTextGenerator = commandTextGenerator;

            this.PopulateWriterDictionary();

            SqlWriterDictionary.Logger.Debug("Exiting constructor");
        }

        public WriterDelegate this[Type type]
        {
            get
            {
                SqlWriterDictionary.Logger.Debug("Entering Type indexer. type: " + type);

                WriterDelegate writer;
                if (!this.writerDictionary.TryGetValue(type, out writer))
                    throw new KeyNotFoundException(string.Format(Messages.PropertyKeyNotFound, type));

                SqlWriterDictionary.Logger.Debug("Exiting Type indexer");

                return writer;
            }
        }

        public object[] Execute()
        {
            SqlWriterDictionary.Logger.Debug("Entering Execute");

            object[] result = this.writePrimitives.Execute();

            SqlWriterDictionary.Logger.Debug("Exiting Execute");
            return result;
        }

        private void PopulateWriterDictionary()
        {
            this.writerDictionary = new Dictionary<Type, WriterDelegate>
            {
                {typeof(int), this.WriteNumberCommand},
                {typeof(short), this.WriteNumberCommand},
                {typeof(long), this.WriteNumberCommand},
                {typeof(byte), this.WriteNumberCommand},
                {typeof(uint), this.WriteNumberCommand},
                {typeof(ushort), this.WriteNumberCommand},
                {typeof(ulong), this.WriteNumberCommand},
                {typeof(string), this.WriteStringCommand}
            };
        }

        private DecoderDelegate WriteNumberCommand(PropertyInfo propertyInfo)
        {
            SqlWriterDictionary.Logger.Debug("Entering WriteNumberCommand. propertyInfo: " +
                                             propertyInfo.GetExtendedMemberInfoString());

            string commandText = this.commandTextGenerator.WriteNumber(propertyInfo);

            this.writePrimitives.AddSqlCommand(commandText);

            SqlWriterDictionary.Logger.Debug("Exiting WriteNumberCommand");
            return SqlWriterDictionary.DecodeNumber;
        }

        private static LargeInteger DecodeNumber(PropertyInfo propertyInfo, object input)
        {
            SqlWriterDictionary.Logger.Debug(
                $"Entering DecodeNumber. propertyInfo: {propertyInfo.GetExtendedMemberInfoString()}, input: {input}");

            if (input is DBNull)
            {
                SqlWriterDictionary.Logger.Debug("input is DBNull");
                return 0;
            }

            if (
                !new[] {typeof(byte), typeof(int), typeof(short), typeof(long)}.Contains(input.GetType()))
                throw new UnexpectedTypeException(string.Format(Messages.UnexpectedHandlerType,
                    propertyInfo.GetExtendedMemberInfoString(), input));

            LargeInteger result = (ulong) Convert.ChangeType(input, typeof(ulong));

            SqlWriterDictionary.Logger.Debug("Exiting DecodeNumber");

            return result;
        }

        private DecoderDelegate WriteStringCommand(PropertyInfo propertyInfo)
        {
            SqlWriterDictionary.Logger.Debug("Entering WriteStringCommand. propertyInfo: " +
                                             propertyInfo.GetExtendedMemberInfoString());

            string commandText = this.commandTextGenerator.WriteString(propertyInfo);

            this.writePrimitives.AddSqlCommand(commandText);

            SqlWriterDictionary.Logger.Debug("Exiting WriteStringCommand");

            return this.DecodeString;
        }

        private LargeInteger DecodeString(PropertyInfo propertyInfo, object input)
        {
            SqlWriterDictionary.Logger.Debug(
                $"Entering DecodeString. propertyInfo: {propertyInfo.GetExtendedMemberInfoString()}, input: {input}");

            if (input is DBNull)
            {
                SqlWriterDictionary.Logger.Debug("input is DBNull");
                return 0;
            }

            if (input.GetType() != typeof(string))
                throw new UnexpectedTypeException(string.Format(Messages.UnexpectedHandlerType, propertyInfo, input));

            LargeInteger result = this.encoder.Decode(((string) input).ToUpper());

            SqlWriterDictionary.Logger.Debug("Exiting DecodeString");

            return result;
        }
    }
}