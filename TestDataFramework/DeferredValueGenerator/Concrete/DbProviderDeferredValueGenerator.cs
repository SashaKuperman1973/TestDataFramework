using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class DbProviderDeferredValueGenerator<T> : IPropertyDataGenerator<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DbProviderDeferredValueGenerator<T>));

        private readonly string connectionString;
        private readonly DbProviderFactory dbProviderFactory;
        private readonly IHandlerDictionary<T> handlerDictionary;

        public DbProviderDeferredValueGenerator(IHandlerDictionary<T> handlerDictionary, DbProviderFactory dbProviderFactory, string connectionString)
        {
            this.handlerDictionary = handlerDictionary;
            this.dbProviderFactory = dbProviderFactory;
            this.connectionString = connectionString;
        }

        public void FillData(IDictionary<PropertyInfo, StandardDeferredValueGenerator<T>.Data> propertyDataDictionary)
        {
            DbProviderDeferredValueGenerator<T>.Logger.Debug("Entering FillData");

            using (DbConnection connection = this.dbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = this.connectionString;
                connection.Open();

                foreach (KeyValuePair<PropertyInfo, StandardDeferredValueGenerator<T>.Data> data in propertyDataDictionary)
                {
                    HandlerDelegate<T> handler = this.handlerDictionary[data.Key.PropertyType];

                    using (DbCommand command = this.dbProviderFactory.CreateCommand())
                    {
                        command.Connection = connection;

                        data.Value.Item = handler(data.Key, command);
                    }
                }
            }

            DbProviderDeferredValueGenerator<T>.Logger.Debug("Exiting FillData");
        }
    }
}
