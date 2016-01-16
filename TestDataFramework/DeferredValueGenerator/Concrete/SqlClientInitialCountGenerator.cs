using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class SqlClientInitialCountGenerator : IPropertyDataGenerator<ulong>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlClientInitialCountGenerator));

        private readonly IWriterDictinary writerDictinary;

        public SqlClientInitialCountGenerator(IWriterDictinary writerDictinary)
        {
            this.writerDictinary = writerDictinary;
        }

        public void FillData(IDictionary<PropertyInfo, Data<ulong>> propertyDataDictionary)
        {
            SqlClientInitialCountGenerator.Logger.Debug("Entering FillData");

            List<DecoderDelegate> decoders = new List<DecoderDelegate>();

            List<KeyValuePair<PropertyInfo, Data<ulong>>> propertyDataList =
                propertyDataDictionary.ToList();

            propertyDataList.ForEach(data =>
            {
                WriterDelegate writer = this.writerDictinary[data.Key.PropertyType];
                decoders.Add(writer(data.Key));
            });

            object[] results = this.writerDictinary.Execute();

            if (results.Length != decoders.Count)
            {
                throw new DataLengthMismatchException(Messages.DataCountsDoNotMatch);
            }

            for (int i = 0; i < results.Length; i++)
            {
                propertyDataList[i].Value.Item = decoders[i](propertyDataList[i].Key, results[i]) + 1;
            }

            SqlClientInitialCountGenerator.Logger.Debug("Exiting FillData");
        }
    }
}
