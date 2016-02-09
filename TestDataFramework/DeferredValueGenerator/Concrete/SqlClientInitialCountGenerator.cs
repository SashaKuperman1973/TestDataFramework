using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class SqlClientInitialCountGenerator : IPropertyDataGenerator<LargeInteger>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlClientInitialCountGenerator));

        private readonly IWriterDictinary writerDictinary;

        public SqlClientInitialCountGenerator(IWriterDictinary writerDictinary)
        {
            SqlClientInitialCountGenerator.Logger.Debug("Entering constructor");

            this.writerDictinary = writerDictinary;

            SqlClientInitialCountGenerator.Logger.Debug("Exiting constructor");
        }

        public void FillData(IDictionary<PropertyInfo, Data<LargeInteger>> propertyDataDictionary)
        {
            SqlClientInitialCountGenerator.Logger.Debug("Entering FillData");

            if (!propertyDataDictionary.Any())
            {
                SqlClientInitialCountGenerator.Logger.Debug("FillData: empty input. Exiting");
                return;
            }

            List<DecoderDelegate> decoders = new List<DecoderDelegate>();

            List<KeyValuePair<PropertyInfo, Data<LargeInteger>>> propertyDataList =
                propertyDataDictionary.ToList();

            propertyDataList.ForEach(data =>
            {
                SqlClientInitialCountGenerator.Logger.Debug($"PropertyInfo keying the writerDictionary: {data.Key}");

                WriterDelegate writer = this.writerDictinary[data.Key.PropertyType];

                SqlClientInitialCountGenerator.Logger.Debug($"Writer delegate: {writer}");

                DecoderDelegate decoder = writer(data.Key);

                SqlClientInitialCountGenerator.Logger.Debug($"Decoder delegate: {decoder}");

                decoders.Add(decoder);
            });

            object[] results = this.writerDictinary.Execute();

            if (results.Length != decoders.Count)
            {
                throw new DataLengthMismatchException(Messages.DataCountsDoNotMatch);
            }

            for (int i = 0; i < results.Length; i++)
            {
                SqlClientInitialCountGenerator.Logger.Debug($"PropertyInfo to set: {propertyDataList[i].Key}, Db result: {results[i]}");

                propertyDataList[i].Value.Item = decoders[i](propertyDataList[i].Key, results[i]) + 1;

                SqlClientInitialCountGenerator.Logger.Debug("Set result : propertyDataList[i].Value.Item");
            }

            SqlClientInitialCountGenerator.Logger.Debug("Exiting FillData");
        }
    }
}
