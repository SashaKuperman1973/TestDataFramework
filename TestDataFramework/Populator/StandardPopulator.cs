using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.Persistence;
using TestDataFramework.Randomizer;
using TestDataFramework.ValueGenerator;
using log4net;

namespace TestDataFramework.Populator
{
    public class StandardPopulator : IPopulator
    {
        private static ILog logger = LogManager.GetLogger(typeof (StandardPopulator));

        private readonly IValueGenerator valueGenerator;
        private IPersistence persistence;

        private readonly List<Type> typesToGenerate = new List<Type>();

        public StandardPopulator(IValueGenerator valueGenerator, IPersistence persistence)
        {
            this.valueGenerator = valueGenerator;
            this.persistence = persistence;
        }

        #region Public Methods

        public void Populate()
        {
            StandardPopulator.logger.Debug("Entering Populate");

            this.typesToGenerate.ForEach(this.GenerateRecord);

            StandardPopulator.logger.Debug("Exiting Populate");
        }

        public void Add<T>() where T : new()
        {
            StandardPopulator.logger.Debug("Entering Add<T>");

            StandardPopulator.logger.Debug("Adding type " + typeof (T));
            this.typesToGenerate.Add(typeof(T));

            StandardPopulator.logger.Debug("Exiting Add<T>");
        }

        #endregion Public Methods

        #region Private Methods

        private void GenerateRecord(Type typeToGenerate)
        {
            StandardPopulator.logger.Debug("Entering GenerateRecord");

            StandardPopulator.logger.Debug("Generaing type " + typeToGenerate);

            PropertyInfo[] propertyInfoCollection = typeToGenerate.GetProperties();

            object recordObject = typeToGenerate.GetConstructor(Type.EmptyTypes).Invoke(null);

            propertyInfoCollection.ToList().ForEach(propertyInfo => this.SetProperty(propertyInfo, recordObject));

            StandardPopulator.logger.Debug("Exiting GenerateRecord");
        }

        private void SetProperty(PropertyInfo propertyInfo, object recordObject)
        {
            StandardPopulator.logger.Debug("Entering SetProperty");

            StandardPopulator.logger.Debug("Setting property " + propertyInfo.Name);

            object value = this.valueGenerator.GetValue(propertyInfo.PropertyType);

            StandardPopulator.logger.Debug("Setting property value: " + value);
            propertyInfo.SetValue(recordObject, value);

            StandardPopulator.logger.Debug("Exiting SetProperty");
        }

        #endregion Private Methods
    }
}